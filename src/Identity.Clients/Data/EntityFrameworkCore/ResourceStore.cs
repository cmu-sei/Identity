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
    public class ResourceStore : DataStore<Resource>, IResourceStore
    {
        public ResourceStore(
            ClientDbContext dbContext,
            IDistributedCache cache = null
        ) : base (dbContext, cache)
        {
            DbContext = dbContext;
        }

        private new ClientDbContext DbContext { get; }
        public async Task<Resource[]> GetAll()
        {
            var result = await FromCache("Identity.Clients.Data.Resource[]:all") as Resource[];
            if (result == null)
            {
                result = await base.List()
                    .Include(c => c.Secrets)
                    .Include(r => r.Managers)
                    .ToArrayAsync();

                await ToCache("Identity.Clients.Data.Resource[]:all", result);
            }
            return result;
        }

        public async Task<Resource> LoadByEnlistCode(string code)
        {
            return await DbContext.Resources
                .Include(c => c.Secrets)
                .Include(c => c.Managers)
                .Where(c => c.EnlistCode == code)
                .SingleOrDefaultAsync();
        }

        public override async Task<Resource> Load(int id)
        {
            if (ScopedCache.ContainsKey(id))
                return ScopedCache[id];

            var resource = await FromCache(id);
            if (resource == null)
            {
                resource = await DbContext.Resources
                .Include(c => c.Secrets)
                .Include(c => c.Managers)
                .Where(c => c.Id == id)
                .SingleOrDefaultAsync();

                await ToCache(resource);
            }
            else
            {
                DbContext.Resources.Attach(resource);
            }

            ScopedCache.Add(id, resource);

            return resource;
        }

        public async Task<bool> CanManage(int id, string subjectId)
        {
            return await DbContext.Resources
                .Where(c => c.Id == id && (c.Managers.Any(m => m.SubjectId == subjectId)))
                .AnyAsync();
        }

    }
}
