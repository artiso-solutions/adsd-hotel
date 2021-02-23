using System;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using artiso.AdsdHotel.Red.Data.Entities;

namespace artiso.AdsdHotel.Red.Data
{
    public class RoomPriceService : IRoomPriceService
    {
        private static readonly MongoClient DbClient = new MongoClient("mongodb://root:example@127.0.0.1:27017");
        private readonly IMongoCollection<RoomType>? _roomTypesCollection;
        private readonly IMongoCollection<RoomRate>? _roomRatesCollection;

        public RoomPriceService(bool repopulate = true)
        {
            IMongoDatabase mongoDatabase = DbClient.GetDatabase("red");
            _roomTypesCollection = mongoDatabase.GetCollection<RoomType>("roomtypes");
            _roomRatesCollection = mongoDatabase.GetCollection<RoomRate>("roomrates");
            if (repopulate)
            {
                _roomTypesCollection.DeleteMany(type => true);
                _roomTypesCollection.InsertMany(new[]
                {
                    new RoomType(Guid.NewGuid(), "BedNBreakfast", new[]
                    {
                        new Rate(Guid.NewGuid(), 50),
                        new Rate(Guid.NewGuid(), 15)
                    }),
                    new RoomType(Guid.NewGuid(), "Honeymoon" , new[]
                    {
                        new Rate(Guid.NewGuid(), 500),
                        new Rate(Guid.NewGuid(), 35),
                        new Rate(Guid.NewGuid(), 50)
                    })
                });
            }
        }

        public async Task<List<Rate>> GetRoomRatesByRoomType(string roomType)
        {
            if (string.IsNullOrEmpty(roomType)) throw new ArgumentException(nameof(roomType));

            var find = await _roomTypesCollection.FindAsync(type => type.Type.Equals(roomType));
            return find?.Single().Rates ?? new List<Rate>();
        }

        public async void InputRoomRates(string orderId, DateTime startDate, DateTime endDate, IEnumerable<Rate> enumerable)
        {
            await _roomRatesCollection?.InsertOneAsync(new RoomRate(orderId, startDate, endDate, enumerable))!;
        }
    }
}
