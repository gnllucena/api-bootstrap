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
        Task<IEnumerable<T>> ListAsync<T>(string sql, object parameters);
        Task<T> GetAsync<T>(string sql, object parameters);
    }
    
    public class SqlService : ISqlService
    {
        private readonly IDatabaseFactory _databaseFactory;

        public SqlService(IDatabaseFactory databaseFactory)
        {
            _databaseFactory = databaseFactory;
        }
        
        public async Task ExecuteAsync(string sql, object param)
        {
            var transaction = await _databaseFactory.BeginTransactionAsync();

            var command = new CommandDefinition(sql, param, transaction);

            await _databaseFactory.Connection().ExecuteAsync(command);
        }

        public async Task<IEnumerable<T>> ListAsync<T>(string sql, object param)
        {
            var transaction = await _databaseFactory.BeginTransactionAsync();

            var command = new CommandDefinition(sql, param, transaction);

            return await _databaseFactory.Connection().QueryAsync<T>(command);
        }
        
        public async Task<T> GetAsync<T>(string sql, object param)
        {
            var transaction = await _databaseFactory.BeginTransactionAsync();

            var command = new CommandDefinition(sql, param, transaction);

            return await _databaseFactory.Connection().QueryFirstOrDefaultAsync<T>(command);
        }
    }
}
