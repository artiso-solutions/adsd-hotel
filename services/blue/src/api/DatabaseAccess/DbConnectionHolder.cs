using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace artiso.AdsdHotel.Blue.Api
{
    internal interface IDbConnectionHolder : IDbConnection, IAsyncDisposable
    {
    }

    internal class DbConnectionHolder : IDbConnectionHolder
    {
        private readonly IDbConnection _connection;

        public DbConnectionHolder(IDbConnection connection)
        {
            _connection = connection;
        }

        #region IDbConnection

        string IDbConnection.ConnectionString
        {
            get => _connection.ConnectionString;
            set => _connection.ConnectionString = value;
        }

        int IDbConnection.ConnectionTimeout => _connection.ConnectionTimeout;

        string IDbConnection.Database => _connection.Database;

        ConnectionState IDbConnection.State => _connection.State;

        IDbTransaction IDbConnection.BeginTransaction() => _connection.BeginTransaction();

        IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il) => _connection.BeginTransaction(il);

        void IDbConnection.ChangeDatabase(string databaseName) => _connection.ChangeDatabase(databaseName);

        void IDbConnection.Close() => _connection.Close();

        IDbCommand IDbConnection.CreateCommand() => _connection.CreateCommand();

        void IDisposable.Dispose() => _connection.Dispose();

        void IDbConnection.Open() => _connection.Open();

        #endregion

        #region IAsyncDisposable

        public async ValueTask DisposeAsync()
        {
            if (_connection is DbConnection dbConnection)
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
