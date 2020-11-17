using Common.Models.Options;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Common.Factories
{
    public interface IDatabaseFactory
    {
        IDbTransaction BeginTransaction();
        IDbConnection Connection();
        Task OpenConnectionAsync();
        void CommitTransaction();
        void RollbackTransaction();
        void CloseConnection();
    }

    public class DatabaseFactory : IDatabaseFactory
    {
        private readonly MySqlConnection _mySqlConnection;
        private readonly Connection _connection;
        private MySqlTransaction _transaction;
        private bool _isTransactionOpen;

        public DatabaseFactory(IOptions<Connection> connection)
        {
            _connection = connection.Value ?? throw new ArgumentNullException(nameof(connection));

            var connectionString = $"Server={_connection.Server};Port={_connection.Port};Database={_connection.Database};Uid={_connection.User};Pwd={_connection.Password};";

            _mySqlConnection = new MySqlConnection(connectionString);
        }

        public IDbTransaction BeginTransaction()
        {
            if (_transaction == null)
            {
                if (_mySqlConnection.State != ConnectionState.Open)
                {
                    throw new Exception("Dabase connection is not opened.");
                }

                _transaction = _mySqlConnection.BeginTransaction();
            }

            _isTransactionOpen = true;

            return _transaction;
        }

        public IDbConnection Connection()
        {
            return _mySqlConnection;
        }

        public async Task OpenConnectionAsync()
        {
            await _mySqlConnection.OpenAsync().ConfigureAwait(false);
        }

        public void CommitTransaction()
        {
            if (_isTransactionOpen)
            {
                _transaction.Commit();
                _transaction = null;
            }

            _isTransactionOpen = false;
        }

        public void RollbackTransaction()
        {
            if (!_isTransactionOpen)
            {
                return;
            }
            _transaction.Rollback();
            _transaction = null;
        }

        public void CloseConnection()
        {
            _mySqlConnection.Close();
        }
    }
}
