using System;
using System.Data;
using System.Threading.Tasks;
using API.Domain.Models.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace API.Configuration.Factories
{
    public interface IDatabaseFactory
    {
        Task<IDbTransaction> BeginTransactionAsync();
        IDbConnection Connection();
        Task OpenConnectionAsync();
        void CommitTransaction();
        void RollbackTransaction();
        void CloseConnection();
    }

    public class DatabaseFactory : IDatabaseFactory
    {
        private readonly MySqlConnection _connection;
        private MySqlTransaction _transaction;
        private bool _isTransactionOpen;

        public DatabaseFactory(IOptions<Database> database)
        {
            var connectionstring = $"Server={database.Value.Server};Port={database.Value.Port};Database={database.Value.Schema};Uid={database.Value.User};Pwd={database.Value.Password};";

            _connection = new MySqlConnection(connectionstring);
        }
        
        public async Task<IDbTransaction> BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                if (_connection.State != ConnectionState.Open)
                    throw new Exception("A conexão com o banco não esta aberta.");

                _transaction = await _connection.BeginTransactionAsync();
            }

            _isTransactionOpen = true;

            return _transaction;
        }

        public IDbConnection Connection()
        {
            return _connection;
        }

        public async Task OpenConnectionAsync()
        {
            var connection = _connection as MySqlConnection;

            await connection.OpenAsync();
        }

        public void CommitTransaction()
        {
            if (_isTransactionOpen)
            {
                _transaction.Commit();
                _transaction.Dispose();
                _transaction = null;
            }

            _isTransactionOpen = false;
        }

        public void RollbackTransaction()
        {
            if (_isTransactionOpen)
                _transaction.Rollback();
        }

        public void CloseConnection()
        {
            _connection.Close();

            _connection.Dispose();
        }
    }

}