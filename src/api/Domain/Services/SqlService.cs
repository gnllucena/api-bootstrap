using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Configurations.Factories;
using API.Domain.Models;
using Dapper;

namespace API.Domain.Services
{
    public interface ISqlService
    {
        Task ExecuteAsync(string sql, object parameters);
        Task<int> Create(string sql, object parameters);
        Task<IEnumerable<T>> ListAsync<T>(string sql, object parameters);
        Task<T> GetAsync<T>(string sql, object parameters);
        Task<int> CountAsync(string sql, object parameters);
        Task<bool> ExistsAsync(string sql, object parameters);
    }
    
    public class SqlService : ISqlService
    {
        private readonly IDatabaseFactory _databaseFactory;

        public SqlService(IDatabaseFactory databaseFactory)
        {
            _databaseFactory = databaseFactory;
        }
        
        public async Task ExecuteAsync(string sql, object parameters)
        {
            var transaction = await _databaseFactory.BeginTransactionAsync();

            var command = new CommandDefinition(sql, parameters, transaction);

            await _databaseFactory.Connection().ExecuteAsync(command);
        }

        public async Task<int> Create(string sql, object parameters)
        {
            var transaction = await _databaseFactory.BeginTransactionAsync();

            var command = new CommandDefinition(sql, parameters, transaction);

            var id = await _databaseFactory.Connection().ExecuteScalarAsync(command);

            return Convert.ToInt32(id);
        }

        public async Task<IEnumerable<T>> ListAsync<T>(string sql, object parameters)
        {
            var transaction = await _databaseFactory.BeginTransactionAsync();

            var command = new CommandDefinition(sql, parameters, transaction);

            return await _databaseFactory.Connection().QueryAsync<T>(command);
        }
        
        public async Task<T> GetAsync<T>(string sql, object parameters)
        {
            var transaction = await _databaseFactory.BeginTransactionAsync();

            var command = new CommandDefinition(sql, parameters, transaction);

            return await _databaseFactory.Connection().QueryFirstOrDefaultAsync<T>(command);
        }

        public async Task<int> CountAsync(string sql, object parameters)
        {
            var transaction = await _databaseFactory.BeginTransactionAsync();

            var command = new CommandDefinition(sql, parameters, transaction);

            return await _databaseFactory.Connection().QueryFirstOrDefaultAsync<int>(command);
        }

        public async Task<bool> ExistsAsync(string sql, object parameters)
        {
            var transaction = await _databaseFactory.BeginTransactionAsync();

            var command = new CommandDefinition(sql, parameters, transaction);

            return await _databaseFactory.Connection().QueryFirstOrDefaultAsync<bool>(command);
        }
    }
}
