using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace artiso.AdsdHotel.ITOps.Sql
{
    public class MySqlConnectionFactory : IDbConnectionFactory
    {
        private readonly SqlConfig? _sqlConfig;
        private readonly IOptions<SqlConfig>? _sqlConfigOptions;

        public MySqlConnectionFactory(SqlConfig dbConfig)
        {
            _sqlConfig = dbConfig;
        }

        public MySqlConnectionFactory(IOptions<SqlConfig> dbConfigOptions)
        {
            _sqlConfigOptions = dbConfigOptions;
        }

        public async Task<DbConnection> CreateAsync()
        {
            var config = GetConfig();
            var connectionString = config.ToMySqlConnectionString();

            var mySqlConnection = new MySqlConnection(connectionString);
            await mySqlConnection.EnsureOpenAsync();

            return mySqlConnection;
        }

        private SqlConfig GetConfig() => _sqlConfig ?? _sqlConfigOptions!.Value;
    }
}
