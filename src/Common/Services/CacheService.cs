using Common.Factories;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Services
{
    public interface ICacheService
    {
        Task<T> GetSingleAsync<T>(string key);
        Task<IEnumerable<T>> GetListAsync<T>(string key);
        Task AddSingleAsync(string key, object value, DateTime? expiration = null);
        Task AddListAsync(string key, object value, DateTime? expiration = null);
        Task RemoveAsync(string key);
        Task SetExpirationAsync(string key, DateTime expiration);
    }

    public class CacheService : ICacheService
    {
        private readonly ILogger<CacheService> _logger;
        private readonly ICacheFactory _cacheFactory;

        public CacheService(
            ILogger<CacheService> logger,
            ICacheFactory cacheFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cacheFactory = cacheFactory ?? throw new ArgumentNullException(nameof(cacheFactory));
        }

        public async Task<T> GetSingleAsync<T>(string key)
        {
            _logger.LogDebug($"CACHE | GET KEY {key}");

            var value = await _cacheFactory.Database().StringGetAsync(key);

            _logger.LogDebug($"CACHE | GOT {value}");

            return value.IsNull ? default : JsonConvert.DeserializeObject<T>(value);
        }

        public async Task<IEnumerable<T>> GetListAsync<T>(string key)
        {
            _logger.LogDebug($"CACHE | LIST KEY {key}");

            var values = await _cacheFactory.Database().ListRangeAsync(key);

            _logger.LogDebug($"CACHE | GOT {values}");

            return Array.ConvertAll(values, value => JsonConvert.DeserializeObject<T>(value));
        }

        public async Task AddSingleAsync(string key, object value, DateTime? expiration = null)
        {
            _logger.LogDebug($"CACHE | ADD SINGLE VALUE ON KEY {key} WITH EXPIRATION ON {expiration} VALUE {value}");

            var json = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            await _cacheFactory.Database().StringSetAsync(key, new RedisValue(json));

            if (expiration != null)
            {
                await SetExpirationAsync(key, expiration.Value);
            }

            _logger.LogDebug($"CACHE | SINGLE CREATED");
        }

        public async Task AddListAsync(string key, object value, DateTime? expiration = null)
        {
            _logger.LogDebug($"CACHE | ADD RANGE VALUE ON KEY {key} WITH EXPIRATION ON {expiration} VALUE {value}");

            var json = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            await _cacheFactory.Database().ListLeftPushAsync(key, new RedisValue(json));

            if (expiration != null)
            {
                await SetExpirationAsync(key, expiration.Value);
            }

            _logger.LogDebug($"CACHE | LIST CREATED");
        }

        public async Task RemoveAsync(string key)
        {
            _logger.LogDebug($"CACHE | REMOVING KEY {key}");

            await _cacheFactory.Database().KeyDeleteAsync(new RedisKey(key));

            _logger.LogDebug($"CACHE | KEY REMOVED");
        }

        public async Task SetExpirationAsync(string key, DateTime expiration)
        {
            _logger.LogDebug($"CACHE | SET EXPIRATION {expiration} TO KEY {key}");

            await _cacheFactory.Database().KeyExpireAsync(new RedisKey(key), expiration);

            _logger.LogDebug($"CACHE | EXPIRATION SET");
        }
    }
}
