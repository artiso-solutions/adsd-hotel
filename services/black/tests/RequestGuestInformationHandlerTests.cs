using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Api.DatabaseModel;
using artiso.AdsdHotel.Black.Api.Handlers;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Black.Events;
using artiso.AdsdHotel.Infrastructure.DataStorage;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus.Testing;
using NuGet.Frameworks;
using Xunit;
using Xunit.Sdk;

namespace artiso.AdsdHotel.Black.Tests
{
    public class RequestGuestInformationHandlerTests
    {
        [Fact]
        public async Task ShouldReplyWithResponseMessage()
        {
            var loggerMock = new Mock<ILogger<RequestGuestInformationHandler>>();
            var dataStoreMock = new Mock<IDataStoreClient>();
            var handler = new RequestGuestInformationHandler(dataStoreMock.Object, loggerMock.Object);
            var context = new TestableMessageHandlerContext();

            GuestInformationRecord guestInformation = new GuestInformationRecord(Guid.NewGuid(), new("first", "last", "mail"));
            dataStoreMock.Setup(m => m.Get(It.IsAny<Expression<Func<GuestInformationRecord, bool>>>()))
                .Returns(Task.FromResult(guestInformation));

            GuestInformationRequest message = new(guestInformation.OrderId);
            await handler.Handle(message, context)
                    .ConfigureAwait(false);

            Assert.Single(context.RepliedMessages);
            var publishedEvent = context.RepliedMessages[0].Message;
            if (publishedEvent is GuestInformationResponse gir)
            {
                Assert.NotNull(gir.GuestInformation);
                Assert.Equal(guestInformation.GuestInformation, gir.GuestInformation);
            }
            else
            {
                throw new XunitException($"Event is not of type '{typeof(GuestInformationResponse).FullName}'");
            }
        }

        [Fact]
        public async Task ShouldReplyWithErrorMessage()
        {
            var loggerMock = new Mock<ILogger<RequestGuestInformationHandler>>();
            var dataStoreMock = new Mock<IDataStoreClient>();
            var handler = new RequestGuestInformationHandler(dataStoreMock.Object, loggerMock.Object);
            var context = new TestableMessageHandlerContext();

            GuestInformationRecord guestInformation = new GuestInformationRecord(Guid.NewGuid(), new("first", "last", "mail"));
            //dataStoreMock.Setup(m => m.Get(It.IsAny<Expression<Func<GuestInformationRecord, bool>>>()))
            //    .Returns(Task.FromResult(guestInformation));

            GuestInformationRequest message = new(guestInformation.OrderId);
            await handler.Handle(message, context)
                    .ConfigureAwait(false);

            Assert.Single(context.RepliedMessages);
            var publishedEvent = context.RepliedMessages[0].Message;
            if (publishedEvent is GuestInformationResponse gir)
            {
                Assert.NotNull(gir.Error);
                Assert.Null(gir.GuestInformation);
            }
            else
            {
                throw new XunitException($"Event is not of type '{typeof(GuestInformationResponse).FullName}'");
            }
        }
    }
}
