using System.Threading.Tasks;
using artiso.AdsdHotel.Yellow.Api.Validation;
using artiso.AdsdHotel.Yellow.Contracts.Commands;
using artiso.AdsdHotel.Yellow.Events;
using NServiceBus;

namespace artiso.AdsdHotel.Yellow.Api.Handlers
{
    public class AuthorizeOrderCancellationFeeHandler : IHandleMessages<AuthorizeOrderCancellationFeeRequest>
    {
        public async Task Handle(AuthorizeOrderCancellationFeeRequest requestMessage, IMessageHandlerContext context)
        {
            var validateResult = requestMessage.Validate()
                .HasData(r => r.OrderId, 
                    $"{nameof(AuthorizeOrderCancellationFeeRequest.OrderId)} should not be null")
                .NotNull(r => r.PaymentMethod, 
                    $"{nameof(AuthorizeOrderCancellationFeeRequest.PaymentMethod)} should not be null")
                .NotNull(r => r.PaymentMethod.CreditCard, 
                    $"{nameof(AuthorizeOrderCancellationFeeRequest.PaymentMethod.CreditCard)} should not be null");
            
            if (!validateResult.IsValid())
            {
                var ex = new ValidationException(validateResult);
                
                var r = new Response<OrderCancellationFeeAuthorizationAcquired>(ex);
                
                await context.Publish(r);
                
                return;
            }
            
            //
            
            string orderId = requestMessage.OrderId;
            
            var responseMessage = new OrderCancellationFeeAuthorizationAcquired(orderId);
            
            var response = new Response<OrderCancellationFeeAuthorizationAcquired>(responseMessage);

            await context.Publish(response);
        }
    }
}
