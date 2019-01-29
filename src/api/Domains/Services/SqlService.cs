using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Configurations.Factories;
using API.Domains.Models;
using Dapper;
using Microsoft.Extensions.Logging;

namespace API.Domains.Services
{
    public interface ISqlService
    {
        Task ExecuteAsync(string sql, object parameters);
        Task<int> CreateAsync(string sql, object parameters);
        Task<IEnumerable<T>> ListAsync<T>(string sql, object parameters);
        Task<T> GetAsync<T>(string sql, object parameters);
        Task<int> CountAsync(string sql, object parameters);
        Task<bool> ExistsAsync(string sql, object parameters);
        Task<string> ObtainAsync(string sql, object parameters);
    }
    
    public class SqlService : ISqlService
    {
        private readonly IDatabaseFactory _databaseFactory;
        private readonly ILogger<SqlService> _logger;

        public SqlService(IDatabaseFactory databaseFactory, ILogger<SqlService> logger)
        {
            _databaseFactory = databaseFactory;
            _logger = logger;
        }
        
        public async Task ExecuteAsync(string sql, object parameters)
        {
            _logger.LogDebug($"QUERY EXECUTE COMMAND | { sql }");
            _logger.LogDebug($"QUERY EXECUTE PARAMETERS | { parameters }");

            var transaction = await _databaseFactory.BeginTransactionAsync();

            var command = new CommandDefinition(sql, parameters, transaction);

            await _databaseFactory.Connection().ExecuteAsync(command);

            _logger.LogDebug($"QUERY EXECUTE EXECUTED");
        }

        public async Task<int> CreateAsync(string sql, object parameters)
        {
            _logger.LogDebug($"QUERY CREATE COMMAND | { sql }");
            _logger.LogDebug($"QUERY CREATE PARAMETERS | { parameters }");

            var transaction = await _databaseFactory.BeginTransactionAsync();

            var command = new CommandDefinition(sql, parameters, transaction);

            var id = await _databaseFactory.Connection().ExecuteScalarAsync(command);

            _logger.LogDebug($"QUERY CREATE EXECUTED");

            return Convert.ToInt32(id);
        }

        public async Task<IEnumerable<T>> ListAsync<T>(string sql, object parameters)
        {
            _logger.LogDebug($"QUERY LIST COMMAND | { sql }");
            _logger.LogDebug($"QUERY LIST PARAMETERS | { parameters }");

            var transaction = await _databaseFactory.BeginTransactionAsync();

            var command = new CommandDefinition(sql, parameters, transaction);

            var result = await _databaseFactory.Connection().QueryAsync<T>(command);    

            _logger.LogDebug($"QUERY LIST EXECUTED");

            return result;
        }
        
        public async Task<T> GetAsync<T>(string sql, object parameters)
        {
            _logger.LogDebug($"QUERY GET COMMAND | { sql }");
            _logger.LogDebug($"QUERY GET PARAMETERS | { parameters }");

            var transaction = await _databaseFactory.BeginTransactionAsync();

            var command = new CommandDefinition(sql, parameters, transaction);

            var result = await _databaseFactory.Connection().QueryFirstOrDefaultAsync<T>(command);

            _logger.LogDebug($"QUERY GET EXECUTED");

            return result;
        }

        public async Task<int> CountAsync(string sql, object parameters)
        {
            _logger.LogDebug($"QUERY COUNT COMMAND | { sql }");
            _logger.LogDebug($"QUERY COUNT PARAMETERS | { parameters }");

            var transaction = await _databaseFactory.BeginTransactionAsync();

            var command = new CommandDefinition(sql, parameters, transaction);

            var result = await _databaseFactory.Connection().QueryFirstOrDefaultAsync<int>(command);

            _logger.LogDebug($"QUERY COUNT EXECUTED");

            return result;
        }

        public async Task<bool> ExistsAsync(string sql, object parameters)
        {
            _logger.LogDebug($"QUERY EXISTS COMMAND | { sql }");
            _logger.LogDebug($"QUERY EXISTS PARAMETERS | { parameters }");

            var transaction = await _databaseFactory.BeginTransactionAsync();

            var command = new CommandDefinition(sql, parameters, transaction);

            var result = await _databaseFactory.Connection().QueryFirstOrDefaultAsync<bool>(command);

            _logger.LogDebug($"QUERY EXISTS EXECUTED");

            return result;
        }

        public async Task<string> ObtainAsync(string sql, object parameters)
        {
            _logger.LogDebug($"QUERY OBTAIN COMMAND | { sql }");
            _logger.LogDebug($"QUERY OBTAIN PARAMETERS | { parameters }");

            var transaction = await _databaseFactory.BeginTransactionAsync();

            var command = new CommandDefinition(sql, parameters, transaction);

            var result = await _databaseFactory.Connection().QueryFirstOrDefaultAsync<string>(command);

            _logger.LogDebug($"QUERY OBTAIN EXECUTED");

            return result;
        }
    }
}
