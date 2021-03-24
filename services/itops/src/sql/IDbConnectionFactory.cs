using System.Data.Common;
using System.Threading.Tasks;

namespace artiso.AdsdHotel.ITOps.Sql
{
    public interface IDbConnectionFactory
    {
        Task<DbConnection> CreateAsync();
    }
}
