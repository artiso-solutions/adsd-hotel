using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;

namespace artiso.AdsdHotel.ITOps.NoSql
{
    public class MongoDbConfig
    {
        static MongoDbConfig()
        {
            var conventions = new ConventionPack
            {
                new IgnoreExtraElementsConvention(true),
                new CamelCaseElementNameConvention(),
                new EnumRepresentationConvention(BsonType.String),
            };

            ConventionRegistry.Register("DefaultConventions", conventions, filter: type => true);
        }

        public string? Host { get; set; }
        public string? User { get; set; }
        public int Port { get; set; }
        public string? Password { get; set; }
        public string? Database { get; set; }
        public string? Scheme { get; set; }
    }


}
