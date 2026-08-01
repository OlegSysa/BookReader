using BookReader.Core.Abstract.Services;
using BookReader.Core.Business;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BookReader.Infrastructure.Services
{
    public class RedisService : BaseService<RedisService>, ICacheService
    {
        private readonly IDatabase _database;
        public RedisService(IConnectionMultiplexer redis,
            IConfiguration config,
            ILogger<RedisService> logger): base(config, logger)
        {
            _database = redis.GetDatabase();
        }
        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);

            if (value.IsNullOrEmpty)
                return default;
            var json = value.ToString();
            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            var json = JsonSerializer.Serialize(value);

            await _database.StringSetAsync(key, json, expiration);
        }

        public Task RemoveAsync(string key)
        {
            return _database.KeyDeleteAsync(key);
        }
    }
}
