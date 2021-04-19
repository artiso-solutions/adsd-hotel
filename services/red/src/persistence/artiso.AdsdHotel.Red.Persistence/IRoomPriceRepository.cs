using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using artiso.AdsdHotel.Red.Persistence.Entities;

namespace artiso.AdsdHotel.Red.Persistence
{
    public interface IRoomPriceRepository
    {
        Task<List<RateItemEntity>> GetRoomRatesByRoomType(string roomType);

        Task<RoomRateEntity> InputRoomRates(string orderId, DateTime startDate, DateTime endDate,
            IEnumerable<RateItemEntity> enumerable);

        Task<RoomTypeEntity?> GetRoomTypeById<TResult>(string rateId);
    }
}
