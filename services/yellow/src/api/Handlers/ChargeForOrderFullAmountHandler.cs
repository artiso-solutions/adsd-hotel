using System;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.Yellow.Api.Services;
using artiso.AdsdHotel.Yellow.Api.Validation;
using artiso.AdsdHotel.Yellow.Contracts;
using artiso.AdsdHotel.Yellow.Contracts.Commands;
using artiso.AdsdHotel.Yellow.Events;
using NServiceBus;

namespace artiso.AdsdHotel.Yellow.Api.Handlers
{
    public class ChargeForOrderFullAmountHandler : IHandleMessages<ChargeForOrderFullAmountRequest>
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentOrderAdapter _paymentOrderAdapter;
        
        public ChargeForOrderFullAmountHandler(IOrderService orderService, IPaymentOrderAdapter paymentOrderAdapter)
        {
            _orderService = orderService;
            _paymentOrderAdapter = paymentOrderAdapter;
        }
        
        public async Task Handle(ChargeForOrderFullAmountRequest message, IMessageHandlerContext context)
        {
            try
            {
                var validateResult = ValidateRequest(message);
                if (!validateResult.IsValid())
                    throw new ValidationException(validateResult);
            
                var order = HandlerHelper.Ensure(await _orderService.FindOneById(message.OrderId))!;

                var amountToPay = order.Price.Amount;
            
                var chargeResult = (message.AlternativePaymentMethod is not null)
                    ? await _paymentOrderAdapter.ChargeOrder(order, amountToPay, message.AlternativePaymentMethod)
                    : await _paymentOrderAdapter.ChargeOrder(order, amountToPay);

                if (message.AlternativePaymentMethod is not null)
                    await _paymentOrderAdapter.AddPaymentMethodToOrder(order, message.AlternativePaymentMethod.CreditCard, chargeResult.AuthorizePaymentToken);

                var lastPaymentMethod = order.PaymentMethods!.Last();
                await _orderService.AddTransaction(order, chargeResult.Transaction.GetOrderTransaction(lastPaymentMethod));
            
                await context.Publish(new OrderFullAmountCharged(message.OrderId));
                await context.Reply(new Response<bool>(true));
            }
            catch (Exception e)
            {
                await context.Publish(new ChargeForOrderFullAmountFailed(message.OrderId, e.Message));
                await context.Reply(new Response<bool>(e));
            }
        }

        private ValidationModelResult<ChargeForOrderFullAmountRequest> ValidateRequest(
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
    }
}
