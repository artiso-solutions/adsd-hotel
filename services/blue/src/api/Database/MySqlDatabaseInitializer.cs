using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Sql;
using MySql.Data.MySqlClient;

namespace artiso.AdsdHotel.Blue.Api
{
    internal class MySqlDatabaseInitializer
    {
        static MySqlDatabaseInitializer()
        {
            // Necessary setup for RepoDb and DbConnectionHolder abstraction.
            RepoDb.MySqlBootstrap.Initialize();
        }

        private readonly SqlConfig _dbConfig;

        public MySqlDatabaseInitializer(SqlConfig dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public async Task EnsureCreatedAsync()
        {
            var connectionString = _dbConfig.ToMySqlConnectionString(includeDatabase: false);

            await using var connection = new MySqlConnection(connectionString);
            await connection.EnsureOpenAsync();

            using var command = RepoDb.DbConnectionExtension.CreateCommand(
                connection,
                $"CREATE DATABASE IF NOT EXISTS `{_dbConfig.Database}`;");

            await command.ExecuteNonQueryAsync();
        }
    }
}
