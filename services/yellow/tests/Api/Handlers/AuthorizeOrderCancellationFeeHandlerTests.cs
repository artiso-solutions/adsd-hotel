using System.Collections.Generic;
using System.Threading.Tasks;
using artiso.AdsdHotel.Yellow.Api.Handlers;
using artiso.AdsdHotel.Yellow.Api.Services;
using artiso.AdsdHotel.Yellow.Api.Validation;
using artiso.AdsdHotel.Yellow.Contracts.Commands;
using artiso.AdsdHotel.Yellow.Contracts.Models;
using artiso.AdsdHotel.Yellow.Events;
using Moq;
using NServiceBus.Testing;
using NUnit.Framework;
using static artiso.AdsdHotel.Yellow.Tests.TestUtility.CreditCardStore;

namespace artiso.AdsdHotel.Yellow.Tests.Api.Handlers
{
    [TestFixture]
    public class AuthorizeOrderCancellationFeeHandlerTests
    {
        [Test]
        [TestCaseSource(nameof(ValidRequestTestSource))]
        public async Task ValidRequestTest(AuthorizeOrderCancellationFeeRequest request,
            Mock<IOrderService> orderService,
            Mock<ICreditCardPaymentService> paymentService)
        {
            var handler = new AuthorizeOrderCancellationFeeHandler(orderService.Object, paymentService.Object);
            var context = new TestableMessageHandlerContext();

            await handler.Handle(request, context)
                .ConfigureAwait(false);

            Assert.AreEqual(1, context.PublishedMessages.Length);
            var responseMessage = context.PublishedMessages[0].Message;
            Assert.IsInstanceOf<Response<OrderCancellationFeeAuthorizationAcquired>>(responseMessage);
            var rm = responseMessage as Response<OrderCancellationFeeAuthorizationAcquired>;
            Assert.True(rm?.IsSuccessful, $"{rm?.Exception?.Message} \n {rm?.Exception?.StackTrace}");
        }

        #region ValidRequestTestSource

        private static IEnumerable<TestCaseData> ValidRequestTestSource()
        {
            var orderService = new Mock<IOrderService>();
            var paymentService = new Mock<ICreditCardPaymentService>();

            orderService
                .Setup(s => s.FindOneById(It.Is<string>(s1 => s1 == "orderId")))
                .ReturnsAsync(new Order("orderId", new Price(0, 0)));

            paymentService
                .Setup(s => s.Authorize(It.IsAny<decimal>(), It.IsAny<CreditCard>()))
                .ReturnsAsync(new AuthorizeResult("AuthToken", null));

            yield return TestUtility.GetCaseData("BasicScenario",
                new object[]
                {
                    new AuthorizeOrderCancellationFeeRequest("orderId", new PaymentMethod(AMEX1)), 
                    orderService,
                    paymentService
                });
        }

        #endregion

        [Test]
        [TestCaseSource(nameof(InvalidRequestTestCaseSources))]
        public async Task InvalidRequestTest(AuthorizeOrderCancellationFeeRequest request,
            Mock<IOrderService> orderService,
            Mock<ICreditCardPaymentService> paymentService)
        {
            var handler = new AuthorizeOrderCancellationFeeHandler(orderService.Object, paymentService.Object);
            var context = new TestableMessageHandlerContext();

            await handler.Handle(request, context)
                .ConfigureAwait(false);

            Assert.AreEqual(1, context.PublishedMessages.Length);
            var responseMessage = context.PublishedMessages[0].Message;
            Assert.IsInstanceOf<Response<OrderCancellationFeeAuthorizationAcquired>>(responseMessage);

            var r = responseMessage as Response<OrderCancellationFeeAuthorizationAcquired>;

            if (r?.Exception is null)
            {
                Assert.Fail("Expected exception");
                return;
            }

            Assert.NotNull(r.Exception);
            Assert.False(r.IsSuccessful);
            Assert.IsNotEmpty(r.Exception.Message, "Expected exception message");
            Assert.IsInstanceOf<ValidationException>(r.Exception);

            await TestContext.Out.WriteLineAsync($"Exception {r.Exception.GetType().Name}, message {r.Exception.Message}");
        }

        #region InvalidRequestTestCaseSources

        private static IEnumerable<TestCaseData> InvalidRequestTestCaseSources()
        {
            var orderService = new Mock<IOrderService>();
            var paymentService = new Mock<ICreditCardPaymentService>();

            orderService
                .Setup(s => s.FindOneById(It.Is<string>(s1 => s1 == string.Empty)))
                .Returns<Order>(null);
            
            paymentService
                .Setup(s => s.Authorize(It.IsAny<decimal>(), It.IsAny<CreditCard>()))
                .ReturnsAsync(new AuthorizeResult("AuthToken", null));
            
            yield return TestUtility.GetCaseData("OrderIdNotValid",
                new object[]
                {
                    new AuthorizeOrderCancellationFeeRequest(string.Empty, new PaymentMethod(AMEX1)), 
                    orderService,
                    paymentService
                });
        }

        #endregion
    }
}
