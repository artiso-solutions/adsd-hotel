using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.Red.Api;
using artiso.AdsdHotel.Red.Data;
using Grpc.Core;

namespace artiso.AdsdHotel.Red.Service.Service
{
    public class RatesService : Rates.RatesBase
    {
        readonly IRoomPriceService _RoomPriceService = new RoomPriceService();

        public override Task<GetRoomRatesByRoomTypeReply> GetRoomRatesByRoomType(GetRoomRatesByRoomTypeRequest request,
            ServerCallContext context)
        {
            var roomRatesByRoomType = _RoomPriceService.GetRoomRatesByRoomType(request.RoomType);
            var roomRates= roomRatesByRoomType?.ConvertAll(rate => new RoomRate()
            {
                Name = rate.Name,
                Price = rate.Price
            });

            return Task.FromResult(new GetRoomRatesByRoomTypeReply
            {
                RoomRates = { roomRates }
            });
        }

        public override Task<InputRoomRatesReply> InputRoomRates(InputRoomRatesRequest request, ServerCallContext context)
        {
            return Task.FromResult(new InputRoomRatesReply
            {
                Message = request.Name
            });
        }
    }
}