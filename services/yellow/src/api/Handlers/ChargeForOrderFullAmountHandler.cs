using System;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.Yellow.Api.Services;
using artiso.AdsdHotel.Yellow.Api.Validation;
using artiso.AdsdHotel.Yellow.Contracts;
using artiso.AdsdHotel.Yellow.Contracts.Commands;
using artiso.AdsdHotel.Yellow.Contracts.Models;
using artiso.AdsdHotel.Yellow.Events;

namespace artiso.AdsdHotel.Yellow.Api.Handlers
{
    public class ChargeForOrderFullAmountHandler : AbstractHandler<ChargeForOrderFullAmountRequest, OrderFullAmountCharged>
    {
        private readonly IOrderService _orderService;
        private readonly ICreditCardPaymentService _creditCardPaymentService;

        public ChargeForOrderFullAmountHandler(IOrderService orderService, ICreditCardPaymentService creditCardPaymentService)
        {
            _orderService = orderService;
            _creditCardPaymentService = creditCardPaymentService;
        }
        
        protected async override Task<OrderFullAmountCharged> Handle(ChargeForOrderFullAmountRequest message)
        {
            var order = await Ensure(message, r => _orderService.FindOneById(r.OrderId));

            var activePaymentMethod = order.OrderPaymentMethods.LastOrDefault();

            if (message.AlternativePaymentMethod is not null)
            {
                var creditCard = message.AlternativePaymentMethod.CreditCard;
                var chargeResult = await _creditCardPaymentService.Charge(order.Price.Amount, creditCard!);
                if (chargeResult.AuthorizePaymentToken is null)
                    throw new InvalidOperationException("Charge result does not contain token");

                var orderPaymentMethod = new OrderPaymentMethod(creditCard.GetOrderCreditCard(chargeResult.AuthorizePaymentToken));
                
                await _orderService.AddPaymentMethod(order, orderPaymentMethod);
                
                return new OrderFullAmountCharged(message.OrderId);
            }
            
            var creditCardProviderPaymentToken = activePaymentMethod?.CreditCard.ProviderPaymentToken;
            
            await _creditCardPaymentService.Charge(order.Price.Amount, creditCardProviderPaymentToken!);
            
            return new OrderFullAmountCharged(message.OrderId);
        }

        protected override ValidationModelResult<ChargeForOrderFullAmountRequest> ValidateRequest(
            ChargeForOrderFullAmountRequest message)
        {
            var v = message.Validate()
                .HasData(m => m.OrderId, $"Missing {nameof(ChargeForOrderFullAmountRequest.OrderId)}");
            
            if (message.AlternativePaymentMethod is not null)
                return v.That(m => m.AlternativePaymentMethod.Validate().PaymentMethodIsValid(p => p).IsValid(),
                    $"{nameof(ChargeForOrderFullAmountRequest.AlternativePaymentMethod)} must have a valid creditcard");

            return v;
        }
    }
}
