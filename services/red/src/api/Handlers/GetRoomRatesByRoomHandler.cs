using System.Collections.Generic;
using System.Threading.Tasks;
using artiso.AdsdHotel.Red.Persistence;
using artiso.AdsdHotel.Red.Persistence.Entities;

namespace artiso.AdsdHotel.Red.Api.Handlers
{
    public class GetRoomRatesByRoomTypeHandler
    {
        private readonly IRoomRepository _roomRepository;

        public GetRoomRatesByRoomTypeHandler(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public Task<List<RateItemEntity>> Handle(string roomType) => _roomRepository.GetRoomRatesByRoomType(roomType);
    }
}
