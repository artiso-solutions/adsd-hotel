using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.Yellow.Api.Validation;
using NServiceBus;

namespace artiso.AdsdHotel.Yellow.Api.Handlers.Templates
{
    public abstract class AbstractHandler<TRequestMessage, TResponseEvent> : IHandleMessages<TRequestMessage>
    {
        public async Task Handle(TRequestMessage message, IMessageHandlerContext context)
        {
            try
            {
                var validateResult = ValidateRequest(message);

                if (!validateResult.IsValid())
                    throw new ValidationException(validateResult);

                var eventMessage = await Handle(message);
                
                await context.Publish(eventMessage);

                Response<TResponseEvent> response = new(eventMessage);
                
                await context.Reply(response);
            }
            catch (Exception e)
            {
                var eventMessage = Fail(message);
                
                await context.Publish(eventMessage);
                
                var failedResponseMessage = new Response<TResponseEvent>(e);
                
                await context.Reply(failedResponseMessage);
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
