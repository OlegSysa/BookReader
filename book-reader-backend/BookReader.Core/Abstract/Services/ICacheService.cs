using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Abstract.Services
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);

        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task SetBatchAsync<T>(Dictionary<string, T> dict, TimeSpan? expiration = null);

        Task RemoveAsync(string key);
    }
}
