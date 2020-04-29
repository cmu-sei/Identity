// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using Identity.Accounts.Data.Abstractions;
using Identity.Accounts.Data;
using Identity.Accounts.Data.EntityFrameworkCore.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace Identity.Accounts.Data.EntityFrameworkCore
{
    public class PropertyStore: DataStore<AccountProperty>, IPropertyStore
    {
        public PropertyStore(
            AccountDbContext dbContext,
            IDistributedCache cache
        ) : base(dbContext, cache)
        {

        }
    }
}
