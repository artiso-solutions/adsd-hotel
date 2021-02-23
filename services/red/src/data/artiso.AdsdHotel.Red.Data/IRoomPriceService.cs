using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using artiso.AdsdHotel.Red.Persistence.Entities;

namespace artiso.AdsdHotel.Red.Persistence
{
    public interface IRoomPriceService
    {
        Task<List<Rate>> GetRoomRatesByRoomType(string roomType);

        void InputRoomRates(string orderId, DateTime startDate, DateTime endDate, IEnumerable<Rate> enumerable);
    }
}
