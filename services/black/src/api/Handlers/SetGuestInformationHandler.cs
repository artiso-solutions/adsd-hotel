using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Api.DatabaseModel;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Black.Events;
using artiso.AdsdHotel.ITOps.NoSql;
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
            var record = new GuestInformationRecord(message.OrderId, message.GuestInformation);
            await _dataStoreClient.AddOrUpdateAsync(record, gir => gir.OrderId == message.OrderId);
            _logger.LogInformation($"Handled command for order {message.OrderId}");
            await context.Publish(new GuestInformationSet(message.OrderId, message.GuestInformation));
        }
    }
}
