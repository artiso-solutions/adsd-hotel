using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Api.DatabaseModel;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Black.Events;
using artiso.AdsdHotel.Infrastructure.DataStorage;
using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using NServiceBus;
using NServiceBus.Extensibility;

namespace artiso.AdsdHotel.Black.Api
{
    public class SetGuestInformationHandler : IHandleMessages<SetGuestInformation>
    {
        private readonly ILogger<SetGuestInformationHandler> logger;

        private readonly IDataStoreClient dataStoreClient;

        public SetGuestInformationHandler(IDataStoreClient dataStoreClient, ILogger<SetGuestInformationHandler> logger)
        {
            this.dataStoreClient = dataStoreClient;
            this.logger = logger;
        }

        public async Task Handle(SetGuestInformation message, IMessageHandlerContext context)
        {
            var record = new GuestInformationRecord { OrderId = message.OrderId, GuestInformation = message.GuestInformation };
            await dataStoreClient.AddOrUpdate(record).ConfigureAwait(false);
            this.logger.LogInformation($"Handled command for order {message.OrderId}");
            await context.Publish(new GuestInformationSet(message.OrderId, message.GuestInformation)).ConfigureAwait(false);
        }
    }
}
