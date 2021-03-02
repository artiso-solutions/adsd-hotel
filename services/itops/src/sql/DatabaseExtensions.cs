using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace artiso.AdsdHotel.ITOps.Sql
{
    public static class DatabaseExtensions
    {
        public static async Task<IDbConnection> EnsureOpenAsync(this IDbConnection connection,
            CancellationToken cancellationToken = default)
        {
            if (connection.State == ConnectionState.Open)
                return connection;

            if (connection is DbConnection dbConnection)
            {
                await dbConnection.OpenAsync(cancellationToken);
            }
            else
            {
                await Task.Run(() => connection.Open(), cancellationToken);
            }

            return connection;
        }

        public static async Task<int> ExecuteNonQueryAsync(this IDbCommand command)
        {
            if (command is DbCommand dbCommand)
            {
                return await dbCommand.ExecuteNonQueryAsync();
            }
            else
            {
                return command.ExecuteNonQuery();
            }
        }

        public static async Task<bool> ReadAsync(this IDataReader reader)
        {
            if (reader is DbDataReader dbDataReader)
            {
                return await dbDataReader.ReadAsync();
            }
            else
            {
                return reader.Read();
            }
        }
    }
}
