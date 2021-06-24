using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.Yellow.Api.Handlers.Templates;
using artiso.AdsdHotel.Yellow.Contracts.Models;
using NServiceBus.Testing;
using NUnit.Framework;

namespace artiso.AdsdHotel.Yellow.Tests.Api.Handlers
{
    [TestFixture]
    public class AbstractHandlerTests
    {
        [Test]
        public async Task RequestAlwaysValidIfInheritorNotCustomizeValidateRequest()
        {
            var context = new TestableMessageHandlerContext();
            var mockHandler = new MockHandler();
            
            await mockHandler.Handle(null!, context);
            
            Assert.AreEqual(1, context.RepliedMessages.Length);
            var responseMessage = context.RepliedMessages[0].Message;
            
            var r = responseMessage as Response<MockResponse>;
            
            Assert.NotNull(r);
            Assert.Null(r!.Exception);
            Assert.IsTrue(r.IsSuccessful);
        }
        
    }

    public class MockHandler : AbstractPaymentHandler<MockRequest, MockResponse>
    {
        protected override object Fail(MockRequest requestMessage)
        {
            return new();
        }

        protected override Task<MockResponse> Handle(MockRequest message)
        {
            return Task.FromResult(new MockResponse()
            {
                Success = true
            });
        }

        protected override Task AddPaymentMethod(Order order, StoredPaymentMethod paymentMethod) => 
            throw new System.NotImplementedException();

        // Do not implement ValidateRequest
    }

    public class MockRequest
    {
        public string? message { get; set; }
    }

    public class MockResponse
    {
        public bool Success { get; set; }
    }
}