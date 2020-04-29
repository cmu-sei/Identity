// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System.Linq;
using System.Threading.Tasks;
using Identity.Clients.Data.Abstractions;
using Identity.Clients.Data;
using Identity.Clients.Data.EntityFrameworkCore.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Identity.Clients.Data.EntityFrameworkCore
{
    public class ManagerStore : DataStore<ClientManager>, IManagerStore
    {
        public ManagerStore(
            ClientDbContext dbContext,
            IDistributedCache cache = null
        ) : base (dbContext, cache)
        {

        }

        // public async Task<Client> LoadClientByEnlistCode(string code)
        // {
        //     return await DbContext.Clients
        //         .Include(c => c.Managers)
        //         .Where(c => c.EnlistCode == code)
        //         .SingleOrDefaultAsync();
        // }
    }
}
