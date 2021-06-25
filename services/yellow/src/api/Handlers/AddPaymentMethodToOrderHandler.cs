using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.Yellow.Api.Services;
using artiso.AdsdHotel.Yellow.Api.Validation;
using artiso.AdsdHotel.Yellow.Contracts;
using artiso.AdsdHotel.Yellow.Contracts.Commands;
using artiso.AdsdHotel.Yellow.Contracts.Models;
using artiso.AdsdHotel.Yellow.Events;
using NServiceBus;

namespace artiso.AdsdHotel.Yellow.Api.Handlers
{
    public class AddPaymentMethodToOrderHandler : IHandleMessages<AddPaymentMethodToOrderRequest>
    {
        private readonly IOrderService _orderService;
        private readonly ICreditCardPaymentService _paymentService;

        public AddPaymentMethodToOrderHandler(IOrderService orderService, ICreditCardPaymentService paymentService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
        }

        public async Task Handle(AddPaymentMethodToOrderRequest message, IMessageHandlerContext context)
        {
            try
            {
                var validateResult = ValidateRequest(message);
                if (!validateResult.IsValid())
                    throw new ValidationException(validateResult);
            
                Order order = HandlerHelper.Ensure(await _orderService.FindOneById(message.OrderId))!;
            
                var paymentToken = await _paymentService.GetPaymentToken(message.PaymentMethod.CreditCard!);
                var orderPaymentMethod = new StoredPaymentMethod(message.PaymentMethod.CreditCard!.GetOrderCreditCard(paymentToken));
            
                await _orderService.AddPaymentMethod(order, orderPaymentMethod);
            
                await context.Publish(new PaymentMethodToOrderAdded(message.OrderId));
                await context.Reply(new Response<bool>(true));
            }
            catch (Exception e)
            {
                await context.Publish(new AddPaymentMethodToOrderFailed(message.OrderId, e.Message));
                await context.Reply(new Response<bool>(e));
            }
        }

        private ValidationModelResult<AddPaymentMethodToOrderRequest> ValidateRequest(AddPaymentMethodToOrderRequest message)
        {
            return message.Validate()
                .HasData(r => r.OrderId, 
                    $"{nameof(AddPaymentMethodToOrderRequest.OrderId)} should contain data")
                .NotNull(r => r.PaymentMethod, 
                    $"{nameof(AddPaymentMethodToOrderRequest.PaymentMethod)} should not be null")
                .NotNull(r => r.PaymentMethod.CreditCard, 
                    $"{nameof(AddPaymentMethodToOrderRequest.PaymentMethod.CreditCard)} should not be null")
                .PaymentMethodIsValid(r => r.PaymentMethod);
        }
    }
}
