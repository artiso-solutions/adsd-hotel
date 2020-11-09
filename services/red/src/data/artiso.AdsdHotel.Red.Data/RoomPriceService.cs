using MongoDB.Driver;
using System.Collections.Generic;
using artiso.AdsdHotel.Red.Data.Entities;

namespace artiso.AdsdHotel.Red.Data
{
    public class RoomPriceService : BaseDatabaseService, IRoomPriceService
    {
        private readonly IMongoCollection<RoomType>? _roomTypesCollection;

        public RoomPriceService(bool repopulate = false)
        {
            _roomTypesCollection = MongoDatabase.GetCollection<RoomType>("roomtypes");
            if (repopulate)
            {
                _roomTypesCollection.DeleteMany(type => true);
                _roomTypesCollection.InsertMany(new[]
                {
                    new RoomType("Bed & Breakfast", new[]
                    {
                        new Rate("Overnight stay", 50),
                        new Rate("Breakfast", 15)
                    }),
                    new RoomType("Honeymoon", new[]
                    {
                        new Rate("Overnight stay", 500),
                        new Rate("Breakfast", 15),
                        new Rate("Champagne", 50)
                    })
                });
            }
        }

        public List<Rate>? GetRoomRatesByRoomType(string roomType)
        {
            var roomRatesByRoomType = _roomTypesCollection?.Find(type => type.Name.Equals(roomType)).First().Rates;
            return roomRatesByRoomType;
        }
    }
}