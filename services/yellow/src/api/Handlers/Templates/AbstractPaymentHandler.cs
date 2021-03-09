using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.Yellow.Api.Services;
using artiso.AdsdHotel.Yellow.Contracts;
using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Api.Handlers.Templates
{
    public abstract class AbstractPaymentHandler<TRequestMessage, TResponseMessage> :
        AbstractHandler<TRequestMessage, TResponseMessage>
    {
        protected async Task AddPaymentMethodToOrder(Order order, CreditCard creditCard, string? authorizePaymentToken)
        {
            if (string.IsNullOrWhiteSpace(authorizePaymentToken))
                throw new InvalidOperationException($"ChargeResult must contain {nameof(ChargeResult.AuthorizePaymentToken)}");
            
            var orderCreditCard = creditCard.GetOrderCreditCard(authorizePaymentToken);

            await AddPaymentMethod(order, new OrderPaymentMethod(orderCreditCard));
        }
        
        protected abstract Task AddPaymentMethod(Order order, OrderPaymentMethod paymentMethod);
    }
}
