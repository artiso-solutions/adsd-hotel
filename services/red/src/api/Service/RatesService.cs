using System;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Communication.Abstraction;
using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;
using artiso.AdsdHotel.Red.Contracts;
using artiso.AdsdHotel.Red.Contracts.Grpc;
using artiso.AdsdHotel.Red.Events;
using artiso.AdsdHotel.Red.Persistence;
using artiso.AdsdHotel.Red.Persistence.Entities;
using Grpc.Core;

namespace artiso.AdsdHotel.Red.Api.Service
{
    public sealed class RatesService : Rates.RatesBase
    {

        private readonly IRoomPriceService _roomPriceService;
        private readonly IChannel _channel;

        public RatesService(IRoomPriceService roomPriceService)
        {
            _roomPriceService = roomPriceService ?? throw new ArgumentNullException(nameof(roomPriceService));
            _channel = NServiceBusChannelFactory.Create("Red.Api.OrderRateSelected", "Yellow.Api", "host=localhost");
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

        public override async Task<InputRoomRatesReply> InputRoomRates(InputRoomRatesRequest request,
            ServerCallContext context)
        {
            try
            {
                var rates = await GetRatesForRequest(request);

                await _roomPriceService.InputRoomRates(request.OrderId,
                    request.StartDate.ToDateTime(), request.EndDate.ToDateTime(),
                    request.RateItems.Select(rate => new Persistence.Entities.RateItem(new Guid(rate.Id), rate.Price)));
                
                //Todo get channel
                await _channel.Publish(new OrderRateSelected(request.OrderId, rates)); 

                return new InputRoomRatesReply
                {
                    Success = true
                };
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

        private async Task<Rate> GetRatesForRequest(InputRoomRatesRequest request)
        {
            decimal price = 0;
            decimal cancellationFee = 0;
            foreach (var task in request.RateItems.Select(async rate =>
                await _roomPriceService.GetRoomTypeById<RoomType>(rate.Id)))
            {
                var roomType = await task;
                if (roomType is null) continue;

                price += (decimal) roomType.Price;
                cancellationFee += (decimal) (roomType.ConfirmationDetails.CancellationFee.FeeInPercentage / 100f *
                                              roomType.Price);
            }

            var days = (request.EndDate.ToDateTime() - request.StartDate.ToDateTime()).Days;
            price *= days;
            cancellationFee *= days;

            return new Rate(new Price()
            {
                Amount = price,
                CancellationFee = cancellationFee
            });
        }
    }
}
