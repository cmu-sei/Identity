// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using Identity.Clients.Data.Abstractions;
using Identity.Clients.Data;
using Identity.Clients.Data.EntityFrameworkCore.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace Identity.Clients.Data.EntityFrameworkCore
{
    public class ResourceClaimStore : DataStore<ResourceClaim>, IResourceClaimStore
    {
        public ResourceClaimStore(
            ClientDbContext dbContext,
            IDistributedCache cache = null
        ) : base (dbContext, cache)
        {

        }
    }
}
