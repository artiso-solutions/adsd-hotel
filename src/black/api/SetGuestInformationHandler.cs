using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Black.Messages;
using NServiceBus;

namespace artiso.AdsdHotel.Black.Api
{
    public class SetGuestInformationHandler : IHandleMessages<SetGuestInformation>
    {
        public async Task Handle(SetGuestInformation message, IMessageHandlerContext context)
        {
            // ToDo save somewhere

            await context.Publish(new GuestInformationSet { OrderId = message.OrderId, GuestInformation = message.GuestInformation }).ConfigureAwait(false);
        }
    }
}
