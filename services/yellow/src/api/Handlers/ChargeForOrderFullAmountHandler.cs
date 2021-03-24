using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.Yellow.Api.Handlers.Templates;
using artiso.AdsdHotel.Yellow.Api.Services;
using artiso.AdsdHotel.Yellow.Api.Validation;
using artiso.AdsdHotel.Yellow.Contracts;
using artiso.AdsdHotel.Yellow.Contracts.Commands;
using artiso.AdsdHotel.Yellow.Contracts.Models;
using artiso.AdsdHotel.Yellow.Events;

namespace artiso.AdsdHotel.Yellow.Api.Handlers
{
    public class ChargeForOrderFullAmountHandler : AbstractChargeHandler<ChargeForOrderFullAmountRequest, OrderFullAmountCharged>
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

            var amountToPay = order.Price.Amount;
            
            var chargeResult = (message.AlternativePaymentMethod is not null)
                ? await ChargeOrder(order, amountToPay, message.AlternativePaymentMethod)
                : await ChargeOrder(order, amountToPay);
            
            if (message.AlternativePaymentMethod is not null)
                await AddPaymentMethodToOrder(order, message.AlternativePaymentMethod.CreditCard, chargeResult.AuthorizePaymentToken);

            // TODO : Should go in a separate handler (and should retrieve it from the used payment token)
            var lastPaymentMethod = order.PaymentMethods!.Last();
            await _orderService.AddTransaction(order, chargeResult.Transaction.GetOrderTransaction(lastPaymentMethod));
            
            return new OrderFullAmountCharged(message.OrderId);
        }

        protected override ValidationModelResult<ChargeForOrderFullAmountRequest> ValidateRequest(
            ChargeForOrderFullAmountRequest message)
        {
            var v = message.Validate()
                .HasData(m => m.OrderId, $"Missing {nameof(ChargeForOrderFullAmountRequest.OrderId)}");
            
            if (message.AlternativePaymentMethod is not null)
                return v.That(m => m.AlternativePaymentMethod.Validate()
                        .PaymentMethodIsValid(p => p!).IsValid(),
                    $"{nameof(ChargeForOrderFullAmountRequest.AlternativePaymentMethod)} must have a valid creditcard");

            return v;
        }

        protected override Task AddPaymentMethod(Order order, StoredPaymentMethod paymentMethod)
        {
            return _orderService.AddPaymentMethod(order, paymentMethod);
        }

        protected override Task<ChargeResult> Charge(decimal amount, string paymentToken)
        {
            return _creditCardPaymentService.Charge(amount, paymentToken);
        }

        protected override Task<ChargeResult> Charge(decimal amount, CreditCard creditCard)
        {
            return _creditCardPaymentService.Charge(amount, creditCard);
        }
    }
}
