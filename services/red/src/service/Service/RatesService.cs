using System;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.Red.Api;
using artiso.AdsdHotel.Red.Data;
using artiso.AdsdHotel.Red.Data.Entities;
using Grpc.Core;
using RoomRate = artiso.AdsdHotel.Red.Api.RoomRate;

namespace artiso.AdsdHotel.Red.Service.Service
{
    public sealed class RatesService : Rates.RatesBase
    {

        private readonly IRoomPriceService _roomPriceService;

        public RatesService()
        {
            _roomPriceService = new RoomPriceService();
        }

        public RatesService(IRoomPriceService roomPriceService)
        {
            _roomPriceService = roomPriceService;
        }

        public override async Task<GetRoomRatesByRoomTypeReply> GetRoomRatesByRoomType(
            GetRoomRatesByRoomTypeRequest request,
            ServerCallContext context)
        {
            var roomRatesByRoomType = await _roomPriceService.GetRoomRatesByRoomType(request.RoomType);
            var roomRates = roomRatesByRoomType.ConvertAll(rate => new RoomRate()
            {
                Id = rate.Id.ToString(),
                Price = rate.Price
            });

            return await Task.FromResult(new GetRoomRatesByRoomTypeReply
            {
                RoomRates = {roomRates},
                ConfirmationDetails = new ConfirmationDetails(),
                TotalPrice = roomRates.Select(rate => rate.Price).Sum()
            });
        }

        public override Task<InputRoomRatesReply> InputRoomRates(InputRoomRatesRequest request,
            ServerCallContext context)
        {
            try
            {
                _roomPriceService.InputRoomRates(request.OrderId,
                    request.StartDate.ToDateTime(), request.EndDate.ToDateTime(),
                    request.RoomRates.Select(rate => new Rate(new Guid(rate.Id), rate.Price)));
                return Task.FromResult(new InputRoomRatesReply
                {
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new InputRoomRatesReply
                {
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }
    }
}