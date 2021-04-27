using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;
using artiso.AdsdHotel.Red.Api.Configuration;
using artiso.AdsdHotel.Red.Contracts;
using artiso.AdsdHotel.Red.Contracts.Grpc;
using artiso.AdsdHotel.Red.Events;
using artiso.AdsdHotel.Red.Persistence;
using NServiceBus;

namespace artiso.AdsdHotel.Red.Api.Handlers
{
    internal class RoomSelectedHandler
    {
        private readonly EndpointHolder _holder;
        private readonly IRoomRepository _roomRepository;

        public static RoomSelectedHandler Create(IRoomRepository roomRepository)
        {
            var busConfiguration = AppSettingsHelper.GetSettings<RabbitMqConfig>();
            var config = NServiceBusEndpointConfigurationFactory.Create("Red.Api", busConfiguration.ToString(), useCallbacks: false);
            var holder = new EndpointHolder(Endpoint.Start(config));
            return new RoomSelectedHandler(holder, roomRepository);
        }

        private RoomSelectedHandler(EndpointHolder holder, IRoomRepository roomRepository)
        {
            _holder = holder;
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

            float price = roomTypeEntity?.Rates.Select(rate => rate.Price).Sum() ?? 0;

            float cancellationFee = roomTypeEntity?.Rates.Select(rate =>
                    roomTypeEntity.ConfirmationDetailsEntity.CancellationFeeEntity.FeeInPercentage / 100f * rate.Price)
                .Sum() ?? 0;

            var days = (request.EndDate.ToDateTime() - request.StartDate.ToDateTime()).Days;
            price *= days;
            cancellationFee *= days;

            return new Rate(new Price
            {
                Amount = (decimal) price,
                CancellationFee = (decimal) cancellationFee
            });
        }

        private async Task NotifyOrderRateSelected(string orderId, Rate rates)
        {
            await _holder.EndpointReady;
            await _holder.Endpoint.Publish(new OrderRateSelected(orderId, rates));
        }
    }
}
