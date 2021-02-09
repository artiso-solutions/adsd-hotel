using System.Threading.Tasks;
using artiso.AdsdHotel.Yellow.Api.Handlers;
using artiso.AdsdHotel.Yellow.Contracts.Commands;
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
            
            Assert.AreEqual(1, context.PublishedMessages.Length);
            var responseMessage = context.PublishedMessages[0].Message;
            
            var r = responseMessage as Response<MockResponse>;
            
            Assert.NotNull(r);
            Assert.Null(r!.Exception);
            Assert.IsTrue(r.IsSuccessful);
        }
        
    }

    public class MockHandler : AbstractHandler<MockRequest, MockResponse>
    {
        protected override Task<MockResponse> Handle(MockRequest message)
        {
            return Task.FromResult(new MockResponse()
            {
                Success = true
            });
        }
        
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
