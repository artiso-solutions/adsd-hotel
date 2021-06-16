using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.Yellow.Api.Handlers;
using artiso.AdsdHotel.Yellow.Api.Services;
using artiso.AdsdHotel.Yellow.Api.Validation;
using artiso.AdsdHotel.Yellow.Contracts;
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

            // Happens when all is ok
            Assert.AreEqual(1, context.PublishedMessages.Length); // 1 published messages
            Assert.AreEqual(1, context.RepliedMessages.Length); // 1 Reply message
            var publishMessage = context.PublishedMessages[0].Message;
            var responseMessage = context.RepliedMessages[0].Message; 
            Assert.IsInstanceOf<OrderCancellationFeeAuthorizationAcquired>(publishMessage);  // 1 type of message response
            Assert.IsInstanceOf<Response<bool>>(responseMessage); // of type Response<bool>
            Assert.IsTrue(((Response<bool>) responseMessage).Value); // whose value is true
        }
        
        #region ValidRequestTestSource

        private static IEnumerable<TestCaseData> ValidRequestTestSource()
        {
            var orderService = new Mock<IOrderService>();
            var paymentService = new Mock<ICreditCardPaymentService>();
            var authResult = new AuthorizeResult("AuthToken", null);
            var orderStoredPaymentMethod = new StoredPaymentMethod(MASTERCARD1.GetOrderCreditCard(authResult.AuthorizePaymentToken!));
            
            orderService
                .Setup(s => s.FindOneById(It.Is<string>(s1 => s1 == "orderId")))
                .ReturnsAsync(new Order("orderId", new Price(0, 0))
                {
                    PaymentMethods = new List<StoredPaymentMethod>()
                    {
                        orderStoredPaymentMethod
                    }
                });

            paymentService
                .Setup(s => s.Authorize(It.IsAny<decimal>(), It.IsAny<string>()))
                .ReturnsAsync(authResult);
            
            yield return TestUtility.GetCaseData("BasicScenario",
                new object[]
                {
                    // TODO : set on the order new PaymentMethod(MASTERCARD1)
                    new AuthorizeOrderCancellationFeeRequest("orderId"), 
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

            // Happens when there's a failure
            Assert.AreEqual(1, context.PublishedMessages.Length); // No published messages
            Assert.AreEqual(1, context.RepliedMessages.Length); // 1 Reply message
            var responseMessage = context.RepliedMessages[0].Message; // of type Response<bool>
            Assert.IsInstanceOf<Response<bool>>(responseMessage);
            var publishMessage = context.PublishedMessages[0].Message;
            Assert.IsInstanceOf<AuthorizeOrderCancellationFeeFailed>(publishMessage);
            Assert.IsFalse(((Response<bool>) responseMessage).Value); // whose value is false
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
            
            yield return TestUtility.GetCaseData("OrderIdMissing",
                new object[]
                {
                    // TODO : Set on the order new PaymentMethod(AMEX1)
                    new AuthorizeOrderCancellationFeeRequest(string.Empty), 
                    orderService,
                    paymentService
                });
            yield return TestUtility.GetCaseData("OrderIdNotFound",
                new object[]
                {
                    // TODO : Set on the order new PaymentMethod(AMEX1)
                    new AuthorizeOrderCancellationFeeRequest("_MISSING_ID"), 
                    orderService,
                    paymentService
                });
        }

        #endregion

        [Test]
        [TestCaseSource(nameof(InvalidOperationTestCaseSources))]
        public async Task InvalidOperationTest(AuthorizeOrderCancellationFeeRequest request,
            Mock<IOrderService> orderService,
            Mock<ICreditCardPaymentService> paymentService)
        {
            var handler = new AuthorizeOrderCancellationFeeHandler(orderService.Object, paymentService.Object);
            var context = new TestableMessageHandlerContext();

            await handler.Handle(request, context)
                .ConfigureAwait(false);

            // Happens when there's a failure
            Assert.AreEqual(1, context.PublishedMessages.Length); // No published messages
            Assert.AreEqual(1, context.RepliedMessages.Length); // 1 Reply message
            var publishMessage = context.PublishedMessages[0].Message;
            Assert.IsInstanceOf<AuthorizeOrderCancellationFeeFailed>(publishMessage);
            var responseMessage = context.RepliedMessages[0].Message; // of type Response<bool>
            Assert.IsInstanceOf<Response<bool>>(responseMessage);
            Assert.IsFalse(((Response<bool>) responseMessage).Value); // whose value is false
        }

        #region InvalidOperationTestCaseSources

        private static IEnumerable<TestCaseData> InvalidOperationTestCaseSources()
        {
            var orderService = new Mock<IOrderService>();
            var paymentService = new Mock<ICreditCardPaymentService>();

            orderService
                .Setup(s => s.FindOneById(It.IsAny<string>()))
                .ReturnsAsync(new Order("o.Id", new Price(10, 100)));
            
            paymentService
                .Setup(s => s.Authorize(It.IsAny<decimal>(), It.IsAny<CreditCard>()))
                .ReturnsAsync(new AuthorizeResult(string.Empty, new Exception("Customer cannot pay the requested amount"))); // not the same exception of the test
            
            yield return TestUtility.GetCaseData("PaymentFails",
                new object[]
                {
                    // todo set on the order new PaymentMethod(MASTERCARD1)
                    new AuthorizeOrderCancellationFeeRequest("_WHATEVER_ID"), 
                    orderService,
                    paymentService
                });
        }

        #endregion
    }
}
