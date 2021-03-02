using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Api.Handlers;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Black.Events;
using artiso.AdsdHotel.ITOps.NoSql;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus.Testing;
using Xunit;
using Xunit.Sdk;

namespace artiso.AdsdHotel.Black.Tests
{
    public class SetGuestInformationHandlerTests
    {
        [Fact]
        public async Task ShouldReplyWithResponseMessage()
        {
            var loggerMock = new Mock<ILogger<SetGuestInformationHandler>>();
            var dataStoreMock = new Mock<IDataStoreClient>();
            var handler = new SetGuestInformationHandler(dataStoreMock.Object, loggerMock.Object);
            var context = new TestableMessageHandlerContext();

            var guestInformation = new Contracts.GuestInformation("first", "last", "mail");

            SetGuestInformation message = new SetGuestInformation(Guid.NewGuid(), guestInformation);
            await handler.Handle(message, context)
                    .ConfigureAwait(false);

            Assert.Single(context.PublishedMessages);
            var publishedEvent = context.PublishedMessages[0].Message;
            if (publishedEvent is GuestInformationSet gis)
            {
                Assert.Equal(message.OrderId, gis.OrderId);
                Assert.NotNull(gis.GuestInformation);
                Assert.Equal(message.GuestInformation, gis.GuestInformation);
            }
            else
            {
                throw new XunitException($"Event is not of type '{typeof(GuestInformationSet).FullName}'");
            }
        }
    }
}
