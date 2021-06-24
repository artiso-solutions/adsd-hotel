using System;
using Microsoft.Extensions.Options;

namespace artiso.AdsdHotel.ITOps.NoSql
{
    public interface IMongoDbClientFactory
    {
        MongoDataStoreClient GetClient(Type type);
        MongoDbConfig GetConfig();
    }

    public class MongoDbClientFactory : IMongoDbClientFactory
    {
        private readonly MongoDbConfig? _mongoDbConfig;
        private readonly IOptions<MongoDbConfig>? _mongoDbConfigOptions;

        public MongoDbClientFactory(MongoDbConfig dbConfig)
        {
            _mongoDbConfig = dbConfig;
        }

        public MongoDbClientFactory(IOptions<MongoDbConfig> mongoDbConfigOptions)
        {
            _mongoDbConfigOptions = mongoDbConfigOptions;
        }

        public virtual MongoDataStoreClient GetClient(Type type)
        {
            var config = GetConfig();
            
            if (config.Database is null)
                throw new InvalidOperationException($"{nameof(config)} must not be null!");

            var builder = new UriBuilder
            {
                Scheme = config.Scheme,
                Host = config.Host,
                Port = config.Port,
                UserName = config.User,
                Password = config.Password
            };

            var collectionName = type.NameForCollection();
            return new MongoDataStoreClient(builder.Uri, config.Database, collectionName);
        }

        public MongoDbConfig GetConfig() => _mongoDbConfig ?? _mongoDbConfigOptions!.Value;
    }
}
