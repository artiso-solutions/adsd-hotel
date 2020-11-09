using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace artiso.AdsdHotel.Blue.Api
{
    internal static class DatabaseExtensions
    {
        public static async Task CommitAsync(this IDbTransaction transaction)
        {
            if (transaction is DbTransaction dbTransaction)
            {
                await dbTransaction.CommitAsync();
            }
            else
            {
                transaction.Commit();
            }
        }

        public static async Task RollbackAsync(this IDbTransaction transaction)
        {
            if (transaction is DbTransaction dbTransaction)
            {
                await dbTransaction.RollbackAsync();
            }
            else
            {
                transaction.Rollback();
            }
        }

        public static async Task ExecuteNonQueryAsync(this IDbCommand command)
        {
            if (command is DbCommand dbCommand)
            {
                await dbCommand.ExecuteNonQueryAsync();
            }
            else
            {
                command.ExecuteNonQuery();
            }
        }
    }
}
