using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Communication.Abstraction;
using artiso.AdsdHotel.Red.Contracts;
using artiso.AdsdHotel.Red.Contracts.Grpc;
using artiso.AdsdHotel.Red.Events;
using artiso.AdsdHotel.Red.Persistence;

namespace artiso.AdsdHotel.Red.Api.Handlers
{
    public class RoomSelectedHandler
    {
        private readonly IChannel _channel;
        private readonly IRoomRepository _roomRepository;

        public RoomSelectedHandler(IChannel channel, IRoomRepository roomRepository)
        {
            _channel = channel;
            _roomRepository = roomRepository;
        }

        public async Task Handle(InputRoomRatesRequest request)
        {
            var rates = await GetRatesForRequest(request);

            await _roomRepository.InputRoomRates(request.OrderId,
                request.StartDate.ToDateTime(), request.EndDate.ToDateTime(), request.RoomRateId);

            await NotifyOrderRateSelected(request.OrderId, rates);
        }

        private async Task<Rate> GetRatesForRequest(InputRoomRatesRequest request)
        {

            var roomTypeEntity = await _roomRepository.GetRoomTypeById(request.RoomRateId);

            var price = roomTypeEntity?.Rates.Select(rate => rate.Price).Sum() ?? 0;

            var cancellationFee = roomTypeEntity?.Rates.Select(rate =>
                    roomTypeEntity.ConfirmationDetailsEntity.CancellationFeeEntity.FeeInPercentage / 100f * rate.Price)
                .Sum() ?? 0;

            var days = (request.EndDate.ToDateTime() - request.StartDate.ToDateTime()).Days;
            price *= days;
            cancellationFee *= days;

            return new Rate(new Price
            {
                Amount = (decimal)price,
                CancellationFee = (decimal)cancellationFee
            });
        }

        private async Task NotifyOrderRateSelected(string orderId, Rate rates)
        {
            await _channel.Publish(new OrderRateSelected(orderId, rates));
        }
    }
}
