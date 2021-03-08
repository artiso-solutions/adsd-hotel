﻿using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace artiso.AdsdHotel.ITOps.Sql
{
    public class MySqlConnectionFactory : IDbConnectionFactory
    {
        private readonly DatabaseConfiguration _dbConfig;

        public MySqlConnectionFactory(DatabaseConfiguration dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public async Task<DbConnection> CreateAsync()
        {
            var connectionString = _dbConfig.ToMySqlConnectionString();

            var mySqlConnection = new MySqlConnection(connectionString);
            await mySqlConnection.EnsureOpenAsync();

            return mySqlConnection;
        }
    }
}