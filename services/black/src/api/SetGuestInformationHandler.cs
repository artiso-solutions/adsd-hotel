using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Black.Events;
using NServiceBus;

namespace artiso.AdsdHotel.Black.Api
{
    public class SetGuestInformationHandler : IHandleMessages<SetGuestInformation>
    {
        public async Task Handle(SetGuestInformation message, IMessageHandlerContext context)
        {
            // ToDo save somewhere
            await context.Publish(new GuestInformationSet(message.OrderId, message.GuestInformation)).ConfigureAwait(false);
        }
    }
}
