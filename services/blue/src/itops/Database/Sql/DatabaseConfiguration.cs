using MySql.Data.MySqlClient;

namespace artiso.AdsdHotel.ITOps.Database.Sql
{
    public record DatabaseConfiguration(
        string Host,
        int Port,
        string Database,
        string Username,
        string? Password)
    {
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
