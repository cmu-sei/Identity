// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Threading.Tasks;
using Identity.Clients.Models;
using Identity.Clients.Services;
using IdentityServer.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Api
{
    [Authorize]
    public class ClientController : _Controller
    {
        public ClientController (
            ILogger<ClientController> logger,
            ClientService svc,
            BrandingOptions branding
        ) : base(logger)
        {
            _svc = svc;
            _branding = branding;
        }

        private readonly ClientService _svc;
        private readonly BrandingOptions _branding;

        [AllowAnonymous]
        [HttpGet("api/clients")]
        [ProducesResponseType(typeof(ClientSummary[]), 200)]
        public async Task<IActionResult> List([FromQuery]SearchModel search)
        {
            return Json(await _svc.Find(search));
        }

        [HttpGet("api/client/{id}")]
        [ProducesResponseType(typeof(Client), 200)]
        public async Task<IActionResult> Load([FromRoute]int id)
        {
            return Json(await _svc.Load(id));
        }

        [HttpPost("api/client")]
        [ProducesResponseType(typeof(ClientSummary), 200)]
        public async Task<IActionResult> Add([FromBody]NewClient model)
        {
            var client = await _svc.Add(model);
            Audit(AuditId.ClientState);
            return Json(client);
        }

        [HttpPut("api/client")]
        [ProducesResponseType(typeof(Client), 200)]
        public async Task<IActionResult> Update([FromBody]Client model)
        {
            var client = await _svc.Update(model);
            Audit(AuditId.ClientState);
            return Json(client);
        }

        [HttpDelete("api/client/{id}")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            await _svc.Delete(id);
            Audit(AuditId.ClientDelete);
            return Ok();
        }

        [HttpPut("api/client/{id}/secret")]
        [ProducesResponseType(typeof(ClientSecret), 200)]
        public async Task<IActionResult> GenerateSecret([FromRoute]int id)
        {
            return Json(await _svc.AddSecret(id));
        }

        [HttpPut("api/client/{id}/code")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> NewEnlistCode([FromRoute]int id)
        {
            string code = await _svc.NewEnlistCode(id);
            return Json(new { Code = code });
        }

        [HttpPost("api/client/enlist/{code}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Enlist([FromRoute]string code)
        {
            await _svc.Enlist(code);
            Audit(AuditId.AcceptInvite, code);
            return Ok();
        }
    }
}
