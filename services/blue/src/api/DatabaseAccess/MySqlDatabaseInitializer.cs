using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using RepoDb;

namespace artiso.AdsdHotel.Blue.Api
{
    internal class MySqlDatabaseInitializer
    {
        private readonly DatabaseConfiguration _dbConfig;

        public MySqlDatabaseInitializer(DatabaseConfiguration dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public async Task EnsureCreatedAsync()
        {
            var connectionString = _dbConfig.ToMySqlConnectionString(includeDatabase: false);

            await using var connection = new MySqlConnection(connectionString);
            await connection.EnsureOpenAsync();

            using var command = connection.CreateCommand(
                $"CREATE DATABASE IF NOT EXISTS `{_dbConfig.Database}`;");

            await command.ExecuteNonQueryAsync();
        }
    }
}
