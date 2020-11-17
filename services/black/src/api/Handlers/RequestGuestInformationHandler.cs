using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Api.DatabaseModel;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Infrastructure.DataStorage;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace artiso.AdsdHotel.Black.Api.Handlers
{
    public class RequestGuestInformationHandler : IHandleMessages<GuestInformationRequest>
    {
        private readonly IDataStoreClient _dataStoreClient;
        private readonly ILogger<RequestGuestInformationHandler> _logger;

        public RequestGuestInformationHandler(IDataStoreClient dataStoreClient, ILogger<RequestGuestInformationHandler> logger)
        {
            _dataStoreClient = dataStoreClient;
            _logger = logger;
        }

        public async Task Handle(GuestInformationRequest message, IMessageHandlerContext context)
        {
            var result = await _dataStoreClient.Get<GuestInformationRecord?>(r => r!.OrderId == message.OrderId);
            if (result == null)
            {
                _logger.LogWarning($"No matching entry found for order '{message.OrderId}'.");
                await context.Reply(new GuestInformationResponse { Error = "Not found" });
            }
            else
            {
                await context.Reply(new GuestInformationResponse { GuestInformation = result.GuestInformation });
            }
        }
    }
}
