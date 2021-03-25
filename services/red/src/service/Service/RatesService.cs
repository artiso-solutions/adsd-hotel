using System;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Communication.Abstraction;
using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;
using artiso.AdsdHotel.Red.Contracts;
using artiso.AdsdHotel.Red.Events;
using artiso.AdsdHotel.Red.Persistence;
using artiso.AdsdHotel.Red.Persistence.Entities;
using Grpc.Core;
using ConfirmationDetails = artiso.AdsdHotel.Red.Contracts.ConfirmationDetails;

namespace artiso.AdsdHotel.Red.Api.Service
{
    public sealed class RatesService : Rates.RatesBase
    {

        private readonly IRoomPriceService _roomPriceService;
        private readonly IChannel _channel;

        public RatesService(IRoomPriceService roomPriceService)
        {
            _roomPriceService = roomPriceService ?? throw new ArgumentNullException(nameof(roomPriceService));
            _channel = NServiceBusChannelFactory.Create("Red.InputRoomRates", "test", "test");
        }

        public override async Task<GetRoomRatesByRoomTypeReply> GetRoomRatesByRoomType(GetRoomRatesByRoomTypeRequest request, ServerCallContext context)
        {
            var roomRatesByRoomType = await _roomPriceService.GetRoomRatesByRoomType(request.RoomType);
            var roomRates = roomRatesByRoomType.ConvertAll(rate => new RoomItem()
            {
                Id = rate.Id.ToString(),
                Price = rate.Price
            });
            var confirmationDetails = new ConfirmationDetails()
            {
                CancellationFee = new Contracts.CancellationFee()
                {
                    DeadLine = new Date(DateTime.Now + new TimeSpan(14, 0,0,0)),
                    FeeInPercentage = 5
                }
            };

            return new GetRoomRatesByRoomTypeReply()
            {
                RateItems = {roomRates},
                ConfirmationDetails = confirmationDetails,
                TotalPrice = roomRates.Select(rate => rate.Price).Sum()
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
                    request.RateItems.Select(rate => new RateItem(new Guid(rate.Id), rate.Price)));

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
