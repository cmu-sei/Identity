// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System.Threading.Tasks;
using Identity.Accounts.Models;
using Identity.Accounts.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using IdentityServer.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Cryptography.X509Certificates;
using Identity.Accounts.Options;
using Identity.Accounts;
using AppMailClient;
using Identity.Accounts.Exceptions;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Api
{
    [Authorize]
    public class ProfileController : _Controller
    {
        public ProfileController (
            ILogger<ProfileController> logger,
            IAccountService accountService,
            IDistributedCache cache,
            AccountOptions options,
            IAppMailClient mailer
        ) : base(logger)
        {
            _svc = accountService;
            _cache = cache;
            _options = options;
            _mailer = mailer;
        }

        private readonly IAccountService _svc;
        private readonly IDistributedCache _cache;
        private readonly AccountOptions _options;
        private readonly IAppMailClient _mailer;

        [HttpGet("api/profile/{id?}")]
        [ProducesResponseType(typeof(AccountProfile), 200)]
        public async Task<IActionResult> GetProfile([FromRoute]string id = null)
        {
            var profile = await _svc.FindProfile(
                id ?? User.GetSubjectId(),
                User.GetSubjectId() == id || String.IsNullOrEmpty(id)
            );
            return Ok(profile);
        }

        [HttpPut("api/profile")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Put([FromBody]AccountProfile model)
        {
            if (!User.IsPrivilegedOrSelf(model.GlobalId))
                return new ForbidResult();

            await _svc.SaveProfile(model);
            return Ok();
        }

        [HttpGet("/api/profile/alt")]
        [ProducesResponseType(typeof(AlternateAccountProfile), 200)]
        public async Task<IActionResult> GetAlt()
        {
            string id = User.GetSubjectId();
            var profile = await _svc.FindAlternateProfile(id);
            if (profile != null)
            {
                //TODO: decide if we want to abstract email address
                // profile.Email = $"{id}@{Request.Host}";
                if (String.IsNullOrEmpty(profile.Email))
                {
                    profile.Email = $"{id}@{Request.Host.Host.ToHostDomain()}";
                }
            }
            return Ok(profile);
        }

        [HttpGet("/api/profile/auth")]
        [ProducesResponseType(typeof(AuthProfile), 200)]
        public IActionResult GetAuth()
        {
            return Ok(new AuthProfile
            {
                Id = User.GetSubjectId(),
                Role = User.GetRole()
            });
        }

        [HttpGet("/api/profile/merge")]
        [ProducesResponseType(typeof(AccountMergeModel), 200)]
        public async Task<IActionResult> GetMergeCode()
        {
            var opts = new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = new TimeSpan(0, 5, 0)
            };

            string key = $"{User.GetSubjectId()}:mergecode";
            string code = await _cache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(code))
                await _cache.RemoveAsync(code);

            code = Guid.NewGuid().ToString("N");
            await _cache.SetStringAsync(code, User.GetSubjectId(), opts);
            await _cache.SetStringAsync(key, code, opts);

            return Ok(new AccountMergeModel {
                Code = code,
                DefunctGlobalId = User.GetSubjectId()
            });
        }

        [HttpPost("/api/profile/merge")]
        [ProducesResponseType(typeof(AccountMergeModel), 200)]
        public async Task<IActionResult> PostMergeCode([FromBody] AccountMergeModel model)
        {
            string defunctGlobalId = await _cache.GetStringAsync(model.Code);
            if (String.IsNullOrEmpty(defunctGlobalId))
                throw new AccountNotConfirmedException();

            await _cache.RemoveAsync(model.Code);
            await _cache.RemoveAsync(defunctGlobalId + ":mergecode");

            model.DefunctGlobalId = defunctGlobalId;
            model.ActiveGlobalId = User.GetSubjectId();
            await _svc.MergeAccounts(model);

            Audit(AuditId.MergeAccount, model.DefunctGlobalId, model.ActiveGlobalId);

            return Ok();
        }

        [HttpGet("/api/profile/totp")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> GenerateTOTP()
        {
            string url = await _svc.GenerateAccountTOTPAsync(User.GetSubjectId());
            url += "&issuer=" + HttpContext.Request.Host.Host;

            Audit(AuditId.GenerateTotp);

            return Ok(url);
        }

        [HttpPost("/api/profile/totp")]
        [ProducesResponseType(typeof(TotpResult), 200)]
        public async Task<IActionResult> ValidateTOTP([FromQuery]string code)
        {
            string id = User.GetSubjectId();

            var result = await _svc.ValidateAccountTOTPAsync(id, code, async (guid, step) =>
            {
                string key = "TOTP:" + guid;
                string lastStep = await _cache.GetStringAsync(key);
                if (step != lastStep)
                {
                    await _cache.SetStringAsync(key, step, new DistributedCacheEntryOptions { SlidingExpiration = new TimeSpan(0,0,30)});
                    return true;
                }
                return false;
            });

            return Ok(result);
        }

        [HttpGet("/api/profile/tokens")]
        [ProducesResponseType(typeof(TokenSummary), 200)]
        public async Task<IActionResult> GetTokenSummary()
        {
            var model = await _svc.GetTokenSummary(new TokenSummary
            {
                GlobalId = User.GetSubjectId(),

                CurrentCertificateIssuer = Request.GetCertificateIssuer(
                    _options.Authentication.ClientCertHeader,
                    _options.Authentication.ClientCertIssuerHeader
                ),

                CurrentCertificateSubject = Request.GetCertificateSubject(
                    _options.Authentication.ClientCertHeader,
                    _options.Authentication.ClientCertSubjectHeader
                )
            });

            return Json(model);
        }

        [HttpGet("/api/profile/cert")]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult GetCurrentCertSubject()
        {
            string subject = Request.GetCertificateSubject(
                _options.Authentication.ClientCertHeader,
                _options.Authentication.ClientCertSubjectHeader
            );
            return Ok(subject);
        }

        [HttpPost("/api/profile/cert")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetCurrentCertSubject()
        {
            string subject = "";

            if (Request.HasCertificate(_options.Authentication.ClientCertHeader, out X509Certificate2 cert))
            {
                await _svc.AddAccountCertificate(User.GetSubjectId(), cert);
                Audit(AuditId.RegisteredCertificate);
            }
            else
            {
                if (Request.HasValidatedSubject(
                    _options.Authentication.ClientCertHeader,
                    _options.Authentication.ClientCertSubjectHeader,
                    _options.Authentication.ClientCertVerifyHeader,
                    out subject)
                ){
                    await _svc.AddAccountValidatedSubject(User.GetSubjectId(), subject);
                    Audit(AuditId.RegisteredCertificate);
                }
                else
                {
                    throw new InvalidOperationException("CertificateNotVerified");
                }

            }

            return Ok();
        }

        [HttpPost("/api/profile/code")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SendUsernameCode([FromBody] Credentials model)
        {
            if (! await _svc.IsTokenUniqueAsync(model.Username))
                return Ok();

            var opts = new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = new TimeSpan(0, 30, 0)
            };

            string key = $"{User.GetSubjectId()}:{model.Username}:verifycode";
            string code = new Random().Next(123456, 987654).ToString();
            await _cache.SetStringAsync(key, code, opts);

            Logger.LogDebug("generated code: " + code);

            if (_mailer != null)
            {
                await _mailer.Send(new MailMessage
                {
                    To = model.Username,
                    Subject = "Verification Code",
                    Text = $"Verification Code: {code}"
                });
            }
            return Ok();
        }

        [HttpPost("/api/profile/username")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> AddUsername([FromBody] Credentials model)
        {
            string key = $"{User.GetSubjectId()}:{model.Username}:verifycode";
            string code = await _cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(code) || model.Code != code)
                throw new AccountNotConfirmedException();

            await _svc.AddorUpdateAccountAsync(User.GetSubjectId(), model.Username);
            await _cache.RemoveAsync(key);

            Audit(AuditId.RegisteredCredential);

            return Ok();
        }

        [HttpPost("/api/profile/password")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangedPassword model)
        {
            await _svc.ChangePasswordAsync(User.GetSubjectId(), model);
            Audit(AuditId.ResetPassword);
            return Ok();
        }

    }
}
