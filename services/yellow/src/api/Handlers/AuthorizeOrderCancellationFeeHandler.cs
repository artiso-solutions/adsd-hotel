using System;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.Yellow.Api.Services;
using artiso.AdsdHotel.Yellow.Api.Validation;
using artiso.AdsdHotel.Yellow.Contracts.Commands;
using artiso.AdsdHotel.Yellow.Events;
using NServiceBus;

namespace artiso.AdsdHotel.Yellow.Api.Handlers
{
    public class AuthorizeOrderCancellationFeeHandler : IHandleMessages<AuthorizeOrderCancellationFeeRequest>
    {
        private readonly IOrderService _orderService;
        private readonly ICreditCardPaymentService _paymentService;

        public AuthorizeOrderCancellationFeeHandler(IOrderService orderService, 
            ICreditCardPaymentService paymentService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
        }
        
        public async Task Handle(AuthorizeOrderCancellationFeeRequest message, IMessageHandlerContext context)
        {
            try
            {
                var validateResult = ValidateRequest(message);
                if (!validateResult.IsValid())
                    throw new ValidationException(validateResult);
                
                var order = HandlerHelper.Ensure(await _orderService.FindOneById(message.OrderId))!;
            
                var paymentMethods = order.PaymentMethods;
                var paymentMethod = paymentMethods?.LastOrDefault();
                var payToken = paymentMethod?.CreditCard.PaymentAuthorizationTokenId;
            
                if (payToken is null)
                    throw new InvalidOperationException($"The active payment method of Order {order.Id} is not suitable for payment: MissingToken");
            
                var authorizeResult = await _paymentService.Authorize(order.Price.CancellationFee, payToken!);
                if (authorizeResult.IsSuccess != true)
                    throw authorizeResult.Exception ?? new InvalidOperationException($"{nameof(authorizeResult)}");
            
                await context.Reply(new Response<bool>(true));
            }
            catch (Exception e)
            {
                await context.Publish(new AuthorizeOrderCancellationFeeFailed(message.OrderId, e.Message));
                await context.Reply(new Response<bool>(e));
            }
        }
        
        private ValidationModelResult<AuthorizeOrderCancellationFeeRequest> ValidateRequest(AuthorizeOrderCancellationFeeRequest message)
        {
            return message.Validate()
                .HasData(r => r.OrderId, 
                    $"{nameof(AuthorizeOrderCancellationFeeRequest.OrderId)} should contain data");
            
        }
    }
}
