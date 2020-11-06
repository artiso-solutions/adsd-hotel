﻿using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace artiso.AdsdHotel.Blue.Api
{
    internal class DbConnectionHolder : IDbConnectionHolder
    {
        private readonly IDbConnection _connection;
        private IDbTransaction? _transaction;

        public DbConnectionHolder(IDbConnection connection)
        {
            _connection = connection;
        }

        public bool HasTransaction => !(_transaction is null);

        #region IDbConnection

        public string ConnectionString
        {
            get => _connection.ConnectionString;
            set => _connection.ConnectionString = value;
        }

        public int ConnectionTimeout => _connection.ConnectionTimeout;

        public string Database => _connection.Database;

        public ConnectionState State => _connection.State;

        public void Open() => _connection.Open();

        public void Close() => _connection.Close();

        public void ChangeDatabase(string databaseName) => _connection.ChangeDatabase(databaseName);

        public IDbTransaction BeginTransaction()
        {
            if (_transaction is null)
                _transaction = _connection.BeginTransaction();

            throw new InvalidOperationException("Can't create multiple transactions on this connection instance.");
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            if (_transaction is null)
                _transaction = _connection.BeginTransaction(il);

            throw new InvalidOperationException("Can't create multiple transactions on this connection instance.");
        }

        public IDbCommand CreateCommand()
        {
            var command = _connection.CreateCommand();
            command.Transaction = _transaction;
            return command;
        }

        public void Dispose() => _connection.Dispose();

        #endregion

        #region IAsyncDisposable

        public async ValueTask DisposeAsync()
        {
            if (_connection is IAsyncDisposable asyncDisposableConnection)
            {
                await asyncDisposableConnection.DisposeAsync();
            }
            else if (_connection is DbConnection dbConnection)
            {
                await dbConnection.DisposeAsync();
            }
            else
            {
                _connection.Dispose();
            }
        }

        #endregion
    }
}
