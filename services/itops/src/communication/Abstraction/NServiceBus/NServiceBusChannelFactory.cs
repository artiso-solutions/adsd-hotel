using NServiceBus;

namespace artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus
{
    public class NServiceBusChannelFactory
    {
        public static IChannel Create(
            string channelName,
            string endpointDestination,
            string rabbitMqConnectionString)
        {
            var config = NServiceBusEndpointConfigurationFactory.Create(channelName, rabbitMqConnectionString);

            var holder = new EndpointHolder(Endpoint.Start(config));
            var channel = new NServiceBusChannel(holder, endpointDestination);

            return channel;
        }
    }
}
