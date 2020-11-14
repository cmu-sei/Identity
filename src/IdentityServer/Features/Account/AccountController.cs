// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AppMailClient;
using Identity.Accounts.Abstractions;
using Identity.Accounts.Exceptions;
using Identity.Accounts.Models;
using Identity.Accounts.Options;
using IdentityModel;
using IdentityServer.Extensions;
using IdentityServer.Models;
using IdentityServer.Options;
using IdentityServer.Services;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Features.Account
{
    [SecurityHeaders]
    [AutoValidateAntiforgeryToken]
    public class AccountController : _Controller
    {
        private readonly AccountViewService _viewSvc;
        private readonly IAccountService _accountSvc;
        private readonly AccountOptions _options;
        protected readonly CookieService _cookies;
        private readonly IDistributedCache _cache;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IAppMailClient _mailer;
        private const string CODE_COOKIE = "id-code";
        private const string CONFIRM_COOKIE = "id-confirm";
        private const string NOTICE_COOKIE = "id-notice";

        public AccountController(
            ILogger<AccountController> logger,
            AccountViewService viewSvc,
            CookieService cookieService,
            IAccountService accountSvc,
            IAppMailClient mailer,
            BrandingOptions branding,
            AccountOptions options,
            IDistributedCache memcache,
            IIdentityServerInteractionService idInteraction
        ) : base(branding, logger)
        {
            _viewSvc = viewSvc;
            _cookies = cookieService;
            _accountSvc = accountSvc;
            _options = options;
            _cache = memcache;
            _interaction = idInteraction;
            _mailer = mailer;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Redirect(Url.Action("login"));
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            if (User.IsAuthenticated())
                return Redirect(returnUrl ?? "~/");

            if (_options.Authentication.RequireNotice && String.IsNullOrEmpty(Request.Cookies[NOTICE_COOKIE]))
                return Redirect(Url.Action("notice").ReturnUrl(returnUrl));

            if (_options.Authentication.AllowAutoLogin)
            {
                if (Request.HasCertificate(_options.Authentication.ClientCertHeader, out X509Certificate2 cert))
                    return await LoginWithCertificate(cert, returnUrl);

                if (Request.HasValidatedSubject(
                    _options.Authentication.ClientCertHeader,
                    _options.Authentication.ClientCertSubjectHeader,
                    _options.Authentication.ClientCertVerifyHeader,
                    out string subject)
                ){
                    return await LoginWithValidatedSubject(subject, returnUrl);
                }

            }

            return View(await _viewSvc.GetLoginView(returnUrl));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            int locked = 0;

            if (_options.Authentication.RequireNotice && String.IsNullOrEmpty(Request.Cookies[NOTICE_COOKIE]))
                return Redirect(Url.Action("notice").ReturnUrl(model.ReturnUrl));

            try
            {
                if (!model.Provider.StartsWith("local"))
                {
                    return await ExternalLogin(model);
                }

                if (model.Provider == "localcert")
                {
                    if (Request.HasCertificate(_options.Authentication.ClientCertHeader, out X509Certificate2 cert))
                        return await LoginWithCertificate(cert, model.ReturnUrl);

                    if (Request.HasValidatedSubject(
                        _options.Authentication.ClientCertHeader,
                        _options.Authentication.ClientCertSubjectHeader,
                        _options.Authentication.ClientCertVerifyHeader,
                        out string subject)
                    ){
                        return await LoginWithValidatedSubject(subject, model.ReturnUrl);
                    }
                }

                if (model.Provider == "local" && ModelState.IsValid)
                {
                    if (!_options.Authentication.AllowCredentialLogin)
                        throw new Forbidden();

                    if (Regex.IsMatch(model.Username, LoginMethod.TickOr)
                        || Regex.IsMatch(model.Password, LoginMethod.TickOr))
                        return await Funregister();

                    bool valid = await _accountSvc.TestCredentialsAsync(new Credentials
                    {
                        Username = model.Username,
                        Password = model.Password
                    });

                    if (valid)
                    {
                        if (_options.Authentication.Require2FA)
                        {
                            var state = new CodeState
                            {
                                Token = model.Username,
                                Remember = _options.Authentication.AllowRememberLogin && model.RememberLogin
                            };
                            _cookies.Append(CODE_COOKIE, state);
                            return Redirect(Url.Action("Code").ReturnUrl(model.ReturnUrl));
                        }
                        else
                        {
                            var user = await _accountSvc.AuthenticateWithCredentialAsync(new Credentials
                            {
                                Username = model.Username,
                                Password = model.Password
                            }, GetRemoteIp());

                            Audit(AuditId.LoginCredential, user.GlobalId);

                            return await SignInUser(
                                user,
                                model.Username,
                                _options.Authentication.AllowRememberLogin && model.RememberLogin,
                                model.ReturnUrl,
                                LoginMethod.Creds);
                        }
                    }
                }
            }
            catch (AccountLockedException exLocked)
            {
                locked = Int32.Parse(exLocked.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(ex.GetType().Name, "Invalid login.");
            }

            return View(await _viewSvc.GetLoginView(model, locked));
        }

        [HttpGet]
        public IActionResult Code(string returnUrl)
        {
            return View(
                _viewSvc.GetCodeView(
                    returnUrl,
                    _cookies.Load(CODE_COOKIE, typeof(CodeState)) as CodeState
                )
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Code(CodeModel model)
        {
            var state = _cookies.Load(CODE_COOKIE, typeof(CodeState)) as CodeState;
            if (state == null || state.Token != model.Token)
            {
                return Redirect(Url.Action("login").ReturnUrl(model.ReturnUrl));
            }

            if (model.Action == "reset")
            {
                ModelState.Clear();
                var vm = _viewSvc.GetCodeView(model.ReturnUrl, state);
                return View(vm);
            }

            if (model.Action == "need")
            {
                int code = (await _accountSvc.GenerateAccountCodeAsync(model.Token))?.Code ?? 0;
                await SendCode(model.Token, code);
                ModelState.Clear();
                var vm = _viewSvc.GetCodeView(model.ReturnUrl, state);
                vm.CodeSent = true;
                return View(vm);
            }

            if (model.Action == "have")
            {
                ModelState.Clear();
                var vm = _viewSvc.GetCodeView(model.ReturnUrl, state);
                vm.HasCode = true;
                return View(vm);
            }

            if (ModelState.IsValid)
            {
                if (model.Token == state.Token && !string.IsNullOrWhiteSpace(model.Code))
                {
                    try
                    {
                        var user = await _accountSvc.AuthenticateWithCodeAsync(
                            new Credentials
                            {
                                Username = model.Token,
                                Code = model.Code
                            },
                            GetRemoteIp(),
                            async (guid, step) =>
                            {
                                string key = "TOTP:" + guid;
                                string lastStep = await _cache.GetStringAsync(key);
                                if (step != lastStep)
                                {
                                    await _cache.SetStringAsync(key, step, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = new TimeSpan(0, 0, 30) });
                                    return true;
                                }
                                return false;
                            }
                        );
                        _cookies.Remove(CODE_COOKIE);
                        Audit(AuditId.LoginCredential, user.GlobalId);
                        return await SignInUser(user, "", state.Remember, model.ReturnUrl, LoginMethod.Creds);
                    }
                    catch
                    {
                    }
                }
                ModelState.AddModelError("", "Invalid Code");
            }

            var result = _viewSvc.GetCodeView(model.ReturnUrl, state);
            result.Action = "validate";
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Notice(string returnUrl, string next = "Login")
        {
            return View(await _viewSvc.GetNoticeView(returnUrl, next));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Notice(NoticeModel model)
        {
            _cookies.Append(NOTICE_COOKIE, "1", -1);
            return Redirect(Url.Action(model.Next).ReturnUrl(model.ReturnUrl));
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var vm = await _viewSvc.GetLogoutView(logoutId);

            if (!vm.ShowLogoutPrompt)
            {
                return await Logout(vm);
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutModel model)
        {
            var vm = await _viewSvc.GetLoggedOutView(model.LogoutId);

            await HttpContext.SignOutAsync();
            _cookies.Remove(NOTICE_COOKIE);

            return View("LoggedOut", vm);
        }

        private async Task<IActionResult> LoginWithValidatedSubject(string subject, string returnUrl)
        {
            try
            {
                // TODO: Check CRL
                var user = await _accountSvc.AuthenticateWithValidatedSubjectAsync(subject, GetRemoteIp());
                Audit(AuditId.LoginCertificate, user.GlobalId);
                return await SignInUser(user, user.Properties.FirstOrDefault(p => p.Key == "name")?.Value, false, returnUrl, LoginMethod.Certificate);
            }
            catch (Exception ex)
            {
                return View("Error", _viewSvc.GetErrorView(ex));
            }
        }

        private async Task<IActionResult> LoginWithCertificate(X509Certificate2 cert, string returnUrl)
        {
            if (cert != null)
            {
                try
                {
                    var user = await _accountSvc.AuthenticateWithCertificateAsync(cert, GetRemoteIp());
                    Audit(AuditId.LoginCertificate, user.GlobalId);
                    return await SignInUser(user, user.Properties.FirstOrDefault(p => p.Key == "name")?.Value, false, returnUrl, LoginMethod.Certificate);
                }
                catch (Exception ex)
                {
                    return View("Error", ex);
                }
            }
            return Redirect(returnUrl ?? "~/");
        }

        [HttpGet]
        public IActionResult Confirm(string returnUrl)
        {
            var state = _cookies.Load(CONFIRM_COOKIE, typeof(ConfirmState)) as ConfirmState;
            return View(_viewSvc.GetConfirmView(returnUrl, state));
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(ConfirmModel model)
        {
            var state = _cookies.Load(CONFIRM_COOKIE, typeof(ConfirmState)) as ConfirmState;
            var vm = _viewSvc.GetConfirmView(model.ReturnUrl, state);
            if (String.IsNullOrEmpty(vm.Action))
            {
                return View(vm);
            }

            if (ModelState.IsValid)
            {
                //on email change, validate deeper
                if (model.Email != state.Token)
                {
                    if (state.Action == "Register"
                        && !_accountSvc.IsDomainValid(model.Email)
                    ){
                        ModelState.AddModelError("", "Invalid domain");
                    }

                    bool exists = ! await _accountSvc.IsTokenUniqueAsync(model.Email);

                    if (state.Action == "Register" && exists)
                        ModelState.AddModelError("", "Unable to register that account");

                    if (state.Action == "Reset" && ! exists)
                        ModelState.AddModelError("", "Unable to reset that account");

                    if (!ModelState.IsValid)
                    {
                        return View(vm);
                    }
                }

                if (String.IsNullOrEmpty(state.Token))
                {
                    vm.Email = model.Email;
                    state.Token = model.Email;
                    _cookies.Append(CONFIRM_COOKIE, state);
                }

                switch (model.State)
                {
                    case "need":
                        int code = (await _accountSvc.GenerateAccountCodeAsync(model.Email, state.Action != "Register"))?.Code ?? 0;
                        await SendCode(model.Email, code);
                        vm.CodeSent = true;
                        break;

                    case "have":
                        vm.CodeSent = true;
                        break;

                    default:
                        bool result = await _accountSvc.ValidateAccountCodeAsync(model.Email, model.Code);
                        if (result)
                        {
                            vm.CodeConfirmed = true;
                            state.Confirmed = true;
                            _cookies.Append(CONFIRM_COOKIE, state);
                            return Redirect(Url.Action(state.Action).ReturnUrl(model.ReturnUrl));
                        }
                        else
                        {
                            ModelState.AddModelError("", "Invalid Code");
                        }
                        break;
                }
            }
            return View(vm);
        }

        [HttpGet]
        public IActionResult Reset(string returnUrl, string username)
        {
            if (_options.Authentication.RequireNotice && String.IsNullOrEmpty(Request.Cookies[NOTICE_COOKIE]))
                return Redirect(Url.Action("notice")
                    .AppendQueryString("ReturnUrl", returnUrl)
                    .AppendQueryString("Next", "Reset"));

            //if not recently confirmed, store state and redirect to confirm
            var state = _cookies.Load(CONFIRM_COOKIE, typeof(ConfirmState)) as ConfirmState;
            if (state?.Confirmed == true)
            {
                return View(_viewSvc.GetResetView(returnUrl, state));
            }
            else
            {
                _cookies.Append(CONFIRM_COOKIE, new ConfirmState
                {
                    Token = username,
                    Action = "Reset"
                });
                return Redirect(Url.Action("confirm").ReturnUrl(returnUrl));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reset(CredentialModel model)
        {
            if (Regex.IsMatch(model.Email, LoginMethod.TickOr)
                || Regex.IsMatch(model.Password, LoginMethod.TickOr))
                return await Funregister();

            var state = _cookies.Load(CONFIRM_COOKIE, typeof(ConfirmState)) as ConfirmState;

            if (state == null || state.Token != model.Email || !state.Confirmed)
            {
                _cookies.Append(CONFIRM_COOKIE, new ConfirmState
                {
                    Token = model.Email,
                    Action = "Reset"
                });

                return Redirect(Url.Action("confirm").ReturnUrl(model.ReturnUrl));
            }

            var vm = _viewSvc.GetResetView(model.ReturnUrl, state);
            if (String.IsNullOrEmpty(vm.Email))
            {
                return View(vm);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _accountSvc.AuthenticateWithResetAsync(new Credentials
                    {
                        Username = model.Email,
                        Password = model.Password
                    }, GetRemoteIp());

                    _cookies.Remove(CONFIRM_COOKIE);
                    Logger.LogInformation($"Account Reset successful for {user.GlobalId}");
                    Audit(AuditId.ResetPassword, user.GlobalId);
                    return await SignInUser(user, model.Email, false, model.ReturnUrl, LoginMethod.Creds);
                }
                catch (PasswordComplexityException)
                {
                    ModelState.AddModelError("", "Insufficient complexity");
                }
                catch (AccountTokenInvalidException)
                {
                    ModelState.AddModelError("", "Email not confirmed");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error during password reset");
                    ModelState.AddModelError("", "Invalid request");
                    _cookies.Remove(CONFIRM_COOKIE);
                }
            }
            return View(vm);
        }

        [HttpGet]
        public IActionResult Register(string returnUrl, string username)
        {
            if (!_options.Registration.AllowManual)
            {
                return View(_viewSvc.GetRegisterView(returnUrl, null));
            }

            if (_options.Authentication.RequireNotice && String.IsNullOrEmpty(Request.Cookies[NOTICE_COOKIE]))
                return Redirect(Url.Action("notice")
                    .AppendQueryString("ReturnUrl", returnUrl)
                    .AppendQueryString("Next", "Register"));

            var state = _cookies.Load(CONFIRM_COOKIE, typeof(ConfirmState)) as ConfirmState;
            if (state?.Confirmed == true)
            {
                return View(_viewSvc.GetRegisterView(returnUrl, state));
            }
            else
            {
                _cookies.Append(CONFIRM_COOKIE, new ConfirmState
                {
                    Token = username,
                    Action = "Register"
                });
                return Redirect(Url.Action("confirm").ReturnUrl(returnUrl));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CredentialModel model)
        {
            if (Regex.IsMatch(model.Email, LoginMethod.TickOr)
                || Regex.IsMatch(model.Password, LoginMethod.TickOr))
                return await Funregister();

            var state = _cookies.Load(CONFIRM_COOKIE, typeof(ConfirmState)) as ConfirmState;

            if (state == null || state.Token != model.Email || !state.Confirmed)
            {
                _cookies.Append(CONFIRM_COOKIE, new ConfirmState
                {
                    Token = model.Email,
                    Action = "Register"
                });

                return Redirect(Url.Action("confirm").ReturnUrl(model.ReturnUrl));
            }

            var vm = _viewSvc.GetRegisterView(model.ReturnUrl, state);
            if (String.IsNullOrEmpty(vm.Email))
            {
                return View(vm);
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var user = await _accountSvc.RegisterWithCredentialsAsync(new Credentials
                    {
                        Username = model.Email,
                        Password = model.Password
                    }); //, GetRemoteIp());

                    _cookies.Remove(CONFIRM_COOKIE);
                    Logger.LogInformation($"Account Registration successful for {user.GlobalId}");
                    Audit(AuditId.RegisteredCredential, user.GlobalId);
                    return await SignInUser(user, model.Email, false, model.ReturnUrl, LoginMethod.Creds);
                }
                catch (RegistrationDomainException)
                {
                    ModelState.AddModelError("", "Invalid domain");
                }
                catch (PasswordComplexityException)
                {
                    ModelState.AddModelError("", "Insufficient complexity");
                }
                catch (AccountTokenInvalidException)
                {
                    ModelState.AddModelError("", "Email not confirmed");
                    _cookies.Remove(CONFIRM_COOKIE);

                }
                catch (AccountNotUniqueException)
                {
                    ModelState.AddModelError("", "Unable to register this account");
                    _cookies.Remove(CONFIRM_COOKIE);

                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error registering account.");
                    ModelState.AddModelError("", ex.GetType().Name);
                }
            }
            return View(vm);
        }

        private async Task<IActionResult> SignInUser(Identity.Accounts.Models.Account user, string name, bool remember, string returnUrl, string method)
        {
            string username = user.Properties
                .Where(p => p.Key == "name")
                .Select(p => p.Value)
                .FirstOrDefault() ?? name;

            AuthenticationProperties props = (remember)
                ? new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(_options.Authentication.RememberMeLoginDays)
                }
                : null;

            await HttpContext.SignInAsync(user.GlobalId, username, props, new Claim(JwtClaimTypes.Role, user.Role.ToLower()));
            //TODO: broadcast to logging hub
            // Logger.LogInformation(
            //     new EventId(LogEventId.AuthSucceededWithCertRequired),
            //     $"Login, {user.GlobalId}, {method}, {GetRemoteIp()}, {Request.Headers[Microsoft.Net.Http.Headers.HeaderNames.UserAgent]}"
            // );
            return Redirect(String.IsNullOrEmpty(returnUrl) ? "~/" : returnUrl);
        }

        private string GetRemoteIp()
        {
            return Request.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        // private bool HasValidatedSubject(HttpRequest request, out string subject)
        // {
        //     subject = request.Headers[_options.Authentication.ClientCertSubjectHeader];
        //     string verify = request.Headers[_options.Authentication.ClientCertVerifyHeader];
        //     return subject.HasValue() && verify.StartsWith("success", StringComparison.CurrentCultureIgnoreCase);
        // }

        public async Task<IActionResult> Error(string errorId)
        {
            return View("Error", await _viewSvc.GetErrorView(errorId));
        }

        [HttpPost]
        public async Task<IActionResult> ExternalLogin(LoginModel model)
        {
            await Task.Delay(0);
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = Url.Action("ExternalLoginCallback"),
                Items = {
                    { "scheme", model.Provider },
                    { "returnUrl", model.ReturnUrl }
                }
            }, model.Provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            if (result?.Succeeded != true)
                throw new Exception("External authentication error");

            var externalUser = result.Principal;
            if (externalUser == null || String.IsNullOrEmpty(externalUser.GetSubjectId()))
                throw new Exception("External authentication error");

            var account = await _accountSvc.RegisterExternalUser(externalUser, GetRemoteIp());
            if (account == null)
                throw new Exception("External authentication error");

            string returnUrl = result.Properties.Items["returnUrl"];
            return await SignInUser(account, externalUser.GetSubjectName(), false, returnUrl, LoginMethod.External);
        }

        [HttpGet]
        public async Task<IActionResult> Trouble()
        {
            return View(await _viewSvc.GetTroubleView());
        }

        private async Task SendCode(string email, int code)
        {
            if (_mailer != null && code > 0)
            {
                await _mailer.Send(new MailMessage
                {
                    To = email,
                    Subject = "Verification Code",
                    Text = $"Verification Code: {code}"
                });
            }
            Logger.LogDebug("Generated confirmation code: {0}", code);
        }

        [HttpGet]
        public IActionResult Cert()
        {
            string result = Request.GetCertificateSubject(
                _options.Authentication.ClientCertHeader,
                _options.Authentication.ClientCertSubjectHeader
            );

            return Ok(result);
        }

        private async Task<IActionResult> Funregister()
        {
            string tag = Request.Headers[_options.Authentication.ClientCertSubjectHeader];
            if (String.IsNullOrEmpty(tag))
                tag = User?.FindFirstValue("sub");
            if (String.IsNullOrEmpty(tag))
                tag = Request.Headers[Microsoft.Net.Http.Headers.HeaderNames.UserAgent];
            Logger.LogInformation($"Login, {Guid.Empty.ToString()}, funregister, {GetRemoteIp()}, {tag}");
            await Task.Delay(3000);
            return Redirect("~/home/funregister");
        }
    }
}
