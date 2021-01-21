using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using RepoDb;

namespace artiso.AdsdHotel.Blue.Api
{
    internal class MySqlConnectionFactory : IDbConnectionFactory
    {
        private readonly DatabaseConfiguration _dbConfig;

        public MySqlConnectionFactory(DatabaseConfiguration dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public async Task<IDbConnectionHolder> CreateAsync()
        {
            var connectionString = _dbConfig.ToMySqlConnectionString();

            var mySqlConnection = new MySqlConnection(connectionString);
            var holder = new DbConnectionHolder(mySqlConnection);

            await holder.EnsureOpenAsync();

            return holder;
        }
    }
}
