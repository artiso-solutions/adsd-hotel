using MySql.Data.MySqlClient;

namespace artiso.AdsdHotel.Blue.Api
{
    internal class DatabaseConfiguration
    {
        public string Host { get; }

        public int Port { get; }

        public string Database { get; }

        public string Username { get; }

        public string? Password { get; }

        public DatabaseConfiguration(
            string host,
            int port,
            string database,
            string username,
            string? password)
        {
            Host = host;
            Port = port;
            Database = database;
            Username = username;
            Password = password;
        }

        public string ToMySqlConnectionString(bool includeDatabase = true)
        {
            var csBuilder = new MySqlConnectionStringBuilder
            {
                Server = Host,
                Port = (uint)Port,
                Database = includeDatabase ? Database : default,
                UserID = Username,
                Password = Password,
            };

            var cs = csBuilder.GetConnectionString(includePass: true);
            return cs;
        }
    }
}
