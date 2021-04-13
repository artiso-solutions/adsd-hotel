using System;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.Yellow.Api.Handlers.Templates;
using artiso.AdsdHotel.Yellow.Api.Services;
using artiso.AdsdHotel.Yellow.Api.Validation;
using artiso.AdsdHotel.Yellow.Contracts.Commands;
using artiso.AdsdHotel.Yellow.Contracts.Models;
using artiso.AdsdHotel.Yellow.Events;

namespace artiso.AdsdHotel.Yellow.Api.Handlers
{
    public class AuthorizeOrderCancellationFeeHandler : AbstractPaymentHandler<AuthorizeOrderCancellationFeeRequest, OrderCancellationFeeAuthorizationAcquired>
    {
        private readonly IOrderService _orderService;
        private readonly ICreditCardPaymentService _paymentService;

        public AuthorizeOrderCancellationFeeHandler(IOrderService orderService, 
            ICreditCardPaymentService paymentService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
        }
        
        protected override async Task<OrderCancellationFeeAuthorizationAcquired> Handle(AuthorizeOrderCancellationFeeRequest message)
        {
            Order order = await Ensure(message, m => _orderService.FindOneById(m.OrderId));
            
            var paymentMethods = order.PaymentMethods;
            var paymentMethod = paymentMethods?.LastOrDefault();
            var payToken = paymentMethod?.CreditCard.PaymentAuthorizationTokenId;
            
            if (payToken is null)
                throw new InvalidOperationException($"The active payment method of Order {order.Id} is not suitable for payment: MissingToken");
            
            var authorizeResult = await _paymentService.Authorize(order.Price.CancellationFee, payToken!);
            if (authorizeResult.IsSuccess != true)
                throw authorizeResult.Exception ?? new InvalidOperationException($"{nameof(authorizeResult)}");
            
            return new OrderCancellationFeeAuthorizationAcquired(message.OrderId);
        }
        
        protected override object Fail(AuthorizeOrderCancellationFeeRequest requestMessage)
        {
            return new AuthorizeOrderCancellationFeeFailed(requestMessage.OrderId);
        }

        protected override ValidationModelResult<AuthorizeOrderCancellationFeeRequest> ValidateRequest(AuthorizeOrderCancellationFeeRequest message)
        {
            return message.Validate()
                .HasData(r => r.OrderId, 
                    $"{nameof(AuthorizeOrderCancellationFeeRequest.OrderId)} should contain data");
            
        }

        protected override Task AddPaymentMethod(Order order, StoredPaymentMethod paymentMethod)
        {
            return _orderService.AddPaymentMethod(order, paymentMethod);
        }
    }
}
