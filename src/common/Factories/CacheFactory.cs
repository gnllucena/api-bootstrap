using Common.Models.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Factories
{
    public interface ICacheFactory
    {
        Task ConnectAsync();
        Task DisconnectAsync();
        IDatabase Database();
    }

    public class CacheFactory : ICacheFactory
    {
        private IDatabase _database;
        private IConnectionMultiplexer _connectionMultiplexer;
        private readonly Cache _cache;
        private readonly ILogger<CacheFactory> _logger;
        
        public CacheFactory(
            IOptions<Cache> cache,
            ILogger<CacheFactory> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache.Value ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task ConnectAsync()
        {
            var connectionString = $"{string.Join(",", _cache.Addresses.Select(address => $"{address}:{_cache.Port}"))},password={_cache.Password}";

            _connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString);

            _database = _connectionMultiplexer.GetDatabase();

            _connectionMultiplexer.ConnectionFailed += ConnectionFailed;
            _connectionMultiplexer.ConnectionRestored += ConnectionRestored;
        }

        public async Task DisconnectAsync()
        {
            if (_connectionMultiplexer == null)
            {
                throw new Exception("Redis connection is not opened.");
            }

            if (_connectionMultiplexer.IsConnected == false)
            {
                throw new Exception("Redis connection is not opened.");
            }

            await _connectionMultiplexer.CloseAsync().ConfigureAwait(false);
        }

        public IDatabase Database()
        {
            return _database;
        }

        public void ConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            _logger.LogCritical($"REDIS CONNECTION LOST | {e.Exception}");
        }

        public void ConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            _logger.LogInformation($"REDIS CONNCECTION RESTORED | {e.Exception}");
        }
    }
}
