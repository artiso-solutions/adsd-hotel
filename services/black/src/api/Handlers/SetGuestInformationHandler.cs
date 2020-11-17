using System.Threading;
using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Api.DatabaseModel;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Black.Contracts.Validation;
using artiso.AdsdHotel.Black.Events;
using artiso.AdsdHotel.Infrastructure.DataStorage;
using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace artiso.AdsdHotel.Black.Api.Handlers
{
    public class SetGuestInformationHandler : IHandleMessages<SetGuestInformation>
    {
        private readonly ILogger<SetGuestInformationHandler> _logger;
        private readonly IDataStoreClient _dataStoreClient;

        public SetGuestInformationHandler(IDataStoreClient dataStoreClient, ILogger<SetGuestInformationHandler> logger)
        {
            _dataStoreClient = dataStoreClient;
            _logger = logger;
        }

        public async Task Handle(SetGuestInformation message, IMessageHandlerContext context)
        {
            // ToDo what if the sent message does contain invalid data? Move to error queue?
            if (!GuestInformationValidator.IsValid(message.GuestInformation))
                return;

            var record = new GuestInformationRecord(message.OrderId, message.GuestInformation);
            await _dataStoreClient.AddOrUpdate(record, gir => gir.OrderId == message.OrderId);
            _logger.LogInformation($"Handled command for order {message.OrderId}");
            await context.Publish(new GuestInformationSet(message.OrderId, message.GuestInformation));
        }
    }
}
