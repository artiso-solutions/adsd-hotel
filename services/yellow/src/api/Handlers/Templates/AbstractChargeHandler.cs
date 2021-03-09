using System;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.Yellow.Api.Services;
using artiso.AdsdHotel.Yellow.Api.Validation;
using artiso.AdsdHotel.Yellow.Contracts.Commands;
using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Api.Handlers.Templates
{
    public abstract class AbstractChargeHandler<TRequestMessage, TResponseMessage>
        : AbstractPaymentHandler<TRequestMessage, TResponseMessage>
    {
        protected abstract Task<ChargeResult> Charge(decimal amount, string paymentToken);
        
        protected abstract Task<ChargeResult> Charge(decimal amount, CreditCard creditCard);
        
        protected async Task ChargeOrder(Order order, decimal amount)
        {
            var paymentMethods = Ensure(order, o => o.OrderPaymentMethods);
            var paymentMethod = paymentMethods!.LastOrDefault();
            var payToken = paymentMethod!.CreditCard.ProviderPaymentToken;

            if (payToken is null)
                throw new InvalidOperationException($"The active payment method of Order {order.Id} is not suitable for payment: MissingToken");

            var chargeResult = await Charge(amount, payToken!);
            if (chargeResult.IsSuccess != true)
                throw chargeResult.Exception ?? new InvalidOperationException($"{nameof(chargeResult)}");
        }

        protected async Task ChargeOrder(Order order, decimal amount, PaymentMethod alternativePaymentMethod)
        {
            if (alternativePaymentMethod.CreditCard is null)
                throw new ValidationException($"{nameof(ChargeForOrderCancellationFeeRequest.AlternativePaymentMethod)} must have a CreditCard");

            if (!alternativePaymentMethod.Validate().PaymentMethodIsValid(p => p).IsValid())
                throw new ValidationException(
                    $"{nameof(ChargeForOrderCancellationFeeRequest.AlternativePaymentMethod)} must have a valid CreditCard");

            var chargeResult = await Charge(amount, alternativePaymentMethod.CreditCard);
            if (!chargeResult.IsSuccess)
                throw chargeResult.Exception ?? new InvalidOperationException($"ChargeOperation failed for order {order.Id}");
            
            await AddPaymentMethodToOrder(order, alternativePaymentMethod.CreditCard, chargeResult.AuthorizePaymentToken);
        }
    }
}
