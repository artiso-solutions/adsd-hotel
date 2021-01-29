using System.Collections.Generic;
using System.Threading.Tasks;
using artiso.AdsdHotel.Yellow.Api.Handlers;
using artiso.AdsdHotel.Yellow.Contracts.Commands;
using artiso.AdsdHotel.Yellow.Contracts.Models;
using artiso.AdsdHotel.Yellow.Events;
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
        public async Task ValidRequestTest(AuthorizeOrderCancellationFeeRequest request)
        {
            var handler = new AuthorizeOrderCancellationFeeHandler();
            var context = new TestableMessageHandlerContext();
            
            await handler.Handle(request, context)
                .ConfigureAwait(false);
            
            Assert.AreEqual(1, context.PublishedMessages.Length);
            var responseMessage = context.PublishedMessages[0].Message;
            Assert.IsInstanceOf<Response<OrderCancellationFeeAuthorizationAcquired>>(responseMessage);
            Assert.True(((responseMessage as Response<OrderCancellationFeeAuthorizationAcquired>)!).IsSuccessful);
        }

        #region ValidRequestTestSource

        private static IEnumerable<TestCaseData> ValidRequestTestSource()
        {
            yield return TestUtility.GetCaseData("BasicScenario", new AuthorizeOrderCancellationFeeRequest("orderId", new PaymentMethod(AMEX1)));
        }

        #endregion

        [Test]
        [TestCaseSource(nameof(InvalidRequestTestCaseSources))]
        public async Task InvalidRequestTest(AuthorizeOrderCancellationFeeRequest request)
        {
            var handler = new AuthorizeOrderCancellationFeeHandler();
            var context = new TestableMessageHandlerContext();
            
            await handler.Handle(request, context)
                .ConfigureAwait(false);
            
            Assert.AreEqual(1, context.PublishedMessages.Length);
            var responseMessage = context.PublishedMessages[0].Message;
            Assert.IsInstanceOf<Response<OrderCancellationFeeAuthorizationAcquired>>(responseMessage);
            
            var r = responseMessage as Response<OrderCancellationFeeAuthorizationAcquired>;
            Assert.NotNull(r?.Exception);
            Assert.False(r?.IsSuccessful);
        }

        #region InvalidRequestTestCaseSources

        private static IEnumerable<TestCaseData> InvalidRequestTestCaseSources()
        {
            yield return TestUtility.GetCaseData("OrderIdNotValid", new AuthorizeOrderCancellationFeeRequest(string.Empty, new PaymentMethod(AMEX1)));
        }

        #endregion
    }
}