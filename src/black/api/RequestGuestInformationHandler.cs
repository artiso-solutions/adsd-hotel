using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Black.Contracts;
using NServiceBus;

namespace artiso.AdsdHotel.Black.Api
{
    public class RequestGuestInformationHandler : IHandleMessages<RequestGuestInformation>
    {
        public async Task Handle(RequestGuestInformation message, IMessageHandlerContext context)
        {
            // Todo retrieve from storage
            await context.Reply(new GuestInformationResponse {GuestInformation = new GuestInformation { FirstName = "test"} });
        }
    }
}
