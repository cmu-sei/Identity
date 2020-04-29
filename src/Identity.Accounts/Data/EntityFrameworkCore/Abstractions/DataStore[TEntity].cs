// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Accounts.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Identity.Accounts.Data.EntityFrameworkCore.Abstractions
{
    public abstract class DataStore<TEntity> : IDataStore<TEntity>
        where TEntity : class, IEntity
    {
        public DataStore(
            DbContext dbContext,
            IDistributedCache cache = null
        )
        {
            DbContext = dbContext;
            _cache = cache;

            _serializeSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            _cacheOptions = new DistributedCacheEntryOptions
            {
                SlidingExpiration = new TimeSpan(0, 15, 0)
            };

        }

        public DbContext DbContext { get; }
        protected IDistributedCache _cache { get; }
        protected Dictionary<int, TEntity> ScopedCache { get; } = new Dictionary<int, TEntity>();
        protected JsonSerializerSettings _serializeSettings { get; }
        protected DistributedCacheEntryOptions _cacheOptions { get; }

        public virtual IQueryable<TEntity> List(string term = null)
        {
            return DbContext.Set<TEntity>().AsNoTracking();
        }

        public virtual async Task<TEntity> Load(int id, Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null)
        {
            if (id == 0)
                return null;

            if (ScopedCache.ContainsKey(id))
                return ScopedCache[id];

            var entity = await FromCache(id);

            if (entity == null) {

                IQueryable<TEntity> query = DbContext.Set<TEntity>();

                if (includes != null)
                    query = includes(query);

                entity = await query.Where(e => e.Id == id).SingleOrDefaultAsync();

                await ToCache(entity);

            } else {

                DbContext.Set<TEntity>().Attach(entity);

            }

            ScopedCache.Add(id, entity);

            return entity;
        }

        public virtual async Task<TEntity> Add(TEntity entity)
        {
            DbContext.Add(entity);
            await DbContext.SaveChangesAsync();
            return await Load(entity.Id);
        }

        public virtual async Task Update(TEntity entity)
        {
            DbContext.Update(entity);
            await DbContext.SaveChangesAsync();
            await UnCache(entity);
        }

        public virtual async Task Delete(int id)
        {
            var entity = await Load(id);

            if (entity != null)
            {
                DbContext.Set<TEntity>().Remove(entity);
                await DbContext.SaveChangesAsync();
                await UnCache(entity);
            }
        }

        public async Task<bool> Exists(int id)
        {
            var entity = await Load(id);
            return entity is TEntity;
        }

        #region Entity Cache Helpers
        protected virtual async Task<TEntity> FromCache(int id)
        {
            string key = $"{typeof(TEntity).FullName}:{id}";
            return await FromCache(key) as TEntity;
        }

        protected virtual async Task ToCache(TEntity entity)
        {
            if (entity == null)
                return;

            string key = $"{entity.GetType().FullName}:{entity.Id}";

            await ToCache(key, entity);

        }

        protected virtual async Task UnCache(TEntity entity)
        {
            if (entity == null)
                return;

            string key = $"{entity.GetType().FullName}:{entity.Id}";

            await UnCache(key);
        }
        #endregion

        #region Distributed Cache methods
        protected async Task<object> FromCache(string key)
        {
            if (_cache != null)
            {
                string v = await _cache.GetStringAsync(key);
                if (!String.IsNullOrEmpty(v))
                {
                    try
                    {
                        string type = key.Substring(0, key.IndexOf(':'));
                        Type t = Type.GetType(type);
                        var obj = JsonConvert.DeserializeObject(v, t);
                        return obj;
                    }
                    catch
                    {
                    }
                }
            }
            return null;
        }

        protected async Task ToCache(string key, object value, DistributedCacheEntryOptions options = null)
        {
            if (_cache != null)
            {
                try {
                    string v = JsonConvert.SerializeObject(value, _serializeSettings);
                    await _cache.SetStringAsync(key, v, options ?? _cacheOptions);
                } catch
                {
                }

            }
        }

        protected async Task UnCache(string key)
        {
            if (_cache != null)
                await _cache.RemoveAsync(key);
        }

        #endregion
    }
}
