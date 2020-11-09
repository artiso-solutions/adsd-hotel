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

        public MongoDataStoreClient(Uri endpoint, string database, string collection)
        {
            var connectionString = $"{endpoint.Scheme}://{endpoint.Host}:{endpoint.Port}";
            _client = new MongoClient(connectionString);
            _db = _client.GetDatabase(database);
            _collection = collection;
        }

        public async Task AddOrUpdate<T>(T entity)
        {
            var col = _db.GetCollection<T>(_collection);
            //var filter = Builders<GuestInformationRecord>.Filter.Eq(x => x.OrderId, message.OrderId);
            //var update = Builders<GuestInformationRecord>.Update.Set(x => x.GuestInformation, message.GuestInformation);
            //var options = new FindOneAndUpdateOptions<GuestInformationRecord>();
            //options.IsUpsert = true;
            //var proj = await collection.FindOneAndUpdateAsync(filter, update, options).ConfigureAwait(false);
            await col.InsertOneAsync(entity).ConfigureAwait(false);
        }

        public async Task<T> Get<T> (Expression<Func<T, bool>> filter)
        {
            var col = _db.GetCollection<T>(_collection);
            var resultCollection = await col.FindAsync<T>(filter).ConfigureAwait(false);
            var result = await resultCollection.FirstOrDefaultAsync().ConfigureAwait(false);
            return result;
        }
    }
}
