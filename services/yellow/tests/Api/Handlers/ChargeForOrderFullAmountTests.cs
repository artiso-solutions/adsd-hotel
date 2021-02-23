using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class ChargeForOrderFullAmountTests
    {
        [Test]
        [TestCaseSource(nameof(ValidRequestTestSource))]
        public async Task ValidRequestTest(ChargeForOrderFullAmountRequest request,
            Order order,
            Mock<IOrderService> orderService,
            Mock<ICreditCardPaymentService> paymentService)
        {
            var handler = new ChargeForOrderFullAmountHandler(orderService.Object, paymentService.Object);
            var context = new TestableMessageHandlerContext();

            await handler.Handle(request, context)
                .ConfigureAwait(false);
            
            Assert.AreEqual(1, context.PublishedMessages.Length);
            var responseMessage = context.PublishedMessages[0].Message;
            Assert.IsInstanceOf<Response<OrderFullAmountCharged>>(responseMessage);
            var rm = responseMessage as Response<OrderFullAmountCharged>;
            Assert.True(rm?.IsSuccessful, $"{rm?.Exception?.Message} \n {rm?.Exception?.StackTrace}");
            
            var activePaymentMethod = order.OrderPaymentMethods.LastOrDefault();
            Assert.NotNull(activePaymentMethod, "activePaymentMethod != null");
            
            if (request.AlternativePaymentMethod is not null)
            {
                // Checks that the right Method gets called
                paymentService
                    .Verify(c => c.Charge(It.Is<decimal>(arg => arg == order.Price.Amount),
                        It.IsAny<CreditCard>()), Times.Once);

                orderService
                    .Verify(c => c.AddPaymentMethod(It.IsAny<Order>(), It.IsAny<OrderPaymentMethod>()),
                        Times.Once);
            }
            else
            {
                var payToken = activePaymentMethod!.CreditCard.ProviderPaymentToken;

                Assert.NotNull(payToken, "payToken != null");

                // Checks that the right Method gets called
                paymentService
                    .Verify(c => c.Charge(It.Is<decimal>(arg => arg == order.Price.Amount),
                        It.Is<string>(arg => arg == payToken)), Times.Once);
            }
        }
        
        #region ValidRequestTestSource

        private static IEnumerable<TestCaseData> ValidRequestTestSource()
        {
            var orderService = new Mock<IOrderService>();
            var paymentService = new Mock<ICreditCardPaymentService>();

            decimal orderCancellationFee = 10;
            decimal orderFullAmount = 100;
            var order = new Order("orderId", new Price(orderCancellationFee, orderFullAmount))
            {
                OrderPaymentMethods = new List<OrderPaymentMethod>()
                {
                    new(AMEX1.GetOrderCreditCard("__AUTH_TOKEN__"))
                }
            };

            orderService
                .Setup(s => s.FindOneById(It.Is<string>(s1 => s1 == "orderId")))
                .ReturnsAsync(order);
            orderService
                .Setup(s => s.AddPaymentMethod(It.IsAny<Order>(), It.IsAny<OrderPaymentMethod>()))
                .Verifiable();

            paymentService
                .Setup(s => s.Charge(It.Is<decimal>(d => d == orderFullAmount), It.IsAny<string>()))
                .ReturnsAsync(new ChargeResult())
                .Verifiable();
            paymentService
                .Setup(s => s.Charge(It.Is<decimal>(d => d == orderFullAmount), It.IsAny<CreditCard>()))
                .ReturnsAsync(new ChargeResult() {AuthorizePaymentToken = "__AUTH_TOKEN__"})
                .Verifiable();

            yield return TestUtility.GetCaseData("BasicScenario.PayWithAlreadyStoredMethod",
                new object[]
                {
                    new ChargeForOrderFullAmountRequest("orderId"), 
                    order, orderService, paymentService
                });

            yield return TestUtility.GetCaseData("BasicScenario.OverridePaymentMethod",
                new object[]
                {
                    new ChargeForOrderFullAmountRequest("orderId") { AlternativePaymentMethod = new PaymentMethod(MASTERCARD1) },
                    order, orderService, paymentService
                });
        }

        #endregion
        
        [Test]
        [TestCaseSource(nameof(InvalidRequestTestCaseSources))]
        public async Task InvalidRequestTest(ChargeForOrderFullAmountRequest request,
            Mock<IOrderService> orderService,
            Mock<ICreditCardPaymentService> paymentService)
        {
            var handler = new ChargeForOrderFullAmountHandler(orderService.Object, paymentService.Object);
            var context = new TestableMessageHandlerContext();

            await handler.Handle(request, context)
                .ConfigureAwait(false);

            var r = context.PublishedMessages[0].Message as Response<OrderFullAmountCharged>;

            Assert.NotNull(r!.Exception);
            Assert.False(r.IsSuccessful);
            Assert.IsNotEmpty(r.Exception!.Message, "Expected exception message");
            Assert.IsInstanceOf<ValidationException>(r.Exception);

            await TestContext.Out.WriteLineAsync(
                $"Exception {r.Exception!.GetType().Name}, message {r.Exception!.Message}");
        }
        
        #region InvalidRequestTestCaseSources

        private static IEnumerable<TestCaseData> InvalidRequestTestCaseSources()
        {
            var orderService = new Mock<IOrderService>();
            var paymentService = new Mock<ICreditCardPaymentService>();

            var order = new Order("orderId", new Price(10, 100))
            {
                OrderPaymentMethods = new List<OrderPaymentMethod>()
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
            paymentService
                .Setup(s => s.Charge(It.IsAny<decimal>(), It.IsAny<CreditCard>()))
                .ReturnsAsync(new ChargeResult()
                {
                    AuthorizePaymentToken = "__AUTH_TOKEN__"
                });
            paymentService
                .Setup(s => s.Charge(It.IsAny<decimal>(), It.IsAny<string>()))
                .ReturnsAsync(new ChargeResult());
            
            yield return TestUtility.GetCaseData("OrderIdMissing",
                new object[] {new ChargeForOrderFullAmountRequest(string.Empty), orderService, paymentService});
            yield return TestUtility.GetCaseData("OrderIdNotFound",
                new object[] {new ChargeForOrderFullAmountRequest("_MISSING_ID"), orderService, paymentService});

            var invalidCreditCard =
                new CreditCard(IssuingNetwork.AmericanExpress, "JohnDoe", "0", "0", DateTime.MaxValue);
            yield return TestUtility.GetCaseData("OverridePaymentMethodHasInvalidCreditCard",
                new object[]
                {
                    new ChargeForOrderFullAmountRequest("_VALID_ID")
                    {
                        AlternativePaymentMethod = new PaymentMethod(invalidCreditCard)
                    },
                    orderService, paymentService
                });
            
            var nullCreditCard = null as CreditCard;
            yield return TestUtility.GetCaseData("OverridePaymentMethodHasInvalidCreditCard",
                new object[]
                {
                    new ChargeForOrderFullAmountRequest("_VALID_ID")
                    {
                        AlternativePaymentMethod = new PaymentMethod(nullCreditCard)
                    },
                    orderService, paymentService
                });
        }

        #endregion
        
        
        [Test]
        [TestCaseSource(nameof(InvalidOperationTestCaseSources))]
        public async Task InvalidOperationTest(ChargeForOrderFullAmountRequest request,
            Mock<IOrderService> orderService,
            Mock<ICreditCardPaymentService> paymentService)
        {
            var handler = new ChargeForOrderFullAmountHandler(orderService.Object, paymentService.Object);
            var context = new TestableMessageHandlerContext();

            await handler.Handle(request, context)
                .ConfigureAwait(false);

            var r = context.PublishedMessages[0].Message as Response<OrderFullAmountCharged>;

            Assert.NotNull(r!.Exception, "Exception not thrown");
            Assert.False(r.IsSuccessful);
            Assert.IsNotEmpty(r.Exception!.Message, "Expected exception message");
            Assert.IsInstanceOf<InvalidOperationException>(r.Exception);
            
            await TestContext.Out.WriteLineAsync($"Exception {r.Exception!.GetType().Name}, message {r.Exception!.Message}");
        }
        
        
        #region InvalidOperationTestCaseSources

        private static IEnumerable<TestCaseData> InvalidOperationTestCaseSources()
        {
            var orderService = new Mock<IOrderService>();
            var invalidPaymentOrderService = new Mock<IOrderService>();
            var paymentService = new Mock<ICreditCardPaymentService>();

            var order = new Order("orderId", new Price(10, 100))
            {
                OrderPaymentMethods = new List<OrderPaymentMethod>()
                {
                    new(AMEX1.GetOrderCreditCard("__AUTH_TOKEN__"))
                }
            };
            
            var invalidOrder = new Order("orderId", new Price(10, 100))
            {
                OrderPaymentMethods = new List<OrderPaymentMethod>()
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
            
            paymentService
                .Setup(s => s.Charge(It.IsAny<decimal>(), It.IsAny<CreditCard>()))
                .ReturnsAsync(new ChargeResult()
                {
                    Exception = new Exception("Payment failed")
                })
                .Verifiable();
            
            paymentService
                .Setup(s => s.Charge(It.IsAny<decimal>(), It.IsAny<string>()))
                .ReturnsAsync(new ChargeResult()
                {
                    Exception = new Exception("Payment failed")
                })
                .Verifiable();
            
            yield return TestUtility.GetCaseData("PaymentFails",
                new object[]
                {
                    new ChargeForOrderFullAmountRequest("_WHATEVER_ID"), 
                    orderService,
                    paymentService
                });
            
            yield return TestUtility.GetCaseData("NoSuitablePaymentToken",
                new object[]
                {
                    new ChargeForOrderFullAmountRequest("_WHATEVER_ID"), 
                    invalidPaymentOrderService,
                    paymentService
                });
        }

        #endregion
    }
}