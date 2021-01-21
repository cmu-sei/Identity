// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System.Linq;
using System.Threading.Tasks;
using Identity.Clients.Abstractions;
using Identity.Clients.Services;
using IdentityServer.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using static IdentityServer4.IdentityServerConstants;

namespace IdentityServer.Features.Consent
{
    public class ConsentViewService : Mvc.Features.IFeatureService
    {
        private readonly IClientStore _clientStore;
        private readonly ResourceService _resourceStore;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly ILogger _logger;

        public ConsentViewService(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            ResourceService resourceStore,
            ILogger<ConsentController> logger
        )
        {
            _interaction = interaction;
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _logger = logger;
        }

        public async Task ProcessConsent(Models.ConsentModel model)
        {
            var request = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            if (request == null)
            {
                _logger.LogError("No consent request matching request: {0}", model.ReturnUrl);
                return;
            }

            var consent = new IdentityServer4.Models.ConsentResponse { Error = IdentityServer4.Models.AuthorizationError.AccessDenied };
            if (model.Action == "consent")
            {
                consent = new IdentityServer4.Models.ConsentResponse();
                consent.RememberConsent = model.RememberConsent;
                consent.ScopesValuesConsented = ((!model.AllowOfflineAccess)
                    ? request.ValidatedResources.RawScopeValues.Where(x => x != StandardScopes.OfflineAccess)
                    : request.ValidatedResources.RawScopeValues).ToArray();
            }

            await _interaction.GrantConsentAsync(request, consent);
        }

        public async Task<ConsentViewModel> GetConsentView(string returnUrl)
        {
            var request = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (request == null)
            {
                _logger.LogError("No consent request matching request: {0}", returnUrl);
                return null;
            }

            var client = await _clientStore.FindEnabledClientByIdAsync(request.Client.ClientId);
            if (client == null)
            {
                _logger.LogError("Invalid client id: {0}", request.Client.ClientId);
                return null;
            }

            var resources = await _resourceStore.LoadAll();
            var vm = new ConsentViewModel();
            vm.ReturnUrl = returnUrl;

            vm.RequestOfflineAccess = request.ValidatedResources.RawScopeValues.Contains(StandardScopes.OfflineAccess);

            vm.AllowRememberConsent = client.AllowRememberConsent && !vm.RequestOfflineAccess;

            vm.ClientName = client.ClientName ?? client.ClientId;
            vm.ClientUrl = client.ClientUri;
            vm.ClientLogoUrl = client.LogoUri;


            vm.IdentityScopes = resources.Where(r =>
                r.Type == ResourceType.Identity &&
                request.ValidatedResources.RawScopeValues.Contains(r.Name)).ToArray();

            vm.ResourceScopes = resources.Where(r =>
                r.Type == ResourceType.Api &&
                request.ValidatedResources.RawScopeValues.Contains(r.Name)).ToArray();

            if (vm.IdentityScopes.Length == 0 && vm.ResourceScopes.Length == 0)
                return null;

            return vm;
        }

    }
}
