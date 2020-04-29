// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System.Threading.Tasks;
using Identity.Clients.Services;
using IdentityServer.Options;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Features.Consent
{
    [SecurityHeaders]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class ConsentController : _Controller
    {
        private readonly ConsentViewService _viewSvc;

        public ConsentController(
            BrandingOptions branding,
            ILogger<ConsentController> logger,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            ResourceService resourceStore
        ) : base(branding, logger)
        {
            _viewSvc = new ConsentViewService(interaction, clientStore, resourceStore, logger);
        }

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var vm = await _viewSvc.GetConsentView(returnUrl);
            if (vm != null)
            {
                return View("Index", vm);
            }

            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> Index(IdentityServer.Models.ConsentModel model)
        {
            await _viewSvc.ProcessConsent(model);
            return Redirect(model.ReturnUrl);
        }
    }
}
