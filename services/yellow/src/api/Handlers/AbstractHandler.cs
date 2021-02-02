using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.Yellow.Api.Validation;
using artiso.AdsdHotel.Yellow.Contracts.Commands;
using NServiceBus;

namespace artiso.AdsdHotel.Yellow.Api.Handlers
{
    public abstract class AbstractHandler<TRequestMessage, TResponseMessage> : IHandleMessages<TRequestMessage>
    {
        public async Task Handle(TRequestMessage message, IMessageHandlerContext context)
        {
            try
            {
                var validateResult = Validate(message);

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

        protected virtual ValidationModelResult<TRequestMessage> Validate(TRequestMessage message) => message.Validate();
    }
}