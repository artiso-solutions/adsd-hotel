using MySql.Data.MySqlClient;

namespace artiso.AdsdHotel.ITOps.Sql
{
    public static class SqlConfigExtensions
    {
        public static string ToMySqlConnectionString(this SqlConfig config, bool includeDatabase = true)
        {
            var csBuilder = new MySqlConnectionStringBuilder
            {
                Server = config.Host,
                Port = (uint)config.Port,
                Database = includeDatabase ? config.Database : default,
                UserID = config.User,
                Password = config.Password,
            };

            var cs = csBuilder.GetConnectionString(includePass: true);
            return cs;
        }

    }
}
