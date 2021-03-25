using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using artiso.AdsdHotel.Red.Persistence.Entities;

namespace artiso.AdsdHotel.Red.Persistence
{
    public interface IRoomPriceService
    {
        Task<List<RateItem>> GetRoomRatesByRoomType(string roomType);

        Task<RoomRate> InputRoomRates(string orderId, DateTime startDate, DateTime endDate,
            IEnumerable<RateItem> enumerable);

        Task<RoomType?> GetRoomTypeById<TResult>(string rateId);
    }
}
