// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Linq;
using System.Threading.Tasks;
using Identity.Clients.Data.Abstractions;
using Identity.Clients.Data.EntityFrameworkCore.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Identity.Clients.Data.EntityFrameworkCore
{
    public class ClientStore : DataStore<Client>, IClientStore
    {
        public ClientStore(
            ClientDbContext dbContext,
            IDistributedCache cache = null
        ) : base (dbContext, cache)
        {
            DbContext = dbContext;
        }

        private new ClientDbContext DbContext { get; }
        public override IQueryable<Client> List()
        {
             return base.List()
                .Include(c => c.Urls);
        }

        public IQueryable<Client> List(string term)
        {
            var q = List();
            if (!string.IsNullOrWhiteSpace(term))
            {
                q = DbContext.Database.IsNpgsql()
                    ? q.Where(c =>
                        EF.Functions.ILike(c.Name, $"%{term}%") ||
                        EF.Functions.ILike(c.DisplayName, $"%{term}%")
                    )
                    : q.Where(c =>
                        EF.Functions.Like(c.Name, $"%{term}%") ||
                        EF.Functions.Like(c.DisplayName, $"%{term}%")
                    );
            }
            return q;
        }

        public override async Task<Client> Load(int id)
        {
            if (ScopedCache.ContainsKey(id))
                return ScopedCache[id];

            var client = await FromCache(id);
            if (client == null)
            {
                client = await DbContext.Clients
                .Include(c => c.Claims)
                .Include(c => c.Secrets)
                .Include(c => c.Events)
                .Include(c => c.EventHandlers).ThenInclude(e => e.ClientEvent).ThenInclude(e => e.Client)
                .Include(c => c.Managers)
                .Include(c => c.Urls)
                .Where(c => c.Id == id)
                .SingleOrDefaultAsync();

                await ToCache(client);

            }
            else
            {
                DbContext.Clients.Attach(client);
            }

            ScopedCache.Add(id, client);

            return client;
        }

        public async Task<Client> Load(string key)
        {
            var client = ScopedCache.Values
                .Where(c => c.GlobalId == key || c.Name == key)
                .SingleOrDefault();

            if (client is Client)
                return client;

            var idmap = await FromCache($"{typeof(CachedId).FullName}:{key}") as CachedId;
            if (idmap?.Id > 0)
                return await Load(idmap.Id);

            int id = await DbContext.Clients
                .Where(c => c.GlobalId == key || c.Name == key)
                .Select(c => c.Id)
                .SingleOrDefaultAsync();

            return await Load(id);
        }

        protected override async Task ToCache(Client entity)
        {
            if (entity == null)
                return;

            await ToCache(
                $"{typeof(CachedId).FullName}:{entity.Name}",
                new CachedId { Id = entity.Id }
            );

            await base.ToCache(entity);
        }

        public async Task<Client> LoadByEnlistCode(string code)
        {
            return await DbContext.Clients
                .Include(c => c.Managers)
                .Where(c => c.EnlistCode == code)
                .SingleOrDefaultAsync();
        }

        public override async Task<Client> Add(Client client)
        {
            if (string.IsNullOrEmpty(client.GlobalId))
                client.GlobalId = Guid.NewGuid().ToString();

            if (string.IsNullOrEmpty(client.Grants))
                client.Grants = "client_credentials";

            await base.Add(client);

            return client;
        }

        public async Task<bool> CanManage(int id, string subjectId)
        {
            return await DbContext.Clients
                .Where(c => c.Id == id && (c.GlobalId == subjectId || c.Managers.Any(m => m.SubjectId == subjectId)))
                .AnyAsync();
        }

        public async Task<bool> CanManage(string globalId, string subjectId)
        {
            return await DbContext.Clients
                .Where(c => c.GlobalId == globalId && (c.GlobalId == subjectId || c.Managers.Any(m => m.SubjectId == subjectId)))
                .AnyAsync();
        }

    }
}
