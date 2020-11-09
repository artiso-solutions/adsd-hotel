using MongoDB.Driver;

namespace artiso.AdsdHotel.Red.Data
{
    public abstract class BaseDatabaseService
    {
        protected static readonly MongoClient DbClient = new MongoClient("mongodb://root:example@127.0.0.1:27017");
        protected readonly IMongoDatabase MongoDatabase;

        protected BaseDatabaseService()
        {
            MongoDatabase = DbClient.GetDatabase("red");
        }
    }
}
