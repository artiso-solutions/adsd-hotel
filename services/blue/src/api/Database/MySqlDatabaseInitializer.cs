using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Sql;
using Microsoft.Extensions.Options;
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

        private readonly SqlConfig? _sqlConfig;
        private readonly IOptions<SqlConfig>? _sqlConfigOptions;

        public MySqlDatabaseInitializer(SqlConfig dbConfig)
        {
            _sqlConfig = dbConfig;
        }

        public MySqlDatabaseInitializer(IOptions<SqlConfig> dbConfigOptions)
        {
            _sqlConfigOptions = dbConfigOptions;
        }

        public async Task EnsureCreatedAsync()
        {
            var config = GetConfig();
            var connectionString = config.ToMySqlConnectionString(includeDatabase: false);

            await using var connection = new MySqlConnection(connectionString);
            await connection.EnsureOpenAsync();

            using var command = RepoDb.DbConnectionExtension.CreateCommand(
                connection,
                $"CREATE DATABASE IF NOT EXISTS `{config.Database}`;");

            await command.ExecuteNonQueryAsync();
        }

        private SqlConfig GetConfig() => _sqlConfig ?? _sqlConfigOptions!.Value;
    }
}
