using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace artiso.AdsdHotel.Infrastructure.DataStorage
{
    public interface IDataStoreClient
    {
        Task AddOrUpdate<T>(T entity);
        Task<T> Get<T>(Expression<Func<T, bool>> filter);
    }
}
