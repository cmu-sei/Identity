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
using IdentityServer.Extensions;
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

        [HttpPut("api/account/{id}/token")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> AddUsername([FromRoute]string id, [FromBody] Credentials model)
        {
            await _svc.AddAccountUsernameAsync(id, model.Username);
            Audit(AuditId.AddUsername, id, model.Username);
            return Ok();
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
        public async Task<List<MailMessageStatus>> SendEmail([FromBody]RelayMailMessage message)
        {
            string batchId = new Random().Next().ToString("x");

            var list = await _svc.FindAll(
                new SearchModel
                {
                    Term = "#enabled"
                }
            );

            if (!message.Groups.Any() && message.To.Any())
            {
                if (message.To.First() == "@here")
                {
                    message.Groups = list.Select(a =>
                        new RecipientGroup
                        {
                            Name = a.Properties.FirstOrDefault(p => p.Key == "name")?.Value,
                            Members = new string[] {a.GlobalId }
                        }
                    ).ToArray();
                }
                else
                {
                    message.Groups = new RecipientGroup[] {
                        new RecipientGroup
                        {
                            Members = message.To
                        }
                    };
                }
            }

            var results = new List<MailMessageStatus>(message.Groups.Length);

            foreach (var group in message.Groups)
            {
                string to = ResolveRecipients(list, group.Members);
                var result = await SendMessage(message, to, group.Name, batchId);
                results.Add(result);
            }

            return results;
        }

        [HttpPost("api/account/mailverify")]
        public async Task<MailMessageStatus[]> VerifyMail([FromBody] MailMessageStatus[] list)
        {
            foreach (var item in list.Where(r => r.Status == "pending"))
            {
                var response = await _mailer.Status(item.ReferenceId);
                item.Status = response.Status;
            }

            return list;
        }

        private async Task<MailMessageStatus> SendMessage(RelayMailMessage message, string to, string name, string batchId)
        {
            var msg = new MailMessage
            {
                To = to,
                Cc = string.Join("; ", message.Cc),
                Bcc = string.Join("; ", message.Bcc),
                From = message.From,
                Subject = message.Subject,
                MessageId = $"{batchId}:{name}:{to}"
            };

            if (message.Body.StartsWith("<!doctype html", true, null))
                msg.Html = message.Body.Replace("{name}", name);
            else
                msg.Text = message.Body.Replace("{name}", name);

            var response = await _mailer.Send(msg);

            return response;
        }

        private string ResolveRecipients(Account[] list, string[] to)
        {
            var addresses = new List<string>();

            foreach (var item in to.Select(x => x.Trim()))
            {
                // item is id or email
                var account = list.FirstOrDefault(a => a.GlobalId == item);

                if (account is null)
                    account = list.FirstOrDefault(a => a.Properties.Any(p => p.Key == "email" && p.Value == item));

                if (account is null)
                    continue;

                string name = account.Properties
                    .FirstOrDefault(p => p.Key == Identity.Accounts.ClaimTypes.Name)?.Value;

                addresses.AddRange(account.Properties
                    .Where(p => p.Key == Identity.Accounts.ClaimTypes.Email)
                    .Select(p => $"{name} <{p.Value}>")
                    .ToList()
                );

            }

            return String.Join("; ", addresses.ToArray());
        }

        [HttpGet("api/stats")]
        [ProducesResponseType(typeof(AccountStats), 200)]
        public async Task<IActionResult> GetStats([FromQuery] DateTime since)
        {
            return Ok(await _svc.GetStats(since));
        }

        [HttpGet("api/version")]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult Version()
        {
            return Ok(Environment.GetEnvironmentVariable("COMMIT") ?? "dev");
        }
    }

    public class RelayMailMessage
    {
        public string[] To { get; set; } = new string[] {};
        public string[] Cc { get; set; } = new string[] {};
        public string[] Bcc { get; set; } = new string[] {};
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public RecipientGroup[] Groups { get; set; } = new RecipientGroup[] {};
    }

    public class RecipientGroup
    {
        public string Name { get; set; }
        public string[] Members { get; set; }
    }
}
