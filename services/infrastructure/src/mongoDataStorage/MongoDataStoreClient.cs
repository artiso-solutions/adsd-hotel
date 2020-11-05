using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using artiso.AdsdHotel.Infrastructure.DataStorage;
using MongoDB.Driver;

namespace artiso.AdsdHotel.Infrastructure.MongoDataStorage
{
    public class MongoDataStoreClient : IDataStoreClient
    {
        private readonly MongoClient client;
        private readonly IMongoDatabase db;
        private readonly string collection;

        public MongoDataStoreClient(Uri endpoint, string database, string collection)
        {
            var connectionString = $"{endpoint.Scheme}://{endpoint.Host}:{endpoint.Port}";
            client = new MongoClient(connectionString);
            db = client.GetDatabase(database);
            this.collection = collection;
        }

        public async Task AddOrUpdate<T>(T entity)
        {
            var col = db.GetCollection<T>(collection);
            //var filter = Builders<GuestInformationRecord>.Filter.Eq(x => x.OrderId, message.OrderId);
            //var update = Builders<GuestInformationRecord>.Update.Set(x => x.GuestInformation, message.GuestInformation);
            //var options = new FindOneAndUpdateOptions<GuestInformationRecord>();
            //options.IsUpsert = true;
            //var proj = await collection.FindOneAndUpdateAsync(filter, update, options).ConfigureAwait(false);
            await col.InsertOneAsync(entity).ConfigureAwait(false);
        }

        public async Task<T> Get<T> (Expression<Func<T, bool>> filter)
        {
            var col = db.GetCollection<T>(collection);
            var resultCollection = await col.FindAsync<T>(filter).ConfigureAwait(false);
            var result = await resultCollection.FirstOrDefaultAsync().ConfigureAwait(false);
            return result;
        }
    }
}
