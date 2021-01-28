using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Api.Controllers;
using artiso.AdsdHotel.Black.Api.DatabaseModel;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Infrastructure.DataStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace artiso.AdsdHotel.Black.Tests
{
    public class GuestInformationControllerTests
    {
        [Fact]
        public async Task GuestInformationRequestReturnsBadRequestWhenRequestIsMalformed()
        {
            Mock<ILogger<GuestInformationController>> loggerMock = new();
            Mock<IDataStoreClient> dataStoreMock = new();
            GuestInformationController controller = new(dataStoreMock.Object, loggerMock.Object);

            var result = await controller.Get(null).ConfigureAwait(false);

            Assert.IsType<BadRequestResult>(result.Result);
        }


        [Fact]
        public async Task GuestInformationRequestReturnsNotFoundWhenOrderWasNotFound()
        {
            Mock<ILogger<GuestInformationController>> loggerMock = new();
            Mock<IDataStoreClient> dataStoreMock = new();
            GuestInformationController controller = new(dataStoreMock.Object, loggerMock.Object);

            dataStoreMock.Setup(m => m.GetAsync<GuestInformationRecord>(It.IsAny<ExpressionCombinationOperator>(), It.IsAny<Expression<Func<GuestInformationRecord, bool>>>()))
                .Returns(Task.FromResult<GuestInformationRecord>(null!));

            var result = await controller.Get(Guid.NewGuid()).ConfigureAwait(false);
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GuestInformationRequestReturnsCorrectInformationForValidOrderId()
        {
            Mock<ILogger<GuestInformationController>> loggerMock = new();
            Mock<IDataStoreClient> dataStoreMock = new();
            GuestInformationController controller = new(dataStoreMock.Object, loggerMock.Object);

            var orderId = Guid.NewGuid();
            GuestInformationRecord guestInformation = new(orderId, new("first", "last", "mail"));
            dataStoreMock.Setup(m => m.GetAsync<GuestInformationRecord>(It.IsAny<ExpressionCombinationOperator>(), It.IsAny<Expression<Func<GuestInformationRecord, bool>>>()))
                .Returns(Task.FromResult(guestInformation));

            GuestInformationRequest message = new(guestInformation.OrderId);
            var result = await controller.Get(orderId).ConfigureAwait(false);
            var actualResult = result.Value;

            Assert.IsType<GuestInformationResponse>(actualResult);
            Assert.Equal(guestInformation.GuestInformation, actualResult.GuestInformation);
        }


    }
}
