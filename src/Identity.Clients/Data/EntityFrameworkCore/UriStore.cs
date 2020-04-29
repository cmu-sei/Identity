// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using Identity.Clients.Data.Abstractions;
using Identity.Clients.Data;
using Identity.Clients.Data.EntityFrameworkCore.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace Identity.Clients.Data.EntityFrameworkCore
{
    public class UriStore : DataStore<ClientUri>, IUriStore
    {
        public UriStore(
            ClientDbContext dbContext,
            IDistributedCache cache = null
        ) : base (dbContext, cache)
        {

        }

        // public override async Task<bool> CanManage(int id, string subjectId)
        // {
        //     var entity = await Load(id);
        //     return entity != null && await base.CanManage(entity.ClientId, subjectId);
        // }
    }
}
