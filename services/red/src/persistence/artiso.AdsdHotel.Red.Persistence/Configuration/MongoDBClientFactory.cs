using System;
using artiso.AdsdHotel.ITOps.NoSql;

namespace artiso.AdsdHotel.Red.Persistence.Configuration
{
    public class MongoDbClientFactory
    {
        private readonly MongoDbConfig _config;

        public MongoDbClientFactory(MongoDbConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public virtual MongoDataStoreClient GetClient(Type type)
        {
            if (_config.Database is null) throw new InvalidOperationException($"{nameof(_config)} must not be null!");

            var builder = new UriBuilder
            {
                Scheme = _config.Scheme,
                Host = _config.Host,
                Port = _config.Port,
                UserName = _config.User,
                Password = _config.Password
            };

            var collectionName = type.NameForCollection();
            return new MongoDataStoreClient(builder.Uri, _config.Database, collectionName);
        }
    }
}
