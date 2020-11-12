using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using artiso.AdsdHotel.Infrastructure.DataStorage;
using MongoDB.Driver;

namespace artiso.AdsdHotel.Infrastructure.MongoDataStorage
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
        public async Task AddOrUpdate<T>(T entity, Expression<Func<T, bool>> filter)
        {
            var col = _db.GetCollection<T>(_collection);
            // ToDo should we return a generated id here?
            await col.ReplaceOneAsync(filter, entity, new ReplaceOptions { IsUpsert = true }).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<T> Get<T>(Expression<Func<T, bool>> filter)
        {
            var col = _db.GetCollection<T>(_collection);
            var resultCollection = await col.FindAsync<T>(filter).ConfigureAwait(false);
            var result = await resultCollection.FirstOrDefaultAsync().ConfigureAwait(false);
            return result;
        }
    }
}
