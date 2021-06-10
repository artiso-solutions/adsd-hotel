using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;
using artiso.AdsdHotel.Yellow.Api.Validation;
using NServiceBus;

namespace artiso.AdsdHotel.Yellow.Api.Handlers.Templates
{
    public abstract class AbstractHandler<TRequestMessage, TResponseEvent> : IHandleMessages<TRequestMessage>
         where TResponseEvent : class
    {
        public async Task Handle(TRequestMessage message, IMessageHandlerContext context)
        {
            var conventions = new AdsdHotelMessageConventions();

            try
            {
                var validateResult = ValidateRequest(message);

                if (!validateResult.IsValid())
                    throw new ValidationException(validateResult);

                var response = await Handle(message);
                response.ToString();

                if (conventions.IsEventType(response.GetType()))
                {
                    await context.Publish(response);
                }
                else
                {
                    await context.Reply(new Response<TResponseEvent>(response));
                }
            }
            catch (Exception e)
            {
                var failResponse = Fail(message);

                if (conventions.IsEventType(failResponse.GetType()))
                {
                    await context.Publish(failResponse);
                }
                else
                {
                    await context.Reply(new Response<TResponseEvent>(e));
                }
            }
        }

        protected abstract object Fail(TRequestMessage requestMessage);

        protected abstract Task<TResponseEvent> Handle(TRequestMessage message);
        
        protected virtual ValidationModelResult<TRequestMessage> ValidateRequest(TRequestMessage message) => message.Validate();

        protected static async Task<TResult> Ensure<TMessage, TResult>(TMessage m, Func<TMessage, Task<TResult?>> func)
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
    }
}
