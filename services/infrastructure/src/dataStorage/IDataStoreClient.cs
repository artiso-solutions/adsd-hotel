using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace artiso.AdsdHotel.Infrastructure.DataStorage
{
    /// <summary>
    /// Interface for accessing data storage.
    /// </summary>
    public interface IDataStoreClient
    {
        /// <summary>
        /// Adds or upates an entity matching the given filter.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="filter">A filter to search for the entity.</param>
        /// <returns>A task that can be awaited.</returns>
        Task AddOrUpdate<T>(T entity, Expression<Func<T, bool>> filter);

        /// <summary>
        /// Retrieves an single entity from the data store given a filter.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="filter">The filter to search for the entity.</param>
        /// <returns>A task that can be awaited with the found entity if it was found.</returns>
        Task<T> Get<T>(Expression<Func<T, bool>> filter);
    }
}
