// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System.Threading.Tasks;
using Identity.Clients.Models;
using Identity.Clients.Services;
using IdentityServer.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Api
{
    [Authorize]
    public class ResourceController : _Controller
    {
        public ResourceController (
            ILogger<ResourceController> logger,
            ResourceService svc,
            ImportService importSvc,
            IHostEnvironment env,
            BrandingOptions branding
        ) : base(logger)
        {
            _svc = svc;
            _importSvc = importSvc;
            _env = env;
            _branding = branding;
        }

        private readonly ResourceService _svc;
        private readonly ImportService _importSvc;
        private readonly IHostEnvironment _env;
        private readonly BrandingOptions _branding;

        [HttpGet("api/resources")]
        [ProducesResponseType(typeof(Resource[]), 200)]
        public async Task<IActionResult> List([FromQuery]SearchModel search)
        {
            return Json(await _svc.List(search));
        }

        [HttpGet("api/resource/{id}")]
        [ProducesResponseType(typeof(Resource), 200)]
        public async Task<IActionResult> Load([FromRoute]int id)
        {
            return Json(await _svc.Load(id));
        }

        [HttpPost("api/resource")]
        [ProducesResponseType(typeof(Resource), 200)]
        public async Task<IActionResult> Add([FromBody]NewResource model)
        {
            var resource = await _svc.Add(model);
            Audit(AuditId.ResourceState);
            return Json(resource);
        }

        [HttpPut("api/resource")]
        [ProducesResponseType(typeof(Resource), 200)]
        public async Task<IActionResult> Update([FromBody]Resource model)
        {
            var resource = await _svc.Update(model);
            Audit(AuditId.ResourceState);
            return Json(resource);
        }

        [HttpDelete("api/resource/{id}")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            await _svc.Delete(id);
            Audit(AuditId.ResourceDelete);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("api/resource/devimport")]
        public async Task<IActionResult> Import([FromBody] ResourceImport model)
        {
            if (!_env.IsDevelopment())
                return Forbid();

            await _importSvc.Import(model);
            return Ok();
        }

        [HttpPut("api/resource/{id}/secret")]
        [ProducesResponseType(typeof(ApiSecret), 200)]
        public async Task<IActionResult> GenerateSecret([FromRoute]int id)
        {
            return Json(await _svc.AddSecret(id));
        }

        [HttpPut("api/resource/{id}/code")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> NewEnlistCode([FromRoute]int id)
        {
            string code = await _svc.NewEnlistCode(id);
            string url = $"{_branding.UiHost}/resource/enlist/{code}";
            return Json(new { Url = url });
        }

        [HttpPost("api/resource/enlist/{code}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Enlist([FromRoute]string code)
        {
            await _svc.Enlist(code);
            Audit(AuditId.AcceptInvite);
            return Ok();
        }
    }
}
