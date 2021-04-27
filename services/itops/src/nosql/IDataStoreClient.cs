using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace artiso.AdsdHotel.ITOps.NoSql
{
    /// <summary>
    /// Interface for accessing data storage.
    /// </summary>
    public interface IDataStoreClient
    {
        /// <summary>
        /// Adds a new document.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>A task that can be awaited.</returns>
        Task InsertOneAsync<T>(T entity);
        
        /// <summary>
        /// Adds or updates an entity matching the given filter.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="filter">A filter to search for the entity.</param>
        /// <returns>A task that can be awaited.</returns>
        Task AddOrUpdateAsync<T>(T entity, Expression<Func<T, bool>> filter);

        /// <summary>
        /// Retrieves all entities from the data store given a filter.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="combinator">How to combine the filters. and/or </param>
        /// <param name="filters">The filters to search for the entity.</param>
        /// <returns>A task that can be awaited with the found entities if any.</returns>
        Task<List<T>> GetAllAsync<T>(ExpressionCombinationOperator combinator, params Expression<Func<T, bool>>[] filters);

        /// <summary>
        /// Retrieves an single entity from the data store given a filter.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="combinator">How to combine the filters. and/or </param>
        /// <param name="filters">The filters to search for the entity.</param>
        /// <returns>A task that can be awaited with the found entity if it was found.</returns>
        Task<T?> GetAsync<T>(ExpressionCombinationOperator combinator, params Expression<Func<T, bool>>[] filters);
    }
}
