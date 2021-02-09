using System.Threading;
using System.Threading.Tasks;
using NServiceBus;

namespace artiso.AdsdHotel.ITOps.Abstraction.NServiceBus
{
    public class NServiceBusChannel : IChannel
    {
        private readonly EndpointHolder _holder;
        private readonly string _destination;

        public NServiceBusChannel(
            EndpointHolder holder, string destination)
        {
            _holder = holder;
            _destination = destination;
        }

        public async Task Send(object command)
        {
            await _holder.EndpointReady.ConfigureAwait(false);
            await _holder.Endpoint.Send(_destination, command).ConfigureAwait(false);
        }

        public async Task<TResponse> Request<TResponse>(object request, CancellationToken cancellationToken)
        {
            await _holder.EndpointReady.ConfigureAwait(false);
            
            var response = await _holder.Endpoint.Request<TResponse>(
                request, GetOptions(), cancellationToken).ConfigureAwait(false);

            return response;
        }

        public async Task Publish(object evt)
        {
            await _holder.EndpointReady.ConfigureAwait(false);
            await _holder.Endpoint.Publish(evt).ConfigureAwait(false);
        }

        public ValueTask DisposeAsync() =>
            _holder.DisposeAsync();

        private SendOptions GetOptions()
        {
            var options = new SendOptions();
            options.SetDestination(_destination);
            return options;
        }
    }
}
