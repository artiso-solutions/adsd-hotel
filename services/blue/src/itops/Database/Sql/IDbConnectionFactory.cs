using System.Data.Common;
using System.Threading.Tasks;

namespace artiso.AdsdHotel.ITOps.Database.Sql
{
    public interface IDbConnectionFactory
    {
        Task<DbConnection> CreateAsync();
    }
}
