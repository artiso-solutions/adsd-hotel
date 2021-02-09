using System;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.Yellow.Api.Services;
using artiso.AdsdHotel.Yellow.Api.Validation;
using artiso.AdsdHotel.Yellow.Contracts.Commands;
using artiso.AdsdHotel.Yellow.Contracts.Models;
using artiso.AdsdHotel.Yellow.Events;

namespace artiso.AdsdHotel.Yellow.Api.Handlers
{
    public class ChargeForOrderCancellationFeeHandler : AbstractHandler<ChargeForOrderCancellationFeeRequest, OrderCancellationFeeCharged>
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
            var order = await Ensure(message, m => _orderService.FindOneById(m.OrderId));

            var amountToPay = order.Price.CancellationFee;

            if (message.AlternativePaymentMethod is not null)
                await ChargeOrder(order, amountToPay, message.AlternativePaymentMethod);
            else
                await ChargeOrder(order, amountToPay);

            return new OrderCancellationFeeCharged(message.OrderId);
        }

        private async Task ChargeOrder(Order order, decimal amount)
        {
            var paymentMethods = Ensure(order, o => o.OrderPaymentMethods);
            var paymentMethod = paymentMethods!.LastOrDefault();
            var payToken = paymentMethod!.CreditCard.ProviderPaymentToken;
            
            var paymentResult = await _paymentService.Charge(amount, payToken!);
            
            if (paymentResult.IsSuccess != true)
                throw paymentResult.Exception ?? new InvalidOperationException($"{nameof(paymentResult)}");
        }

        private async Task ChargeOrder(Order order, decimal amount, PaymentMethod alternativePaymentMethod)
        {
            if (alternativePaymentMethod.CreditCard is null)
                throw new ValidationException($"Alternative {nameof(ChargeForOrderCancellationFeeRequest.AlternativePaymentMethod)} must have a CreditCard");

            if (!alternativePaymentMethod.Validate().PaymentMethodIsValid(p => p).IsValid())
                throw new ValidationException(
                    $"Alternative {nameof(ChargeForOrderCancellationFeeRequest.AlternativePaymentMethod)} must have a valid CreditCard");

            var chargeResult = await _paymentService.Charge(amount, alternativePaymentMethod.CreditCard);
            if (!chargeResult.IsSuccess)
                throw chargeResult.Exception ?? new InvalidOperationException($"ChargeOperation failed for order {order.Id}");
            
            await AddPaymentMethodToOrder(_orderService, order, alternativePaymentMethod.CreditCard, chargeResult.AuthorizePaymentToken);
        }

        protected override ValidationModelResult<ChargeForOrderCancellationFeeRequest> ValidateRequest(
            ChargeForOrderCancellationFeeRequest message)
        {
            return message.Validate()
                .HasData(m => m.OrderId, $"{nameof(ChargeForOrderCancellationFeeRequest.OrderId)} is empty");
        }
    }
}
