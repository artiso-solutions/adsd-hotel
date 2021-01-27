using NServiceBus;

namespace artiso.AdsdHotel.Blue.Ambassador
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
