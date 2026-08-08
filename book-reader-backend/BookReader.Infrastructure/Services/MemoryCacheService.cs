using BookReader.Core.Abstract.Services;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BookReader.Infrastructure.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public Task<T?> GetAsync<T>(string key)
        {
           if(!_memoryCache.TryGetValue(key, out T? value))
            {
                return Task.FromResult<T?>(default);
            }
            return Task.FromResult<T?>(value);
        }

        public async Task RemoveAsync(string key)
        {
           _memoryCache.Remove(key);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
           _memoryCache.Set(key, value, expiration ?? TimeSpan.FromMinutes(30));
        }

        public async Task SetBatchAsync<T>(Dictionary<string, T> dict, TimeSpan? expiration = null)
        {
            foreach (var item in dict)
            {
                _memoryCache.Set(item.Key, item.Value,
                    absoluteExpirationRelativeToNow: expiration ?? TimeSpan.FromMinutes(30));
            }
        }
    }
}
