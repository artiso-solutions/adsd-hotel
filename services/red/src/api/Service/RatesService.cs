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
        private readonly RoomSelectedHandler _roomSelectedHandler;
        private readonly GetRoomRatesByRoomTypeHandler _getRoomRateByRoomTypeTypeHandler;

        public RatesService(RoomSelectedHandler roomSelectedHandler, GetRoomRatesByRoomTypeHandler getRoomRatesByRoomTypeHandler)
        {
            _roomSelectedHandler = roomSelectedHandler;
            _getRoomRateByRoomTypeTypeHandler = getRoomRatesByRoomTypeHandler;
        }

        public override async Task<GetRoomRatesByRoomTypeReply> GetRoomRatesByRoomType(
            GetRoomRatesByRoomTypeRequest request,
            ServerCallContext context)
        {
            var rateItemsEntities = await _getRoomRateByRoomTypeTypeHandler.Handle(request.RoomType);

            var rateItems = rateItemsEntities.Select(x => new RoomItem
            {
                Id = x.Id.ToString(),
                Price = x.Price,
            });

            return new GetRoomRatesByRoomTypeReply
            {
                RoomRates =
                {
                    new RoomRate
                    {
                        Id = "rate1",
                        CancellationFee = new CancellationFee
                        {
                            DeadLine = new Date(DateTime.Now + new TimeSpan(14, 0, 0, 0)),
                            FeeInPercentage = 5
                        },
                        RateItems = {rateItems},
                        TotalPrice = rateItemsEntities.Select(rate => rate.Price).Sum(),
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
