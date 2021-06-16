using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ChargeForOrderCancellationFeeHandlerTests
    {
        [Test]
        [TestCaseSource(nameof(ValidRequestTestSource))]
        public async Task ValidRequestTest(ChargeForOrderCancellationFeeRequest request,
            Order order,
            Mock<IOrderService> orderService,
            Mock<IPaymentOrderAdapter> paymentOrderAdapter)
        {
            var handler = new ChargeForOrderCancellationFeeHandler(orderService.Object, paymentOrderAdapter.Object);
            var context = new TestableMessageHandlerContext();

            await handler.Handle(request, context)
                .ConfigureAwait(false);
            
            // Happens when all is ok
            Assert.AreEqual(1, context.PublishedMessages.Length); // 1 published messages
            Assert.AreEqual(1, context.RepliedMessages.Length); // 1 Reply message
            var publishMessage = context.PublishedMessages[0].Message;
            var responseMessage = context.RepliedMessages[0].Message; 
            Assert.IsInstanceOf<OrderCancellationFeeCharged>(publishMessage);  // 1 type of message response
            Assert.IsInstanceOf<Response<bool>>(responseMessage); // of type Response<bool>
            Assert.IsTrue(((Response<bool>) responseMessage).Value); // whose value is true
            
            var activePaymentMethod = order.PaymentMethods!.LastOrDefault();

            Assert.NotNull(activePaymentMethod, "activePaymentMethod != null");

            if (request.AlternativePaymentMethod is not null)
            {
                // Checks that the right Method gets called
                // paymentOrderAdapter
                //     .Verify(c => c.Charge(It.Is<decimal>(arg => arg == order.Price.CancellationFee),
                //         It.IsAny<CreditCard>()), Times.Once);

                orderService
                    .Verify(c => c.AddPaymentMethod(It.IsAny<Order>(), It.IsAny<StoredPaymentMethod>()),
                        Times.Once);
            }
            else
            {
                var payToken = activePaymentMethod!.CreditCard.PaymentAuthorizationTokenId;

                Assert.NotNull(payToken, "payToken != null");
            }
        }

        #region ValidRequestTestSource

        private static IEnumerable<TestCaseData> ValidRequestTestSource()
        {
            var orderService = new Mock<IOrderService>();
            var paymentService = new Mock<IPaymentOrderAdapter>();

            decimal orderCancellationFee = 10;
            decimal orderCancellationFullAmount = 100;
            var order = new Order("orderId", new Price(orderCancellationFee, orderCancellationFullAmount))
            {
                PaymentMethods = new List<StoredPaymentMethod>()
                {
                    new(AMEX1.GetOrderCreditCard("__AUTH_TOKEN__"))
                }
            };

            orderService
                .Setup(s => s.FindOneById(It.Is<string>(s1 => s1 == "orderId")))
                .ReturnsAsync(order);
            orderService
                .Setup(s => s.AddPaymentMethod(It.IsAny<Order>(), It.IsAny<StoredPaymentMethod>()))
                .Verifiable();

            // paymentService
            //     .Setup(s => s.Charge(It.Is<decimal>(d => d == 10), It.IsAny<string>()))
            //     .ReturnsAsync(new ChargeResult()
            //     {
            //         Transaction = new Transaction("SOME_ID", "__AUTH_TOKEN__", 10, DateTime.Now)
            //     })
            //     .Verifiable();
            // paymentService
            //     .Setup(s => s.Charge(It.Is<decimal>(d => d == 10), It.IsAny<CreditCard>()))
            //     .ReturnsAsync(new ChargeResult()
            //     {
            //         Transaction = new Transaction("SOME_ID", "__AUTH_TOKEN__", 10, DateTime.Now),
            //         AuthorizePaymentToken = "__AUTH_TOKEN__"
            //     })
            //     .Verifiable();

            yield return TestUtility.GetCaseData("BasicScenario.PayWithAlreadyStoredMethod",
                new object[]
                {
                    new ChargeForOrderCancellationFeeRequest("orderId"), order, orderService, paymentService
                });

            yield return TestUtility.GetCaseData("BasicScenario.OverridePaymentMethod",
                new object[]
                {
                    new ChargeForOrderCancellationFeeRequest("orderId")
                    {
                        AlternativePaymentMethod = new PaymentMethod(MASTERCARD1)
                    },
                    order, orderService, paymentService
                });
        }

        #endregion

        [Test]
        [TestCaseSource(nameof(InvalidRequestTestCaseSources))]
        public async Task InvalidRequestTest(ChargeForOrderCancellationFeeRequest request,
            Mock<IOrderService> orderService,
            Mock<IPaymentOrderAdapter> paymentOrderAdapter)
        {
            var handler = new ChargeForOrderCancellationFeeHandler(orderService.Object, paymentOrderAdapter.Object);
            var context = new TestableMessageHandlerContext();

            await handler.Handle(request, context)
                .ConfigureAwait(false);

            // Happens when there's a failure
            Assert.AreEqual(1, context.RepliedMessages.Length); // 1 Reply message
            var responseMessage = context.RepliedMessages[0].Message; // of type Response<bool>
            var publishMessage = context.PublishedMessages[0].Message;
            Assert.IsInstanceOf<ChargeOrderCancellationFeeFailed>(publishMessage);
            Assert.IsInstanceOf<Response<bool>>(responseMessage);
            Assert.IsFalse(((Response<bool>) responseMessage).Value); // whose value is false
        }

        #region InvalidRequestTestCaseSources

        private static IEnumerable<TestCaseData> InvalidRequestTestCaseSources()
        {
            var orderService = new Mock<IOrderService>();
            var paymentService = new Mock<IPaymentOrderAdapter>();

            var order = new Order("orderId", new Price(10, 100))
            {
                PaymentMethods = new List<StoredPaymentMethod>()
                {
                    new(AMEX1.GetOrderCreditCard("__AUTH_TOKEN__"))
                }
            };

            orderService
                .Setup(s => s.FindOneById(It.Is<string>(s1 => s1 == string.Empty)))
                .Returns<Order>(null);
            orderService
                .Setup(s => s.FindOneById(It.Is<string>(s1 => s1 == "_VALID_ID")))
                .ReturnsAsync(order);

            // PaymentService Charge setup
            // paymentService
            //     .Setup(s => s.Charge(It.IsAny<decimal>(), It.IsAny<CreditCard>()))
            //     .ReturnsAsync(new ChargeResult()
            //     {
            //         AuthorizePaymentToken = "__AUTH_TOKEN__"
            //     });
            // paymentService
            //     .Setup(s => s.Charge(It.IsAny<decimal>(), It.IsAny<string>()))
            //     .ReturnsAsync(new ChargeResult());
            
            yield return TestUtility.GetCaseData("OrderIdMissing",
                new object[] {new ChargeForOrderCancellationFeeRequest(string.Empty), orderService, paymentService});
            yield return TestUtility.GetCaseData("OrderIdNotFound",
                new object[] {new ChargeForOrderCancellationFeeRequest("_MISSING_ID"), orderService, paymentService});

            var invalidCreditCard =
                new CreditCard(IssuingNetwork.AmericanExpress, "JohnDoe", "0", "0", DateTime.MaxValue);
            yield return TestUtility.GetCaseData("OverridePaymentMethodHasInvalidCreditCard",
                new object[]
                {
                    new ChargeForOrderCancellationFeeRequest("_VALID_ID")
                    {
                        AlternativePaymentMethod = new PaymentMethod(invalidCreditCard)
                    },
                    orderService, paymentService
                });

            var nullCreditCard = null as CreditCard;
            yield return TestUtility.GetCaseData("OverridePaymentMethodHasInvalidCreditCard",
                new object[]
                {
                    new ChargeForOrderCancellationFeeRequest("_VALID_ID")
                    {
                        AlternativePaymentMethod = new PaymentMethod(nullCreditCard)
                    },
                    orderService, paymentService
                });
        }

        #endregion
        
        [Test]
        [TestCaseSource(nameof(InvalidOperationTestCaseSources))]
        public async Task InvalidOperationTest(ChargeForOrderCancellationFeeRequest request,
            Mock<IOrderService> orderService,
            Mock<IPaymentOrderAdapter> paymentOrderAdapter)
        {
            var handler = new ChargeForOrderCancellationFeeHandler(orderService.Object, paymentOrderAdapter.Object);
            var context = new TestableMessageHandlerContext();

            await handler.Handle(request, context)
                .ConfigureAwait(false);

            // Happens when there's a failure
            Assert.AreEqual(1, context.RepliedMessages.Length); // 1 Reply message
            var responseMessage = context.RepliedMessages[0].Message; // of type Response<bool>
            var publishMessage = context.PublishedMessages[0].Message;
            Assert.IsInstanceOf<ChargeOrderCancellationFeeFailed>(publishMessage);
            Assert.IsInstanceOf<Response<bool>>(responseMessage);
            Assert.IsFalse(((Response<bool>) responseMessage).Value); // whose value is false
        }

        #region InvalidOperationTestCaseSources

        private static IEnumerable<TestCaseData> InvalidOperationTestCaseSources()
        {
            var orderService = new Mock<IOrderService>();
            var invalidPaymentOrderService = new Mock<IOrderService>();
            var paymentService = new Mock<IPaymentOrderAdapter>();

            var order = new Order("orderId", new Price(10, 100))
            {
                PaymentMethods = new List<StoredPaymentMethod>()
                {
                    new(AMEX1.GetOrderCreditCard("__AUTH_TOKEN__"))
                }
            };
            
            var invalidOrder = new Order("orderId", new Price(10, 100))
            {
                PaymentMethods = new List<StoredPaymentMethod>()
                {
                    new(AMEX1.GetOrderCreditCard(null!))
                }
            };
            
            orderService
                .Setup(s => s.FindOneById(It.IsAny<string>()))
                .ReturnsAsync(order);
            
            invalidPaymentOrderService
                .Setup(s => s.FindOneById(It.IsAny<string>()))
                .ReturnsAsync(invalidOrder);
            
            // paymentService
            //     .Setup(s => s.Charge(It.IsAny<decimal>(), It.IsAny<string>()))
            //     .ReturnsAsync(new ChargeResult()
            //     {
            //         Exception = new Exception("Payment failed")
            //     })
            //     .Verifiable();
            // paymentService
            //     .Setup(s => s.Charge(It.IsAny<decimal>(), It.IsAny<CreditCard>()))
            //     .ReturnsAsync(new ChargeResult()
            //     {
            //         Exception = new Exception("Payment failed")
            //     })
            //     .Verifiable();
            
            yield return TestUtility.GetCaseData("PaymentFails",
                new object[]
                {
                    new ChargeForOrderCancellationFeeRequest("_WHATEVER_ID"), 
                    orderService,
                    paymentService
                });
            
            yield return TestUtility.GetCaseData("NoSuitablePaymentToken",
                new object[]
                {
                    new ChargeForOrderCancellationFeeRequest("_WHATEVER_ID"), 
                    invalidPaymentOrderService,
                    paymentService
                });
        }

        #endregion
    }
}
