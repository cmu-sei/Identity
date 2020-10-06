// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Linq;
using System.Threading.Tasks;
using Identity.Accounts.Data.Abstractions;
using Identity.Accounts.Data.EntityFrameworkCore.Abstractions;
using Identity.Accounts.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Identity.Accounts.Data.EntityFrameworkCore
{
    public class AccountStore : DataStore<Account>, IAccountStore
    {
        public AccountStore (
            AccountDbContext dbContext,
            IDistributedCache cache
        ) : base(dbContext, cache)
        {
            DbContext = dbContext;
        }

        protected new AccountDbContext DbContext { get; }

        #region Account

        public override IQueryable<Account> List(string term = null)
        {
            var query = base.List()
                .Include(a => a.Properties);

            return query;
        }

        public async Task<Account> Load(int id)
        {
            return await base.Load(id, query => query
                .Include(u => u.Tokens)
                .Include(u => u.Properties)
            );
        }

        public async Task<Account> LoadByGuid(string guid)
        {
            var scoped = ScopedCache.Values.Where(a => a.GlobalId == guid).SingleOrDefault();
            if (scoped is Account)
                return scoped;

            var idmap = await FromCache($"{typeof(CachedId).FullName}:{guid}") as CachedId;
            if (idmap?.Id > 0)
                return await Load(idmap.Id);

            int id = await DbContext.Accounts
                .Where(u => u.GlobalId == guid)
                .Select(u => u.Id)
                .SingleOrDefaultAsync();

            return await Load(id);
        }

        /// <summary>
        /// Loads an account from the username or certificate key.
        /// Migrates from SHA1 hashing to SHA256.
        /// </summary>
        /// <param name="key">Username/Email or certificate key</param>
        /// <returns></returns>
        public async Task<Account> LoadByToken(string key)
        {
            string sha2 = key.ToSha256();

            var account = await LoadByHash(sha2);

            if (account == null)
            {
                string sha1 = key.ToSha1();

                account = await LoadByHash(sha1);

                if (account != null)
                {
                    var token = account.Tokens.First(t => t.Hash == sha1);

                    account.Tokens.Add(new AccountToken
                    {
                        Hash = sha2,
                        Type = token.Type,
                        WhenCreated = DateTime.UtcNow
                    });

                    account.Tokens.Remove(token);

                    await DbContext.SaveChangesAsync();
                }
            }

            return account;
        }
        private async Task<Account> LoadByHash(string hash)
        {
            var scoped = ScopedCache.Values
                .Where(a => a.Tokens.Any(t => t.Hash == hash))
                .SingleOrDefault();

            if (scoped is Account)
                return scoped;

            int id = await DbContext.Accounts
                .Where(a => a.Tokens.Any(t => t.Hash == hash))
                .Select(a => a.Id)
                .SingleOrDefaultAsync();

            return await Load(id);
        }

        protected override async Task ToCache(Account entity)
        {
            if (entity == null)
                return;

            await ToCache(
                $"{typeof(CachedId).FullName}:{entity.GlobalId}",
                new CachedId { Id = entity.Id }
            );

            await base.ToCache(entity);
        }

        public async Task<bool> IsTokenUnique(string hash)
        {
            string sha2 = hash.ToSha256();
            string sha1 = hash.ToSha1();
            return !(await DbContext.AccountTokens.AnyAsync(t => t.Hash == sha2 || t.Hash == sha1));
        }

        public async Task<Models.AccountStats> GetStats(DateTime since)
        {
            return new Models.AccountStats
            {
                Since = since,
                AccountsCreated = await DbContext.Accounts.CountAsync(
                    a => a.WhenCreated > since
                ),
                AccountsAuthed = await DbContext.Accounts.CountAsync(
                    a => a.WhenAuthenticated > since
                )
            };
        }

        public async Task FixUsernames()
        {
            var accounts = await DbContext.Accounts
                .Include(a => a.Properties)
                .Where(a => !a.Properties.Any(p => p.Key == ClaimTypes.Username))
                .ToListAsync();

            foreach (var account in accounts)
            {
                string name = account.Properties.FirstOrDefault(p => p.Key == ClaimTypes.Name)?.Value;

                if (string.IsNullOrWhiteSpace(name))
                    continue;

                account.Properties.Add(new AccountProperty
                {
                    Key = ClaimTypes.Username,
                    Value = $"{name.ToAccountSlug()}.{account.Id.ToString("x4")}"
                });
            }
            await DbContext.SaveChangesAsync();
        }

        #endregion

        #region AccountCode

        public async Task<AccountCode> GetAccountCode(string key)
        {
            var code = await DbContext.AccountCodes.FindAsync(key.ToSha256());

            if (code == null)
                code= await DbContext.AccountCodes.FindAsync(key.ToSha1());

            return code;
        }

        public async Task Save(AccountCode token)
        {
            var existing = await DbContext.AccountCodes.FindAsync(token.Hash);

            if (existing != null)
            {
                existing.Code = token.Code;
                existing.WhenCreated = token.WhenCreated;
                DbContext.AccountCodes.Update(existing);
            }
            else
            {
                DbContext.AccountCodes.Add(token);
            }

            await DbContext.SaveChangesAsync();
        }

        public async Task Delete(AccountCode token)
        {
            DbContext.AccountCodes.Remove(token);
            await DbContext.SaveChangesAsync();
        }

        #endregion

        #region OverrideCode

        public async Task<OverrideCode> GetOverrideCode(string code)
        {
            return await DbContext.OverrideCodes.Where(o => o.Code == code).FirstOrDefaultAsync();
        }

        #endregion

        // #region AccountToken

        // public async Task Add(AccountToken token)
        // {
        //     token.WhenCreated = DateTime.UtcNow;
        //     DbContext.AccountTokens.Add(token);
        //     await DbContext.SaveChangesAsync();
        // }

        // public async Task Delete(AccountToken token)
        // {
        //     DbContext.AccountTokens.Remove(token);
        //     await DbContext.SaveChangesAsync();
        // }

        // #endregion
    }
}
