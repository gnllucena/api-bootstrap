using Common.Factories;
using Dapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Services
{
    public interface ISqlService
    {
        Task ExecuteAsync(string sql, CommandType commandType, object parameters = null);
        Task<T> ExecuteScalarAsync<T>(string sql, CommandType commandType, object parameters = null);
        Task<IList<T>> QueryAsync<T>(string sql, CommandType commandType, object parameters = null);
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, CommandType commandType, object parameters = null);
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

        public async Task ExecuteAsync(string sql, CommandType commandType, object parameters = null)
        {
            _logger.LogDebug($"QUERY EXECUTE COMMAND | { sql.Trim() }");
            _logger.LogDebug($"QUERY EXECUTE PARAMETERS | { parameters }");

            var transaction = _databaseFactory.BeginTransaction();

            await _databaseFactory.Connection().ExecuteAsync(
                sql: sql,
                param: parameters,
                transaction: transaction,
                commandType: commandType
            );

            _logger.LogDebug("QUERY EXECUTE EXECUTED");
        }

        public async Task<T> ExecuteScalarAsync<T>(string sql, CommandType commandType, object parameters = null)
        {
            _logger.LogDebug($"QUERY SCALAR COMMAND | { sql.Trim() }");
            _logger.LogDebug($"QUERY SCALAR PARAMETERS | { parameters }");

            var transaction = _databaseFactory.BeginTransaction();

            var result = await _databaseFactory.Connection().ExecuteScalarAsync<T>(
                sql: sql,
                param: parameters,
                transaction: transaction,
                commandType: commandType
            );

            _logger.LogDebug("QUERY SCALAR EXECUTED");

            return result;
        }

        public async Task<IList<T>> QueryAsync<T>(string sql, CommandType commandType, object parameters = null)
        {
            _logger.LogDebug($"QUERY LIST COMMAND | { sql.Trim() }");
            _logger.LogDebug($"QUERY LIST PARAMETERS | { parameters }");

            var transaction = _databaseFactory.BeginTransaction();

            var result = await _databaseFactory.Connection().QueryAsync<T>(
                sql: sql,
                param: parameters,
                transaction: transaction,
                commandType: commandType
            );

            _logger.LogDebug("QUERY LIST EXECUTED");

            return result.ToList();
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, CommandType commandType, object parameters = null)
        {
            _logger.LogDebug($"QUERY GET COMMAND | { sql.Trim() }");
            _logger.LogDebug($"QUERY GET PARAMETERS | { parameters }");

            var transaction = _databaseFactory.BeginTransaction();

            var result = await _databaseFactory.Connection().QueryFirstOrDefaultAsync<T>(
                sql: sql,
                param: parameters,
                transaction: transaction,
                commandType: commandType
            );

            _logger.LogDebug("QUERY GET EXECUTED");

            return result;
        }
    }
}