using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Api;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Black.Messages;
using NServiceBus.Testing;
using Xunit;

namespace artiso.AdsdHotel.Black.Tests
{
    public class SetGuestInformationHandlerTests
    {
        [Fact]
        public async Task ShouldReplyWithResponseMessage()
        {
            var handler = new SetGuestInformationHandler();
            var context = new TestableMessageHandlerContext();

            SetGuestInformation message = new SetGuestInformation { OrderId = Guid.NewGuid(), GuestInformation = new Contracts.GuestInformation()};
            await handler.Handle(message, context)
                .ConfigureAwait(false);

            Assert.Single(context.PublishedMessages);
            var publishedEvent = context.PublishedMessages[0].Message;
            Assert.IsType<GuestInformationSet>(publishedEvent);
            var gis = publishedEvent as GuestInformationSet;
            Assert.Equal(message.OrderId, gis.OrderId);
            Assert.NotNull(gis.GuestInformation);
            // ToDo check all properties
        }
    }
}
