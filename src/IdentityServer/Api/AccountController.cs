// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppMailClient;
using Identity.Accounts.Abstractions;
using Identity.Accounts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Api
{
    [Authorize(Policy = AppConstants.PrivilegedPolicy)]
    public class AccountController : _Controller
    {
        public AccountController (
            ILogger<AccountController> logger,
            IAccountService svc,
            IAppMailClient mailer
        ) : base(logger)
        {
            _svc = svc;
            _mailer = mailer;
        }

        private readonly IAccountService _svc;
        private readonly IAppMailClient _mailer;

        [HttpGet("api/accounts")]
        [ProducesResponseType(typeof(Account[]), 200)]
        public async Task<IActionResult> List([FromQuery]SearchModel search, CancellationToken ct)
        {
            if (!String.IsNullOrEmpty(search.Since))
                return Json(await _svc.FindUpdated(search.Since, ct));

            return Json(await _svc.FindAll(search, ct));
        }

        [HttpPut("api/account/{id}/state/{state}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetStatus([FromRoute]int id, [FromRoute]AccountStatus state)
        {
            await _svc.SetStatus(id, state);
            Audit(AuditId.UserState, id, state);
            return Ok();
        }

        [HttpPut("api/account/{id}/unlock")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Unlock([FromRoute]int id)
        {
            await _svc.Unlock(id);
            Audit(AuditId.ClearLock, id);
            return Ok();
        }

        [Authorize(Roles=AppConstants.AdminRole)]
        [HttpPut("api/account/{id}/role/{role}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetRole([FromRoute]int id, [FromRoute]string role)
        {
            await _svc.SetRole(id, Enum.Parse<AccountRole>(role, true));
            Audit(AuditId.UserRole, id, role);
            return Ok();
        }

        [HttpPut("api/account/{id}/code")]
        [ProducesResponseType(typeof(AccountCode), 200)]
        public async Task<IActionResult> GetVerificationCode([FromRoute]int id)
        {
            return Json(await _svc.GenerateAccountCodeAsync(id));
        }

        [HttpPost("api/account")]
        [ProducesResponseType(typeof(UsernameRegistration[]), 201)]
        public async Task<IActionResult> CreateAccount([FromBody]UsernameRegistrationModel model)
        {
            var result = await _svc.RegisterUsernames(model);
            int success = result.Count(r => string.IsNullOrEmpty(r.Message));
            Audit(AuditId.RegisteredCredential, "import", success);
            return Json(result);
        }

        [HttpPut("api/account/property")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetProperty([FromBody]AccountProperty property)
        {
            await _svc.SetProperty(property);
            return Ok();
        }

        [HttpPut("/api/account/merge")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> PostMergeCode([FromBody] AccountMergeModel model)
        {
            await _svc.MergeAccounts(model);
            Audit(AuditId.MergeAccount, "admin", model.DefunctGlobalId, model.ActiveGlobalId);
            return Ok();
        }

        [HttpPost("api/account/mail")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SendEmail([FromBody]RelayMailMessage message)
        {
            List<string> addresses = new List<string>();
            foreach(string id in message.To)
            {
                var account = await _svc.FindByGuidAsync(id);
                if (account != null)
                {
                    string name = account.Properties.FirstOrDefault(p => p.Key == Identity.Accounts.ClaimTypes.Name)?.Value;
                    foreach (var email in account.Properties.Where(p => p.Key == Identity.Accounts.ClaimTypes.Email).Select(p => p.Value))
                    {
                        addresses.Add($"{name} <{email}>");
                    }
                }
            }

            if (addresses.Count == 0)
                throw new Exception("No valid addresses.");

            await _mailer.Send(new MailMessage
            {
                To = String.Join("; ", addresses.ToArray()),
                Subject = message.Subject,
                Text = message.Body
            });

            return Ok();
        }

        [HttpGet("api/version")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult Version()
        {
            return Ok(Environment.GetEnvironmentVariable("COMMIT") ?? "dev");
        }
    }

    public class RelayMailMessage
    {
        public string[] To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
