// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using IdentityServer.Options;
using IdentityServer.Models;

namespace IdentityServer.Features.Grants
{
    [SecurityHeaders]
    [Authorize(AuthenticationSchemes = IdentityServer4.IdentityServerConstants.DefaultCookieAuthenticationScheme)]
    [AutoValidateAntiforgeryToken]
    public class GrantsController : _Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clients;
        private readonly IResourceStore _resources;

        public GrantsController(
            BrandingOptions branding,
            ILogger<GrantsController> logger,
            IIdentityServerInteractionService interaction,
            IClientStore clients,
            IResourceStore resources
        ) : base(branding, logger)
        {
            _interaction = interaction;
            _clients = clients;
            _resources = resources;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View("Index", await GetGrantsView());
        }

        [HttpPost]
        public async Task<IActionResult> Revoke(string clientId)
        {
            await _interaction.RevokeUserConsentAsync(clientId);
            return RedirectToAction("Index");
        }

        private async Task<List<GrantViewModel>> GetGrantsView()
        {
            var grants = await _interaction.GetAllUserGrantsAsync();

            var list = new List<GrantViewModel>();
            foreach(var grant in grants)
            {
                var client = await _clients.FindClientByIdAsync(grant.ClientId);
                if (client != null)
                {
                    var resources = await _resources.FindResourcesByScopeAsync(grant.Scopes);

                    var item = new GrantViewModel()
                    {
                        ClientId = grant.ClientId,
                        ClientName = client.ClientName ?? client.ClientId,
                        Created = grant.CreationTime,
                        Expires = grant.Expiration,
                        IdentityGrantNames = resources.IdentityResources.Select(x => x.DisplayName ?? x.Name).ToArray(),
                        ApiGrantNames = resources.ApiResources.Select(x => x.DisplayName ?? x.Name).ToArray()
                    };

                    list.Add(item);
                }
            }

            return list;
        }
    }
}
