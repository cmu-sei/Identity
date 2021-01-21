// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Clients.Services;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer.Services
{
    public class IdsrvGrantStore : IPersistedGrantStore
    {
        public IdsrvGrantStore(
            GrantService svc
        )
        {
            _svc = svc;
        }

        private readonly GrantService _svc;

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            var grants = await _svc.GetAllAsync(filter.SubjectId);
            return grants.Select(g => Map(g)).ToArray();
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            var grant = await _svc.GetAsync(key);
            return Map(grant);
        }

        public async Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            await _svc.RemoveAllAsync(filter.SubjectId, filter.ClientId);
        }

        public async Task RemoveAllAsync(PersistedGrantFilter filter, string type)
        {
            await _svc.RemoveAllAsync(filter.SubjectId, filter.ClientId, type);
        }

        public async Task RemoveAsync(string key)
        {
            await _svc.RemoveAsync(key);
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            await _svc.StoreAsync(Map(grant));
        }

        private PersistedGrant Map(Identity.Clients.Models.PersistedGrant source)
        {
            return (source == null) ? null : new PersistedGrant
            {
                Key = source.Key,
                Type = source.Type,
                SubjectId = source.SubjectId,
                ClientId = source.ClientId,
                Expiration = source.Expiration,
                CreationTime = source.CreationTime,
                Data = source.Data
            };
        }
        private Identity.Clients.Models.PersistedGrant Map(PersistedGrant source)
        {
            return (source == null) ? null : new Identity.Clients.Models.PersistedGrant
            {
                Key = source.Key,
                Type = source.Type,
                SubjectId = source.SubjectId,
                ClientId = source.ClientId,
                Expiration = source.Expiration,
                CreationTime = source.CreationTime,
                Data = source.Data
            };
        }
    }
}