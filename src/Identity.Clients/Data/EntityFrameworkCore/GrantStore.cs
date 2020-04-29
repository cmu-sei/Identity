// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System.Linq;
using System.Threading.Tasks;
using Identity.Clients.Data.Abstractions;
using Identity.Clients.Data;
using Identity.Clients.Data.EntityFrameworkCore.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Identity.Clients.Data.EntityFrameworkCore
{
    public class GrantStore : IGrantStore
    {
        public GrantStore(
            ClientDbContext dbContext
        )
        {
            _db = dbContext;
        }

        private readonly ClientDbContext _db;

        public async Task Clear(string subjectId, string clientId)
        {
            var list = await _db.PersistedGrants
                .Where(g => g.SubjectId == subjectId && g.ClientId == clientId)
                .ToArrayAsync();

            _db.RemoveRange(list);
            await _db.SaveChangesAsync();
        }

        public async Task Clear(string subjectId, string clientId, string type)
        {
            var list = await _db.PersistedGrants
                .Where(g => g.SubjectId == subjectId && g.ClientId == clientId && g.Type == type)
                .ToArrayAsync();

            _db.RemoveRange(list);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(string key)
        {
            var grant = await _db.PersistedGrants.FindAsync(key);
            if (grant != null)
            {
                _db.Remove(grant);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<PersistedGrant[]> List(string subjectId)
        {
            return await _db.PersistedGrants
                .Where(g => g.SubjectId == subjectId)
                .ToArrayAsync();
        }

        public async Task<PersistedGrant> Load(string key)
        {
            return await _db.PersistedGrants.FindAsync(key);
        }

        public async Task Save(PersistedGrant grant)
        {
            var entity = await _db.PersistedGrants.FindAsync(grant.Key);

            if (entity != null)
            {
                entity.Data = grant.Data;
                entity.CreationTime = grant.CreationTime;
                entity.Expiration = grant.Expiration;
                _db.Update(entity);
            }
            else
                _db.Add(grant);

            await _db.SaveChangesAsync();
        }
    }
}
