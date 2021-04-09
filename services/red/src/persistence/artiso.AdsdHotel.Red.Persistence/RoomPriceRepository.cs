using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.NoSql;
using artiso.AdsdHotel.Red.Persistence.Configuration;
using artiso.AdsdHotel.Red.Persistence.Entities;

namespace artiso.AdsdHotel.Red.Persistence
{
    public class RoomPriceRepository : IRoomPriceRepository
    {
        private readonly IDataStoreClient _dataStoreClientRoomType;
        private readonly IDataStoreClient _dataStoreClientRoomRate;

        public RoomPriceRepository(MongoDbClientFactory mongoDbClientFactory)
        {
            _dataStoreClientRoomType = mongoDbClientFactory.GetClient(typeof(RoomType));
            _dataStoreClientRoomRate = mongoDbClientFactory.GetClient(typeof(RoomRate));

            Repopulate();
        }

        private async void Repopulate()
        {
            var allRoomTypes =
                await _dataStoreClientRoomType.GetAllAsync<RoomType>(ExpressionCombinationOperator.And, type => true);
            if (allRoomTypes.Count == 0)
            {
                await _dataStoreClientRoomType.InsertOneAsync(new RoomType(Guid.NewGuid(), "RMTY-001", new[]
                {
                    new RateItem(Guid.NewGuid(), 50),
                    new RateItem(Guid.NewGuid(), 15)
                }, new ConfirmationDetails()
                {
                    CancellationFee = new CancellationFee
                    {
                        FeeInPercentage = 5
                    }
                }));
                await _dataStoreClientRoomType.InsertOneAsync(new RoomType(Guid.NewGuid(), "RMTY-002", new[]
                {
                    new RateItem(Guid.NewGuid(), 50),
                    new RateItem(Guid.NewGuid(), 35)
                }, new ConfirmationDetails()
                {
                    CancellationFee = new CancellationFee
                    {
                        FeeInPercentage = 3
                    }
                }));
                await _dataStoreClientRoomType.InsertOneAsync(new RoomType(Guid.NewGuid(), "RMTY-Q04", new[]
                {
                    new RateItem(Guid.NewGuid(), 100),
                    new RateItem(Guid.NewGuid(), 35)
                }, new ConfirmationDetails()
                {
                    CancellationFee = new CancellationFee
                    {
                        FeeInPercentage = 3
                    }
                }));
                await _dataStoreClientRoomType.InsertOneAsync(new RoomType(Guid.NewGuid(), "RMTY-K02", new[]
                {
                    new RateItem(Guid.NewGuid(), 500),
                    new RateItem(Guid.NewGuid(), 35),
                    new RateItem(Guid.NewGuid(), 50)
                }, new ConfirmationDetails()
                {
                    CancellationFee = new CancellationFee
                    {
                        FeeInPercentage = 3
                    }
                }));
                await _dataStoreClientRoomType.InsertOneAsync(new RoomType(Guid.NewGuid(), "RMTY-F05", new[]
                {
                    new RateItem(Guid.NewGuid(), 750),
                    new RateItem(Guid.NewGuid(), 35),
                    new RateItem(Guid.NewGuid(), 100)
                }, new ConfirmationDetails()
                {
                    CancellationFee = new CancellationFee
                    {
                        FeeInPercentage = 3
                    }
                }));
            }
        }

        public async Task<List<RateItem>> GetRoomRatesByRoomType(string roomType)
        {
            if (string.IsNullOrEmpty(roomType)) throw new ArgumentNullException(nameof(roomType));

            var rateItem = await _dataStoreClientRoomType.GetAsync<RoomType>(ExpressionCombinationOperator.And, type => type.Type.Equals(roomType));
            return rateItem?.Rates ?? new List<RateItem>();
        }

        public async Task<RoomRate> InputRoomRates(string orderId, DateTime startDate, DateTime endDate, IEnumerable<RateItem> enumerable)
        {
            var roomRate = new RoomRate(orderId, startDate, endDate, enumerable);
            await _dataStoreClientRoomRate.InsertOneAsync(roomRate)!;
            return roomRate;
        }

        public async Task<RoomType?> GetRoomTypeById<TResult>(string rateId)
        {
            if (string.IsNullOrEmpty(rateId)) throw new ArgumentNullException(nameof(rateId));

            var roomType = await _dataStoreClientRoomType.GetAsync<RoomType>(ExpressionCombinationOperator.And, type => type.Id.ToString().Equals(rateId));
            return roomType;
        }
    }
}
