using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Api.DatabaseModel;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Infrastructure.DataStorage;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace artiso.AdsdHotel.Black.Api
{
    public class RequestGuestInformationHandler : IHandleMessages<GuestInformationRequest>
    {
        private readonly IDataStoreClient dataStoreClient;
        private readonly ILogger<RequestGuestInformationHandler> logger;

        public RequestGuestInformationHandler(IDataStoreClient dataStoreClient, ILogger<RequestGuestInformationHandler> logger)
        {
            this.dataStoreClient = dataStoreClient;
            this.logger = logger;
        }

        public async Task Handle(GuestInformationRequest message, IMessageHandlerContext context)
        {
            var result = await dataStoreClient.Get<GuestInformationRecord?>(r => r!.OrderId == message.OrderId).ConfigureAwait(false);
            if (result == null)
            {
                logger.LogWarning($"No matching entry found for order '{message.OrderId}'.");
                await context.Reply(new GuestInformationResponse("Not found")).ConfigureAwait(false);
            }
            else
            {
                await context.Reply(new GuestInformationResponse(result.GuestInformation)).ConfigureAwait(false);
            }
        }
    }
}
