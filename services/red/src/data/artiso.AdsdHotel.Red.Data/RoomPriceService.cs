using System;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using artiso.AdsdHotel.Red.Data.Entities;

namespace artiso.AdsdHotel.Red.Data
{
    public class RoomPriceService : BaseDatabaseService, IRoomPriceService
    {
        private readonly IMongoCollection<RoomType>? _roomTypesCollection;

        public RoomPriceService(bool repopulate = false)
        {
            _roomTypesCollection = _mongoDatabase.GetCollection<RoomType>("roomtypes");
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
                        new Rate("Breakfast", 35),
                        new Rate("Champagne", 50)
                    })
                });
            }
        }

        public async Task<List<Rate>> GetRoomRatesByRoomType(string roomType)
        {
            var find = await _roomTypesCollection.FindAsync(type => type.Name.Equals(roomType));
            return find?.First().Rates ?? new List<Rate>();
        }
    }
}