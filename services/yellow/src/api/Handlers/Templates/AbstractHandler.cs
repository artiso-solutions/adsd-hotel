using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.Yellow.Api.Services;
using artiso.AdsdHotel.Yellow.Api.Validation;
using artiso.AdsdHotel.Yellow.Contracts;
using artiso.AdsdHotel.Yellow.Contracts.Commands;
using artiso.AdsdHotel.Yellow.Contracts.Models;
using NServiceBus;

namespace artiso.AdsdHotel.Yellow.Api.Handlers.Templates
{
    public abstract class AbstractHandler<TRequestMessage, TResponseMessage> : IHandleMessages<TRequestMessage>
    {
        public async Task Handle(TRequestMessage message, IMessageHandlerContext context)
        {
            try
            {
                var validateResult = ValidateRequest(message);

                if (!validateResult.IsValid())
                    throw new ValidationException(validateResult);

                var result = await Handle(message);

                Response<TResponseMessage> responseMessage = new(result);

                await context.Publish(responseMessage);
            }
            catch (Exception e)
            {
                var failedResponseMessage = new Response<TResponseMessage>(e);
                
                await context.Publish(failedResponseMessage);
            }
        }

        protected abstract Task<TResponseMessage> Handle(TRequestMessage message);
        
        protected abstract Task AddPaymentMethod(Order order, OrderPaymentMethod paymentMethod);

        protected virtual ValidationModelResult<TRequestMessage> ValidateRequest(TRequestMessage message) => message.Validate();

        protected static async Task<TResult> Ensure<TMessage, TResult>(TMessage m, Func<TMessage, Task<TResult>> func)
        {
            var result = await func(m);

            if (result is null)
                throw new ValidationException($"{typeof(TResult).Name} should not be null");

            return result;
        }
        
        protected static TResult Ensure<TMessage, TResult>(TMessage m, Func<TMessage, TResult> func)
        {
            var result = func(m);

            if (result is null)
                throw new ValidationException($"{typeof(TResult).Name} should not be null");

            return result;
        }

        protected async Task AddPaymentMethodToOrder(Order order, CreditCard creditCard, string? authorizePaymentToken)
        {
            if (string.IsNullOrWhiteSpace(authorizePaymentToken))
                throw new InvalidOperationException($"ChargeResult must contain {nameof(ChargeResult.AuthorizePaymentToken)}");
            
            var orderCreditCard = creditCard.GetOrderCreditCard(authorizePaymentToken);

            await AddPaymentMethod(order, new OrderPaymentMethod(orderCreditCard));
        }
    }
}
