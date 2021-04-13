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
    public class ChargeForOrderCancellationFeeHandler : AbstractChargeHandler<ChargeForOrderCancellationFeeRequest, OrderCancellationFeeCharged>
    {
        private readonly IOrderService _orderService;
        private readonly ICreditCardPaymentService _paymentService;

        public ChargeForOrderCancellationFeeHandler(IOrderService orderService,
            ICreditCardPaymentService paymentService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
        }

        protected override async Task<OrderCancellationFeeCharged> Handle(ChargeForOrderCancellationFeeRequest message)
        {
            Order order = await Ensure(message, m => _orderService.FindOneById(m.OrderId));

            var amountToPay = order.Price.CancellationFee;

            var chargeResult = (message.AlternativePaymentMethod is not null)
                ? await ChargeOrder(order, amountToPay, message.AlternativePaymentMethod)
                : await ChargeOrder(order, amountToPay);
            
            if (message.AlternativePaymentMethod is not null)
                await AddPaymentMethodToOrder(order, message.AlternativePaymentMethod.CreditCard, chargeResult.AuthorizePaymentToken);

            // TODO : Should go in a separate handler (and should retrieve it from the used payment token)
            var lastPaymentMethod = order.PaymentMethods!.Last();
            await _orderService.AddTransaction(order, chargeResult.Transaction.GetOrderTransaction(lastPaymentMethod));

            return new OrderCancellationFeeCharged(message.OrderId);
        }
        
        protected override object Fail(ChargeForOrderCancellationFeeRequest requestMessage)
        {
            return new ChargeOrderCancellationFeeFailed(requestMessage.OrderId);
        }
        
        protected override ValidationModelResult<ChargeForOrderCancellationFeeRequest> ValidateRequest(
            ChargeForOrderCancellationFeeRequest message)
        {
            var v = message.Validate()
                .HasData(m => m.OrderId, $"{nameof(ChargeForOrderCancellationFeeRequest.OrderId)} is empty");
            
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
            return _paymentService.Charge(amount, paymentToken);
        }

        protected override Task<ChargeResult> Charge(decimal amount, CreditCard creditCard)
        {
            return _paymentService.Charge(amount, creditCard);
        }
    }
}
