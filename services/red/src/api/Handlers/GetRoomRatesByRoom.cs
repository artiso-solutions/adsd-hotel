using System;
using System.Collections.Generic;
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
using CancellationFee = artiso.AdsdHotel.Red.Contracts.Grpc.CancellationFee;
using RoomRate = artiso.AdsdHotel.Red.Contracts.Grpc.RoomRate;

namespace artiso.AdsdHotel.Red.Api.Handlers
{
    internal class GetRoomRatesByRoomTypeHandler
    {
        private readonly EndpointHolder _holder;
        private readonly IRoomRepository _roomRepository;

        public static GetRoomRatesByRoomTypeHandler Create(IRoomRepository roomRepository)
        {
            var busConfiguration = AppSettingsHelper.GetSettings<RabbitMqConfig>();
            var config = NServiceBusEndpointConfigurationFactory.Create("Red.Api", busConfiguration.ToString(), useCallbacks: false);
            var holder = new EndpointHolder(Endpoint.Start(config));
            return new GetRoomRatesByRoomTypeHandler(holder, roomRepository);
        }

        private GetRoomRatesByRoomTypeHandler(EndpointHolder holder, IRoomRepository roomRepository)
        {
            _holder = holder;
            _roomRepository = roomRepository;
        }

        public Task<List<RateItemEntity>> Handle(string roomType) => _roomRepository.GetRoomRatesByRoomType(roomType);
    }
}
