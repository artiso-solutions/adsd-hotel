using System.Collections.Generic;
using System.Threading.Tasks;
using artiso.AdsdHotel.Red.Data.Entities;

namespace artiso.AdsdHotel.Red.Data
{
    public interface IRoomPriceService
    {
        Task<List<Rate>> GetRoomRatesByRoomType(string roomType);
    }
}