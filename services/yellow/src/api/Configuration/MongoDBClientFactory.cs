using System;
using artiso.AdsdHotel.ITOps.NoSql;
using artiso.AdsdHotel.Yellow.Api.Extensions;

namespace artiso.AdsdHotel.Yellow.Api.Configuration
{
    public class MongoDBClientFactory
    {
        private readonly MongoDbConfig _config;

        public MongoDBClientFactory(MongoDbConfig config)
        {
            _config = config;
        }
        
        public virtual IDataStoreClient GetClient(Type type)
        {
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
