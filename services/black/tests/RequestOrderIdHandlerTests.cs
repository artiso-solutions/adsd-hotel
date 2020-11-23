using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Api.DatabaseModel;
using artiso.AdsdHotel.Black.Api.Handlers;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Infrastructure.DataStorage;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus.Testing;
using Xunit;
using Xunit.Sdk;

namespace artiso.AdsdHotel.Black.Tests
{
    public class RequestOrderIdHandlerTests
    {
        [Fact]
        public async Task OrderIdRequestShouldFindNoMatchForFirstName()
        {
            var loggerMock = new Mock<ILogger<RequestOrderIdHandler>>();
            var dataStoreMock = new Mock<IDataStoreClient>();
            var handler = new RequestOrderIdHandler(dataStoreMock.Object, loggerMock.Object);
            var context = new TestableMessageHandlerContext();

            dataStoreMock.Setup(m => m.GetAllAsync(It.IsAny<Expression<Func<GuestInformationRecord, bool>>>()))
                .Returns(Task.FromResult(new List<GuestInformationRecord>()));

            OrderIdRequest message = new("Thomas", null, null);
            await handler.Handle(message, context)
                    .ConfigureAwait(false);

            Assert.Single(context.RepliedMessages);
            var publishedEvent = context.RepliedMessages[0].Message;
            if (publishedEvent is OrderIdRespone oir)
            {
                Assert.True(oir.OrderIds == null || !oir.OrderIds.Any());
            }
            else
            {
                throw new XunitException($"Event is not of type '{typeof(GuestInformationResponse).FullName}'");
            }
        }

        [Fact]
        public async Task OrderIdRequestShouldFindAMatchForFirstName()
        {
            var loggerMock = new Mock<ILogger<RequestOrderIdHandler>>();
            var dataStoreMock = new Mock<IDataStoreClient>();
            var handler = new RequestOrderIdHandler(dataStoreMock.Object, loggerMock.Object);
            var context = new TestableMessageHandlerContext();

            GuestInformationRecord guestInformation = new GuestInformationRecord(Guid.NewGuid(), new("Thomas", "last", "mail"));
            dataStoreMock.Setup(m => m.GetAllAsync(It.IsAny<Expression<Func<GuestInformationRecord, bool>>>()))
                .Returns(Task.FromResult(new List<GuestInformationRecord> { guestInformation }));

            OrderIdRequest message = new("Thomas", null, null);
            await handler.Handle(message, context)
                    .ConfigureAwait(false);

            Assert.Single(context.RepliedMessages);
            var publishedEvent = context.RepliedMessages[0].Message;
            if (publishedEvent is OrderIdRespone oir)
            {
                Assert.NotNull(oir.OrderIds);
                Assert.NotEmpty(oir.OrderIds);
                Assert.Equal(guestInformation.OrderId, oir.OrderIds.First());
            }
            else
            {
                throw new XunitException($"Event is not of type '{typeof(GuestInformationResponse).FullName}'");
            }
        }
    }
}
