// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer.Options;

namespace IdentityServer.Services
{
    public class IdsrvProfileClaimsCache
    {
        public IdsrvProfileClaimsCache(
            int cacheExpirationSeconds
        ) {
            _cache = new ConcurrentDictionary<string, CacheItem>();
            _cacheExpirationSeconds = cacheExpirationSeconds;
        }

        private ConcurrentDictionary<string, CacheItem> _cache;
        private int _cacheExpirationSeconds;

        public bool Exists(string id)
        {
            return _cache.ContainsKey(id);
        }

        public List<Claim> Find(string id)
        {
            _cache.TryGetValue(id, out CacheItem item);
            if (item == null)
                return null;


            if (IsExpired(item))
            {
                Remove(id);
                return null;
            }

            return item.Claims;
        }

        public void Add(string id, List<Claim> claims)
        {
            var item = new CacheItem {
                Timestamp = DateTime.UtcNow,
                Claims = claims
            };

            _cache.TryAdd(id, item);
        }

        public void Remove(string id)
        {
            _cache.TryRemove(id, out CacheItem item);
        }

        private bool IsExpired(CacheItem item)
        {
            return DateTime.UtcNow.CompareTo(item.Timestamp.AddSeconds(_cacheExpirationSeconds)) > 0;
        }
    }

    public class CacheItem
    {
        public DateTime Timestamp { get; set; }
        public List<Claim> Claims { get; set; }
    }
}