using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using RepoDb;

namespace artiso.AdsdHotel.Blue.Api
{
    internal class MySqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public MySqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IDbConnectionHolder> CreateAsync()
        {
            var mySqlConnection = new MySqlConnection(_connectionString);
            var holder = new DbConnectionHolder(mySqlConnection);

            await holder.EnsureOpenAsync();

            return holder;
        }
    }
}
