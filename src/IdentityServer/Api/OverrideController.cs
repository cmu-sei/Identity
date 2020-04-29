// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System.Threading.Tasks;
using Identity.Accounts.Models;
using Identity.Accounts.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Api
{
    [Authorize(
        Policy = AppConstants.PrivilegedPolicy,
        Roles = AppConstants.AdminRole
    )]
    public class OverrideController : Controller
    {
        public OverrideController(
            IOverrideService svc
        )
        {
            _svc = svc;
        }

        private readonly IOverrideService _svc;

        [HttpGet("api/codes")]
        [ProducesResponseType(typeof(OverrideCode[]), 200)]
        public async Task<IActionResult> List()
        {
            return Json(await _svc.List());
        }

        [HttpPost("api/code")]
        [ProducesResponseType(typeof(OverrideCode), 200)]
        public async Task<IActionResult> Add([FromBody]NewOverrideCode model)
        {
            model.Description = $"Added by {User.Identity.Name}";
            return Json(await _svc.Add(model));
        }

        [HttpDelete("api/code/{id}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            await _svc.Delete(id);
            return Ok();
        }

    }
}
