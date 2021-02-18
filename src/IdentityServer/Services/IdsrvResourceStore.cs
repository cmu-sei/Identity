// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System.Threading.Tasks;
using IdentityServer4.Stores;
using IdentityServer4.Models;
using System.Collections.Generic;
using Identity.Clients.Services;
using System.Linq;
using Identity.Clients.Abstractions;

namespace IdentityServer.Services
{
    public class IdsrvResourceStore : IdentityServer4.Stores.IResourceStore
    {
        public IdsrvResourceStore(
            ResourceService svc
        ){
            _svc = svc;
        }
        private readonly ResourceService _svc;

        Identity.Clients.Models.ResourceDetail[] _all;

        private Identity.Clients.Models.ResourceDetail[] GetAll()
        {
            if (_all != null)
                return _all;

            _all = _svc.LoadAll().Result;
            return _all;
        }

         public async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            await Task.Delay(0);
            return GetAll()
                .Where(r => r.Type == ResourceType.Api && apiResourceNames.Contains(r.Name))
                .Select(r => ConvertApiResource(r));
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            await Task.Delay(0);
            return GetAll()
                .Where(r => r.Type == ResourceType.Api && r.Scopes.Split(' ').Any(s => scopeNames.Contains(s)))
                .Select(r => ConvertApiResource(r));
        }

        async Task<IEnumerable<ApiScope>> IResourceStore.FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            await Task.Delay(0);
            return GetAll()
                .Where(r => r.Type == ResourceType.Api)
                .SelectMany(r => r.Scopes.Split(' '))
                .Distinct()
                .Where(r => scopeNames.Contains(r))
                .Select(r => new ApiScope(r));
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            await Task.Delay(0);
            List<IdentityResource> result = new List<IdentityResource>();
            var all = GetAll();
            foreach (var r in all.Where(r => r.Type == ResourceType.Identity))
                if (scopeNames.Contains(r.Name))
                    result.Add(ConvertIdentityResource(r));
            return result;
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            await Task.Delay(0);
            return new Resources(
                GetAll()
                    .Where(r => r.Type == ResourceType.Identity)
                    .Select(r => ConvertIdentityResource(r)).ToArray(),
                GetAll()
                    .Where(r => r.Type == ResourceType.Api)
                    .Select(r => ConvertApiResource(r)).ToArray(),
                GetAll()
                    .Where(r => r.Type == ResourceType.Api)
                    .SelectMany(r => r.Scopes.Split(' '))
                    .Distinct()
                    .Select(r => new ApiScope(r)).ToArray()
            );
        }

        private ApiResource ConvertApiResource(Identity.Clients.Models.ResourceDetail resource)
        {
            if (resource == null)
                return null;

            return new ApiResource
                {
                    Enabled = resource.Enabled,
                    Name = resource.Name,
                    DisplayName = resource.DisplayName,
                    Description = resource.Description,
                    Scopes = resource.Scopes.Split(' '),
                    ApiSecrets = resource.Secrets.Select(s => new Secret(s.Value)).ToArray(),
                    UserClaims = resource.UserClaims?.Split(' ') ?? new string[]{}
                };
        }

        private IdentityResource ConvertIdentityResource(Identity.Clients.Models.ResourceDetail resource)
        {
            if (resource == null)
                return null;

            return new IdentityResource
                {
                    Enabled = resource.Enabled,
                    Name = resource.Name,
                    DisplayName = resource.DisplayName,
                    Description = resource.Description,
                    Emphasize = resource.Emphasize,
                    Required = resource.Required,
                    ShowInDiscoveryDocument = resource.ShowInDiscoveryDocument,
                    UserClaims = resource.UserClaims?.Split(' ') ?? new string[]{}
                };
        }

        private ApiScope ConvertApiScope(Identity.Clients.Models.ResourceDetail resource)
        {
            if (resource == null)
                return null;

            return new ApiScope
                {
                    Enabled = resource.Enabled,
                    Name = resource.Name,
                    DisplayName = resource.DisplayName,
                    Description = resource.Description,
                    Emphasize = resource.Emphasize,
                    Required = resource.Required,
                    ShowInDiscoveryDocument = resource.ShowInDiscoveryDocument,
                    UserClaims = resource.UserClaims?.Split(' ') ?? new string[]{}
                };
        }
    }
}
