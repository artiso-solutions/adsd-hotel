using NServiceBus;

namespace artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus
{
    public class NServiceBusChannelFactory
    {
        public static IChannel Create(
            string channelName,
            string endpointDestination,
            string rabbitMqConnectionString,
            bool useCallbacks = false)
        {
            var config = NServiceBusEndpointConfigurationFactory.Create(channelName, rabbitMqConnectionString, useCallbacks);

            var holder = new EndpointHolder(Endpoint.Start(config));
            var channel = new NServiceBusChannel(holder, endpointDestination);

            return channel;
        }
    }
}
