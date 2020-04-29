// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Identity.Clients.Abstractions;
using Identity.Clients.Data.Abstractions;
using Identity.Clients.Extensions;
using Identity.Clients.Models;
using Microsoft.Extensions.Logging;

namespace Identity.Clients.Services
{
    public class GrantService
    {
        private readonly IGrantStore _store;
        private readonly ILogger _logger;
        private readonly IMapper Mapper;

        public GrantService
        (
            IGrantStore store,
            ILogger<GrantService> logger,
            IMapper mapper
        ) {
            _logger = logger;
            _store = store;
            Mapper = mapper;
        }

        public async Task<PersistedGrant[]> GetAllAsync(string subjectId)
        {
            return Mapper.Map<PersistedGrant[]>(await _store.List(subjectId));
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            return Mapper.Map<PersistedGrant>(await _store.Load(key));
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            await _store.Clear(subjectId, clientId);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            await _store.Clear(subjectId, clientId, type);
        }

        public async Task RemoveAsync(string key)
        {
            await _store.Delete(key);
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            var entity = Mapper.Map<Data.PersistedGrant>(grant);
            await _store.Save(entity);
        }
    }
}
