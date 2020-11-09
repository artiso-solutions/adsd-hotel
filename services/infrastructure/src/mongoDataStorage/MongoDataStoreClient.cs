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

        public async Task AddOrUpdate<T,TFilterField,TValueField>(
            T entity, 
            Expression<Func<T,TFilterField>> filterKey, 
            Expression<Func<T,TValueField>> propertyToSet )
        {
            var col = db.GetCollection<T>(collection);

            // that may be slow here
            var filterCompareValue = filterKey.Compile().Invoke(entity);
            var filterDefinition = Builders<T>.Filter.Eq(filterKey, filterCompareValue);

            // that may be slow here
            var updateValue = propertyToSet.Compile().Invoke(entity);
            var update = Builders<T>.Update.Set(propertyToSet, updateValue);

            var options = new UpdateOptions();
            options.IsUpsert = true;

            await col.UpdateOneAsync(filterDefinition, update, options, default);
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
