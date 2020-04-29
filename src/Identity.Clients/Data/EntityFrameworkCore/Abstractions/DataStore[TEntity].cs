// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;
using System.Linq;
using System.Threading.Tasks;
using Identity.Clients.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Identity.Clients.Data.EntityFrameworkCore.Abstractions
{
    public abstract class DataStore<TEntity> : IDataStore<TEntity>
        where TEntity : class, IEntity
    {
        public DataStore(
            DbContext dbContext,
            IDistributedCache cache = null
        ){
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

        public virtual IQueryable<TEntity> List()
        {
            return DbContext.Set<TEntity>().AsNoTracking();
        }

        public virtual async Task<TEntity> Load(int id)
        {
            if (ScopedCache.ContainsKey(id))
                return ScopedCache[id];

            var entity = await FromCache(id);
            if (entity == null) {
                entity = await DbContext.Set<TEntity>().FindAsync(id);
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
            await UnCache(entity); // uncache resource list
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
            var entity = await DbContext.Set<TEntity>().FindAsync(id);

            if (entity != null)
            {
                DbContext.Set<TEntity>().Remove(entity);
                await DbContext.SaveChangesAsync();
                await UnCache(entity);
            }
        }

        public async Task<bool> Exists(int id)
        {
            return await DbContext.Set<TEntity>().AnyAsync(p => p.Id == id);
        }

        protected virtual async Task<TEntity> FromCache(int id)
        {
            string key = $"{typeof(TEntity).FullName}:{id}";
            return await FromCache(key) as TEntity;
        }

        protected virtual async Task ToCache(TEntity entity)
        {
            if (!(entity is IEntityClientProperty))
            {
                string key = $"{entity.GetType().FullName}:{entity.Id}";
                await ToCache(key, entity);
            }
        }

        protected virtual async Task UnCache(TEntity entity)
        {
            string key = (entity is IEntityClientProperty)
                ? $"{typeof(Data.Client).FullName}:{((IEntityClientProperty)entity).ClientId}"
                : (entity is ResourceClaim)
                    ? "Identity.Clients.Data.Resource[]:all"
                    : $"{entity.GetType().FullName}:{entity.Id}";

            await UnCache(key);

            if (entity is Resource)
                await UnCache("Identity.Clients.Data.Resource[]:all");
        }

        protected async Task UnCache(Type t, int id)
        {
            await UnCache($"{t.FullName}:{id}");
        }

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
                        return JsonConvert.DeserializeObject(v, Type.GetType(type));
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
    }
}
