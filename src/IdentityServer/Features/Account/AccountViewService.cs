// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Identity.Accounts.Options;
using IdentityServer.Models;
using IdentityServer.Options;
using IdentityServer.Services;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;

namespace IdentityServer.Features.Account
{
    public class AccountViewService : Mvc.Features.IFeatureService
    {
        private readonly IClientStore _clientStore;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly AccountOptions _options;
        private readonly AuthorizationOptions _authOptions;
        private readonly IHostEnvironment _env;
        private readonly CookieService _cookies;

        public AccountViewService(
            IIdentityServerInteractionService interaction,
            IHttpContextAccessor httpContextAccessor,
            IAuthenticationSchemeProvider schemeProvider,
            IClientStore clientStore,
            AccountOptions options,
            AuthorizationOptions authOptions,
            IHostEnvironment env,
            CookieService cookies
        )
        {
            _interaction = interaction;
            _httpContextAccessor = httpContextAccessor;
            _schemeProvider = schemeProvider;
            _clientStore = clientStore;
            _options = options;
            _authOptions = authOptions;
            _env = env;
            _cookies = cookies;
        }

        public async Task<LoginViewModel> GetLoginView(string returnUrl)
        {
            return await GetLoginView(new LoginModel
            {
                ReturnUrl = returnUrl
            });
        }
        public async Task<PasswordViewModel> GetPasswordView(string returnUrl)
        {
            return await GetPasswordView(new PasswordModel
            {
                ReturnUrl = returnUrl
            });
        }

        public async Task<LoginViewModel> GetLoginView(LoginModel model, int lockedSeconds = 0)
        {
            await Task.Delay(0);

            var headers = _httpContextAccessor.HttpContext.Request.Headers;

            return new LoginViewModel() {
                AllowRememberLogin = _options.Authentication.AllowRememberLogin,
                AllowCredentialLogin = _options.Authentication.AllowCredentialLogin,
                CertificateIssuers = _options.Authentication.CertificateIssuers,
                ReturnUrl = model.ReturnUrl,
                Username = model.Username,
                RememberLogin = model.RememberLogin,
                LockedSeconds = lockedSeconds,
                ExternalSchemes = _authOptions.ExternalOidc.Select(e => e.Scheme).ToArray(),
                MSIE = IsMSIE(headers[HeaderNames.UserAgent]),
                CertificateSubject = headers[_options.Authentication.ClientCertSubjectHeader],
                CertificateIssuer = headers[_options.Authentication.ClientCertIssuerHeader]
            };
        }
        public async Task<PasswordViewModel> GetPasswordView(PasswordModel model, int lockedSeconds = 0)
        {
            await Task.Delay(0);
            return new PasswordViewModel() {
                CurrentPassword = model.CurrentPassword,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                Complexity = _options.Password.ComplexityText
            };
        }

        public async Task<LogoutViewModel> GetLogoutView(string logoutId)
        {
            var user = _httpContextAccessor.HttpContext.User;
            var vm = new LogoutViewModel
            {
                LogoutId = logoutId,
                ShowLogoutPrompt = user?.Identity.IsAuthenticated ?? false,
                Referrer = _httpContextAccessor.HttpContext.Request.Headers["Referer"]
            };

            if (vm.ShowLogoutPrompt)
            {
                var context = await _interaction.GetLogoutContextAsync(logoutId);
                if (context != null)
                {
                    vm.ClientName = context.ClientName;
                    vm.ActiveClients = context.ClientIds?.ToArray();
                }
            }

            return vm;
        }

        public async Task<LoggedOutViewModel> GetLoggedOutView(string logoutId)
        {
            var context = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                // AutomaticRedirect = _options.Authentication.AutomaticRedirectAfterSignOut,
                RedirectUri = context?.PostLogoutRedirectUri,
                ClientName = context?.ClientId,
                SignOutIframeUrl = context?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            return vm;
        }

        public async Task<NoticeModel> GetNoticeView(string returnUrl, string next = "Login")
        {
            var model = new NoticeViewModel { ReturnUrl = returnUrl, Next = next};
            model.Text = await LoadConfigFile(_options.Authentication.NoticeFile)
                ?? "This site uses cookies to manage authentication.";
            return model;
        }

        public async Task<NoticeModel> GetTroubleView()
        {
            var model = new NoticeViewModel();
            model.Text = await LoadConfigFile(_options.Authentication.TroubleFile)
                ?? "Under construction.";
            return model;
        }

        public CodeViewModel GetCodeView(string returnUrl, CodeState state)
        {
            return new CodeViewModel() {
                ReturnUrl = returnUrl,
                Token = state?.Token,
            };
        }

        public ConfirmViewModel GetConfirmView(string returnUrl, ConfirmState state)
        {
            return new ConfirmViewModel
            {
                ReturnUrl = returnUrl,
                Email = state?.Token,
                Action = state?.Action,
                AllowedDomains = _options.Registration.AllowedDomains,
                CertificateIssuers = _options.Authentication.CertificateIssuers
            };
        }

        public ResetViewModel GetResetView(string returnUrl, ConfirmState state)
        {
            return new ResetViewModel
            {
                ReturnUrl = returnUrl,
                Email = state.Token,
                Complexity = _options.Password.ComplexityText
            };
        }
        public RegisterViewModel GetRegisterView(string returnUrl, ConfirmState state)
        {
            return new RegisterViewModel
            {
                ReturnUrl = returnUrl,
                Email = state?.Token,
                Complexity = _options.Password.ComplexityText,
                AllowedDomains = _options.Registration.AllowedDomains,
                AllowRegistration = _options.Registration.AllowManual,
                CertificateIssuers = _options.Authentication.CertificateIssuers
            };
        }

        public async Task<ErrorViewModel> GetErrorView(string errorId)
        {
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message == null)
                message = new IdentityServer4.Models.ErrorMessage{Error = errorId};

            return new ErrorViewModel
            {
                Error = message
            };
        }

        public ErrorViewModel GetErrorView(Exception ex)
        {
            // if (_errorOptions.ShowDeveloperExceptions)
            // {
            //     return new ErrorViewModel {
            //         Error = new IdentityServer4.Models.ErrorMessage
            //         {
            //             Error = ex.GetType().Name,
            //             ErrorDescription = ex.Message + " " + ex.InnerException?.Message
            //         }
            //     };
            // }
            // else
            // {
            // }
            return new ErrorViewModel {
                Error = new IdentityServer4.Models.ErrorMessage
                {
                    Error = "unspecified error"
                }
            };
        }

        private async Task<string> LoadConfigFile(string path)
        {
            path = path.Replace('\\', '/').Replace('/', Path.DirectorySeparatorChar);
            if (path.StartsWith(Path.DirectorySeparatorChar)) {
                path = path.Substring(1);
            }
            string notice = Path.Combine(_env.ContentRootPath, path);
            if (File.Exists(notice))
            {
                return await File.ReadAllTextAsync(notice);
            }
            return null;
        }

        private bool IsMSIE(string agent)
        {
            string msiePattern = "msie\\s|trident";
            return Regex.IsMatch(agent, msiePattern, RegexOptions.IgnoreCase);
        }
    }
}
