using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using artiso.AdsdHotel.Infrastructure.DataStorage;
using Microsoft.VisualBasic.FileIO;
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
        public async Task AddOrUpdateAsync<T>(T entity, Expression<Func<T, bool>> filter)
        {
            var col = _db.GetCollection<T>(_collection);
            await col.ReplaceOneAsync(filter, entity, new ReplaceOptions { IsUpsert = true }).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<T> GetAsync<T>(Expression<Func<T, bool>> filter)
        {
            var col = _db.GetCollection<T>(_collection);
            var resultCollection = await col.FindAsync<T>(filter).ConfigureAwait(false);
            var result = await resultCollection.FirstOrDefaultAsync().ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc/>
        public async Task<List<T>> GetAllAsync<T>(Expression<Func<T, bool>> filter)
        {
            var col = _db.GetCollection<T>(_collection);
            var result = await col.FindAsync(filter).ConfigureAwait(false);
            var list = await result.ToListAsync().ConfigureAwait(false);
            return list;
        }
    }
}
