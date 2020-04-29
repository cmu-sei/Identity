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

        Identity.Clients.Models.Resource[] _all;

        private Identity.Clients.Models.Resource[] GetAll()
        {
            if (_all != null)
                return _all;

            _all = _svc.LoadAll().Result;
            return _all;
        }

        public async Task<ApiResource> FindApiResourceAsync(string name)
        {
            await Task.Delay(0);
            return GetAll()
                .Where(r => r.Type == ResourceType.Api && r.Name == name)
                .Select(r => ConvertApiResource(r))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            await Task.Delay(0);
            List<ApiResource> result = new List<ApiResource>();
            var all = GetAll();
            var map = all.Where(r => r.Type == ResourceType.Api).SelectMany(r => r.Claims).ToDictionary(c => c.Type);
            foreach(string scope in scopeNames)
            {
                if (map.ContainsKey(scope))
                    result.Add(ConvertApiResource(all.Where(r => r.Id == map[scope].ResourceId).First()));
            }
            return result;
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
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
                    .Select(r => ConvertApiResource(r)).ToArray()
            );
        }

        private ApiResource ConvertApiResource(Identity.Clients.Models.Resource resource)
        {
            if (resource == null)
                return null;

            return new ApiResource
                {
                    Enabled = resource.Enabled,
                    Name = resource.Name,
                    DisplayName = resource.DisplayName,
                    Description = resource.Description,
                    Scopes = resource.Claims.Select(c => new Scope{
                        Name = c.Type,
                        DisplayName = resource.DisplayName,
                        Required = resource.Required,
                        Emphasize = resource.Emphasize,
                        ShowInDiscoveryDocument = resource.ShowInDiscoveryDocument
                    }).ToArray()
                };
        }

        private IdentityResource ConvertIdentityResource(Identity.Clients.Models.Resource resource)
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
                    UserClaims = resource.Claims.Select(c => c.Type).ToArray()
                };
        }

    }
}
