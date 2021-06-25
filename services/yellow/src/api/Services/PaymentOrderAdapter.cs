using System;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.Yellow.Api.Validation;
using artiso.AdsdHotel.Yellow.Contracts;
using artiso.AdsdHotel.Yellow.Contracts.Commands;
using artiso.AdsdHotel.Yellow.Contracts.Models;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace artiso.AdsdHotel.Yellow.Api.Services
{
    public class PaymentOrderAdapter : IPaymentOrderAdapter
    {
        private readonly ICreditCardPaymentService _paymentService;
        private readonly IOrderService _orderService;

        public PaymentOrderAdapter(ICreditCardPaymentService paymentService, IOrderService orderService)
        {
            _paymentService = paymentService;
            _orderService = orderService;
        }
        
        public async Task<ChargeResult> ChargeOrder(Order order, decimal amount)
        {
            var paymentMethods = HandlerHelper.Ensure(order.PaymentMethods);
            var paymentMethod = paymentMethods!.LastOrDefault();
            var payToken = paymentMethod!.CreditCard.PaymentAuthorizationTokenId;

            if (payToken is null)
                throw new InvalidOperationException($"The active payment method of Order {order.Id} is not suitable for payment: MissingToken");

            var chargeResult = await Charge(amount, payToken!);
            if (chargeResult.IsSuccess != true)
                throw chargeResult.Exception ?? new InvalidOperationException($"{nameof(chargeResult)}");

            return chargeResult;
        }

        public async Task<ChargeResult> ChargeOrder(Order order, decimal amount, PaymentMethod alternativePaymentMethod)
        {
            if (alternativePaymentMethod.CreditCard is null)
                throw new ValidationException($"{nameof(ChargeForOrderCancellationFeeRequest.AlternativePaymentMethod)} must have a CreditCard");

            if (!alternativePaymentMethod.Validate().PaymentMethodIsValid(p => p).IsValid())
                throw new ValidationException(
                    $"{nameof(ChargeForOrderCancellationFeeRequest.AlternativePaymentMethod)} must have a valid CreditCard");

            var chargeResult = await Charge(amount, alternativePaymentMethod.CreditCard);
            if (!chargeResult.IsSuccess)
                throw chargeResult.Exception ?? new InvalidOperationException($"ChargeOperation failed for order {order.Id}");

            return chargeResult;
        }
        
        public async Task AddPaymentMethodToOrder(Order order, CreditCard? creditCard, string? authorizePaymentToken)
        {
            if (creditCard is null)
                throw new InvalidOperationException("Payment method must have a credit card");

            if (string.IsNullOrWhiteSpace(authorizePaymentToken))
                throw new InvalidOperationException($"ChargeResult must contain {nameof(ChargeResult.AuthorizePaymentToken)}");
            
            var orderCreditCard = creditCard.GetOrderCreditCard(authorizePaymentToken);

            await AddPaymentMethod(order, new StoredPaymentMethod(orderCreditCard));
        }
        
        private Task<ChargeResult> Charge(decimal amount, string paymentToken)
        {
            return _paymentService.Charge(amount, paymentToken);
        }

        private Task<ChargeResult> Charge(decimal amount, CreditCard creditCard)
        {
            return _paymentService.Charge(amount, creditCard);
        }
        
        private Task AddPaymentMethod(Order order, StoredPaymentMethod paymentMethod)
        {
            return _orderService.AddPaymentMethod(order, paymentMethod);
        }
    }
}
