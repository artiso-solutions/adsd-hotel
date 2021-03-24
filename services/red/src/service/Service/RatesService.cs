using System;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Communication.Abstraction;
using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;
using artiso.AdsdHotel.Red.Contracts;
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
            _channel = NServiceBusChannelFactory.Create("Red.InputRoomRates", "");
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

        public override Task<InputRoomRatesReply> InputRoomRates(InputRoomRatesRequest request,
            ServerCallContext context)
        {
            try
            {
                _roomPriceService.InputRoomRates(request.OrderId,
                    request.StartDate.ToDateTime(), request.EndDate.ToDateTime(),
                    request.RateItems.Select(rate => new RateItem(new Guid(rate.Id), rate.Price)));
                _channel.Publish(new )
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
