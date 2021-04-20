using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.NoSql;
using artiso.AdsdHotel.Red.Persistence.Configuration;
using artiso.AdsdHotel.Red.Persistence.Entities;

namespace artiso.AdsdHotel.Red.Persistence
{
    public class RoomRepository : IRoomRepository
    {
        private readonly IDataStoreClient _dataStoreClientRoomType;
        private readonly IDataStoreClient _dataStoreClientRoomRate;

        public RoomRepository(MongoDbClientFactory dbClientFactory)
        {
            _dataStoreClientRoomType = dbClientFactory.GetClient(typeof(RoomTypeEntity));
            _dataStoreClientRoomRate = dbClientFactory.GetClient(typeof(RoomRateEntity));

            Repopulate();
        }

        private async void Repopulate()
        {
            var allRoomTypes =
                await _dataStoreClientRoomType.GetAllAsync<RoomTypeEntity>(ExpressionCombinationOperator.And, type => true);
            if (allRoomTypes.Count == 0)
            {
                await _dataStoreClientRoomType.InsertOneAsync(new RoomTypeEntity(Guid.NewGuid(), "RMTY-001", new[]
                {
                    new RateItemEntity(Guid.NewGuid(), 50),
                    new RateItemEntity(Guid.NewGuid(), 15)
                }, new ConfirmationDetailsEntity()
                {
                    CancellationFeeEntity = new CancellationFeeEntity
                    {
                        FeeInPercentage = 5
                    }
                }));
                await _dataStoreClientRoomType.InsertOneAsync(new RoomTypeEntity(Guid.NewGuid(), "RMTY-002", new[]
                {
                    new RateItemEntity(Guid.NewGuid(), 50),
                    new RateItemEntity(Guid.NewGuid(), 35)
                }, new ConfirmationDetailsEntity()
                {
                    CancellationFeeEntity = new CancellationFeeEntity
                    {
                        FeeInPercentage = 3
                    }
                }));
                await _dataStoreClientRoomType.InsertOneAsync(new RoomTypeEntity(Guid.NewGuid(), "RMTY-Q04", new[]
                {
                    new RateItemEntity(Guid.NewGuid(), 100),
                    new RateItemEntity(Guid.NewGuid(), 35)
                }, new ConfirmationDetailsEntity()
                {
                    CancellationFeeEntity = new CancellationFeeEntity
                    {
                        FeeInPercentage = 3
                    }
                }));
                await _dataStoreClientRoomType.InsertOneAsync(new RoomTypeEntity(Guid.NewGuid(), "RMTY-K02", new[]
                {
                    new RateItemEntity(Guid.NewGuid(), 500),
                    new RateItemEntity(Guid.NewGuid(), 35),
                    new RateItemEntity(Guid.NewGuid(), 50)
                }, new ConfirmationDetailsEntity()
                {
                    CancellationFeeEntity = new CancellationFeeEntity
                    {
                        FeeInPercentage = 3
                    }
                }));
                await _dataStoreClientRoomType.InsertOneAsync(new RoomTypeEntity(Guid.NewGuid(), "RMTY-F05", new[]
                {
                    new RateItemEntity(Guid.NewGuid(), 750),
                    new RateItemEntity(Guid.NewGuid(), 35),
                    new RateItemEntity(Guid.NewGuid(), 100)
                }, new ConfirmationDetailsEntity()
                {
                    CancellationFeeEntity = new CancellationFeeEntity
                    {
                        FeeInPercentage = 3
                    }
                }));
            }
        }

        public async Task<List<RateItemEntity>> GetRoomRatesByRoomType(string roomType)
        {
            if (string.IsNullOrEmpty(roomType)) throw new ArgumentNullException(nameof(roomType));

            var rateItem = await _dataStoreClientRoomType.GetAsync<RoomTypeEntity>(ExpressionCombinationOperator.And, type => type.Type.Equals(roomType));
            return rateItem?.Rates ?? new List<RateItemEntity>();
        }

        public async Task<RoomRateEntity> InputRoomRates(string orderId, DateTime startDate, DateTime endDate, IEnumerable<RateItemEntity> enumerable)
        {
            var roomRate = new RoomRateEntity(orderId, startDate, endDate, enumerable);
            await _dataStoreClientRoomRate.InsertOneAsync(roomRate)!;
            return roomRate;
        }

        public async Task<RoomTypeEntity?> GetRoomTypeById<TResult>(string rateId)
        {
            if (string.IsNullOrEmpty(rateId)) throw new ArgumentNullException(nameof(rateId));

            var roomType = await _dataStoreClientRoomType.GetAsync<RoomTypeEntity>(ExpressionCombinationOperator.And, type => type.Id.ToString().Equals(rateId));
            return roomType;
        }
    }
}
