using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace artiso.AdsdHotel.ITOps.NoSql
{
    public class MongoDataStoreClient : IDataStoreClient
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _db;
        private readonly string _collection;

        /// <summary>
        /// Creates a client for accessing a mongodb.
        /// </summary>
        /// <param name="endpoint">The host where the mongodb runs.</param>
        /// <param name="database">The name of the database.</param>
        /// <param name="collection">The name of the collection.</param>
        public MongoDataStoreClient(Uri endpoint, string database, string collection)
        {
            var connectionString = $"{endpoint.Scheme}://{endpoint.Host}:{endpoint.Port}";
            _client = new MongoClient(connectionString);
            _db = _client.GetDatabase(database);
            _collection = collection;
        }

        /// <inheritdoc/>
        public async Task InsertOneAsync<T>(T entity)
        {
            var col = _db.GetCollection<T>(_collection);
            await col.InsertOneAsync(entity).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task AddOrUpdateAsync<T>(T entity, Expression<Func<T, bool>> filter)
        {
            var col = _db.GetCollection<T>(_collection);
            await col.ReplaceOneAsync(filter, entity, new ReplaceOptions { IsUpsert = true }).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<T?> GetAsync<T>(ExpressionCombinationOperator combinator, params Expression<Func<T, bool>>[] filters)
        {
            var col = _db.GetCollection<T>(_collection);
            var filter = CombineFilters(combinator, filters);
            var resultCollection = await col.FindAsync<T>(filter).ConfigureAwait(false);
            var result = await resultCollection.FirstOrDefaultAsync().ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc/>
        public async Task<List<T>> GetAllAsync<T>(ExpressionCombinationOperator combinator, params Expression<Func<T, bool>>[] filters)
        {
            // combinator is a workaround until we figure out how combined expressions work correctly

            var col = _db.GetCollection<T>(_collection);
            var filter = CombineFilters(combinator, filters);
            var result = await col.FindAsync(filter).ConfigureAwait(false);
            var list = await result.ToListAsync().ConfigureAwait(false);
            return list;
        }

        private FilterDefinition<T> CombineFilters<T>(ExpressionCombinationOperator combinator, params Expression<Func<T, bool>>[] filters)
        {
            var builder = new FilterDefinitionBuilder<T>();
            //var filter = builder.Empty;
            var filter = filters switch
            {
                { Length: 1 } => filters.First(),
                { Length: > 1 } => combinator switch
                {
                    ExpressionCombinationOperator.And => builder.And(filters.Select(f => builder.Where(f))),
                    ExpressionCombinationOperator.Or => builder.Or(filters.Select(f => builder.Where(f))),
                    _ => filters.First(),
                },
                _ => builder.Empty
            };
            return filter;
        }
    }
}
