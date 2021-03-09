using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.Yellow.Api.Validation;
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

                //await context.Publish(responseMessage);
                await context.Reply(responseMessage);
            }
            catch (Exception e)
            {
                var failedResponseMessage = new Response<TResponseMessage>(e);
                
                await context.Publish(failedResponseMessage);
                await context.Reply(failedResponseMessage);
            }
        }

        protected abstract Task<TResponseMessage> Handle(TRequestMessage message);
        
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
