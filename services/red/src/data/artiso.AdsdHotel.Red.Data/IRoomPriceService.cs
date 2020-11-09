using System.Collections.Generic;
using artiso.AdsdHotel.Red.Data.Entities;

namespace artiso.AdsdHotel.Red.Data
{
    public interface IRoomPriceService
    {
        List<Rate>? GetRoomRatesByRoomType(string roomType);
    }
}