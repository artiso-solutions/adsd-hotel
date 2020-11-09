using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace artiso.AdsdHotel.Infrastructure.DataStorage
{
    public interface IDataStoreClient
    {
        Task AddOrUpdate<T, TFilterField, TValueField>(T entity, Expression<Func<T, TFilterField>> filterKey, Expression<Func<T, TValueField>> propertyToSet);
        Task<T> Get<T>(Expression<Func<T, bool>> filter);
    }
}
