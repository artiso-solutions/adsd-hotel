
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.NoSql;
using artiso.AdsdHotel.Red.Persistence.Configuration;
using artiso.AdsdHotel.Red.Persistence.Entities;

namespace artiso.AdsdHotel.Red.Persistence
{
    public class RoomPriceService : IRoomPriceService
    {
        private readonly IDataStoreClient _dataStoreClientRoomType;
        private readonly IDataStoreClient _dataStoreClientRoomRate;

        public RoomPriceService(MongoDBClientFactory mongoDbClientFactory)
        {
            _dataStoreClientRoomType = mongoDbClientFactory.GetClient(typeof(RoomType));
            _dataStoreClientRoomRate = mongoDbClientFactory.GetClient(typeof(RoomRate));

            var repopulate = true;
            if (repopulate)
            {
                _dataStoreClientRoomType.InsertOneAsync(new RoomType(Guid.NewGuid(), "BedNBreakfast", new[]
                {
                    new RateItem(Guid.NewGuid(), 50),
                    new RateItem(Guid.NewGuid(), 15)
                }, new ConfirmationDetails()
                {
                    CancellationFee = new CancellationFee
                    {
                        DeadLine = DateTime.Now,
                        FeeInPercentage = 5
                    }
                }));
                _dataStoreClientRoomType.InsertOneAsync(new RoomType(Guid.NewGuid(), "Honeymoon", new[]
                {
                    new RateItem(Guid.NewGuid(), 500),
                    new RateItem(Guid.NewGuid(), 35),
                    new RateItem(Guid.NewGuid(), 50)
                }, new ConfirmationDetails()
                {
                    CancellationFee = new CancellationFee
                    {
                        DeadLine = DateTime.Now,
                        FeeInPercentage = 3
                    }
                }));
            }
        }

        public async Task<List<RateItem>> GetRoomRatesByRoomType(string roomType)
        {
            if (string.IsNullOrEmpty(roomType)) throw new ArgumentException(nameof(roomType));

            var find = await _dataStoreClientRoomType.GetAsync<RoomType>(ExpressionCombinationOperator.And, type => type.Type.Equals(roomType));
            return find?.Rates ?? new List<RateItem>();
        }

        public async Task<RoomRate> InputRoomRates(string orderId, DateTime startDate, DateTime endDate, IEnumerable<RateItem> enumerable)
        {
            var roomRate = new RoomRate(orderId, startDate, endDate, enumerable);
            await _dataStoreClientRoomRate.InsertOneAsync(roomRate)!;
            return roomRate;
        }
    }
}
