using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.Yellow.Api.Services;
using artiso.AdsdHotel.Yellow.Api.Validation;
using artiso.AdsdHotel.Yellow.Contracts;
using artiso.AdsdHotel.Yellow.Contracts.Commands;
using artiso.AdsdHotel.Yellow.Contracts.Models;
using artiso.AdsdHotel.Yellow.Events;

namespace artiso.AdsdHotel.Yellow.Api.Handlers
{
    public class AuthorizeOrderCancellationFeeHandler : AbstractHandler<AuthorizeOrderCancellationFeeRequest, OrderCancellationFeeAuthorizationAcquired>
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
            // FormOfPayment validation
                // CreditCard
            
            // Collection
                // Order
                // FormOfPayments
                    // CreditCard (Register the last 4 digit of PAN)
                        // PaymentToken
                // Transactions
                    // OrderId
                    // FopId

            Order order = await Ensure(message, m => _orderService.FindOneById(m.OrderId));
            var cancellationFee = order.Price.CancellationFee;
            var creditCard = message.PaymentMethod.CreditCard;
            
            // Make Authorization
                // Payment Service
                // Given a ValidCreditCard
                // Check if the CreditCard has the required CancellationFee amount
            var authorizeResult = await _paymentService.Authorize(cancellationFee, creditCard!);
            if (authorizeResult.IsSuccess != true)
                throw authorizeResult.Exception ?? new ArgumentNullException($"{nameof(authorizeResult)}");
            
            // Store the authorize result
            order.OrderPaymentMethod = new OrderPaymentMethod(creditCard!.GetOrderCreditCard(authorizeResult.AuthorizePaymentToken));

            // Transaction??
            return new OrderCancellationFeeAuthorizationAcquired(message.OrderId);
        }

        protected override ValidationModelResult<AuthorizeOrderCancellationFeeRequest> ValidateRequest(AuthorizeOrderCancellationFeeRequest message)
        {
            return message.Validate()
                .HasData(r => r.OrderId, 
                    $"{nameof(AuthorizeOrderCancellationFeeRequest.OrderId)} should contain data")
                .NotNull(r => r.PaymentMethod, 
                    $"{nameof(AuthorizeOrderCancellationFeeRequest.PaymentMethod)} should not be null")
                .NotNull(r => r.PaymentMethod.CreditCard, 
                    $"{nameof(AuthorizeOrderCancellationFeeRequest.PaymentMethod.CreditCard)} should not be null");
        }
    }
}
