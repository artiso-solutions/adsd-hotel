using MongoDB.Driver;

namespace artiso.AdsdHotel.Red.Data
{
    public abstract class BaseDatabaseService
    {
        private static readonly MongoClient _dbClient = new MongoClient("mongodb://root:example@127.0.0.1:27017");
        protected readonly IMongoDatabase _mongoDatabase;

        protected BaseDatabaseService()
        {
            _mongoDatabase = _dbClient.GetDatabase("red");
        }
    }
}
