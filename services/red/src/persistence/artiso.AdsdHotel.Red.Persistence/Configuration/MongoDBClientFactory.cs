using System;
using artiso.AdsdHotel.ITOps.NoSql;
using artiso.AdsdHotel.Red.Persistence.Extensions;

namespace artiso.AdsdHotel.Red.Persistence.Configuration
{
    public class MongoDBClientFactory
    {
        private readonly MongoDbConfig _config;

        public MongoDBClientFactory(MongoDbConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }
        
        public virtual IDataStoreClient GetClient(Type type)
        {
            if (_config.Database is null) throw new InvalidOperationException($"{nameof(_config)} must not be null!");

            var auth = string.Empty;

            if (_config.User.HasData() && _config.Password.HasData()) 
                auth = $"{_config.User}:{_config.Password}@";

            var builder = new UriBuilder
            {
                Scheme = _config.Scheme,
                Host = $"{auth}{_config.Host}",
                Port = _config.Port
            };
                
            var collectionName = type.NameForCollection();
            
            return new MongoDataStoreClient(builder.Uri, _config.Database, collectionName);
        }
    }
}
