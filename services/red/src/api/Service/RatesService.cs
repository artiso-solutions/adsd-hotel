using System;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.Red.Api.Handlers;
using artiso.AdsdHotel.Red.Contracts.Grpc;
using artiso.AdsdHotel.Red.Persistence;
using Grpc.Core;

namespace artiso.AdsdHotel.Red.Api.Service
{
    public sealed class RatesService : Rates.RatesBase
    {
        private readonly IRoomPriceService _roomPriceService;
        private readonly RoomSelectedHandler _roomSelectedHandler;

        public RatesService(IRoomPriceService roomPriceService)
        {
            _roomPriceService = roomPriceService;
            _roomSelectedHandler = RoomSelectedHandler.Create(roomPriceService);
        }

        public override async Task<GetRoomRatesByRoomTypeReply> GetRoomRatesByRoomType(GetRoomRatesByRoomTypeRequest request, ServerCallContext context)
        {
            var rateItems = await _roomPriceService.GetRoomRatesByRoomType(request.RoomType);

            var grpcRateItems = rateItems.Select(x => new RoomItem
            {
                Id = x.Id.ToString(),
                Price = x.Price,
            });

            return new GetRoomRatesByRoomTypeReply
            {
                RoomRates =
                {
                    new Contracts.Grpc.RoomRate
                    {
                        Id = "rate1",
                        CancellationFee = new Contracts.Grpc.CancellationFee()
                        {
                            DeadLine = new Date(DateTime.Now + new TimeSpan(14, 0,0,0)),
                            FeeInPercentage = 5
                        },
                        RateItems = { grpcRateItems },
                        TotalPrice = rateItems.Select(rate => rate.Price).Sum(),
                    }
                }
            };
        }

        public override async Task<InputRoomRatesReply> InputRoomRates(
            InputRoomRatesRequest request,
            ServerCallContext context)
        {
            try
            {
                await _roomSelectedHandler.Handle(request);
                return new InputRoomRatesReply { Success = true };
            }
            catch (Exception ex)
            {
                return new InputRoomRatesReply
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
