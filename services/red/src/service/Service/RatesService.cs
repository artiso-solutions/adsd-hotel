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
        private readonly IRoomPriceService _roomPriceService = new RoomPriceService();

        public override async Task<GetRoomRatesByRoomTypeReply> GetRoomRatesByRoomType(GetRoomRatesByRoomTypeRequest request,
            ServerCallContext context)
        {
            var roomRatesByRoomType = await _roomPriceService.GetRoomRatesByRoomType(request.RoomType);
            var roomRates= roomRatesByRoomType.ConvertAll(rate => new RoomRate()
            {
                Name = rate.Name,
                Price = rate.Price
            });

            return await Task.FromResult(new GetRoomRatesByRoomTypeReply
            {
                RoomRates = { roomRates },
                ConfirmationDetails = new ConfirmationDetails(),
                TotalPrice = roomRates.Select(rate => rate.Price).Sum()
            });
        }

        public override Task<InputRoomRatesReply> InputRoomRates(InputRoomRatesRequest request, ServerCallContext context)
        {
            return Task.FromResult(new InputRoomRatesReply
            {
                Success = true
            });
        }
    }
}