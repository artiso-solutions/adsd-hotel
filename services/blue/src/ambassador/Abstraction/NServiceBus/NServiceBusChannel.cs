using System.Threading;
using System.Threading.Tasks;
using NServiceBus;

namespace artiso.AdsdHotel.Blue.Ambassador
{
    public class NServiceBusChannel : IChannel
    {
        private readonly EndpointHolder _holder;
        private readonly string _destination;
        private readonly SendOptions _sendOptions;

        public NServiceBusChannel(
            EndpointHolder holder, string destination)
        {
            _holder = holder;
            _destination = destination;
            
            _sendOptions = new SendOptions();
            _sendOptions.SetDestination(destination);
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
                request, _sendOptions, cancellationToken).ConfigureAwait(false);
            return response;
        }

        public async Task Publish(object evt)
        {
            await _holder.EndpointReady.ConfigureAwait(false);
            await _holder.Endpoint.Publish(evt).ConfigureAwait(false);
        }

        public ValueTask DisposeAsync() =>
            _holder.DisposeAsync();
    }
}
