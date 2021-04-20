using System;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;
using artiso.AdsdHotel.Red.Api.Configuration;
using artiso.AdsdHotel.Red.Contracts;
using artiso.AdsdHotel.Red.Contracts.Grpc;
using artiso.AdsdHotel.Red.Events;
using artiso.AdsdHotel.Red.Persistence;
using artiso.AdsdHotel.Red.Persistence.Entities;
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
                request.StartDate.ToDateTime(), request.EndDate.ToDateTime(),
                request.RateItems.Select(rate => new RateItemEntity(new Guid(rate.Id), rate.Price)));

            await NotifyOrderRateSelected(request.OrderId, rates);
        }

        private async Task<Rate> GetRatesForRequest(InputRoomRatesRequest request)
        {
            decimal price = 0;
            decimal cancellationFee = 0;
            foreach (var task in request.RateItems.Select(async rate =>
                await _roomRepository.GetRoomTypeById<RoomTypeEntity>(rate.Id)))
            {
                var roomType = await task;
                if (roomType is null) continue;

                price += (decimal)roomType.Price;
                cancellationFee += (decimal)(roomType.ConfirmationDetailsEntity.CancellationFeeEntity.FeeInPercentage / 100f *
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

        private async Task NotifyOrderRateSelected(string orderId, Rate rates)
        {
            await _holder.EndpointReady;
            await _holder.Endpoint.Publish(new OrderRateSelected(orderId, rates));
        }
    }
}
