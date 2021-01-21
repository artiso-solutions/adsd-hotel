using System;
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
            var config = new EndpointConfiguration(channelName);
            config.MakeInstanceUniquelyAddressable($"{channelName}.{Guid.NewGuid()}");

            NServiceBusEndpointConfigurator.Configure(config);

            config.UseTransport<RabbitMQTransport>()
                .UseConventionalRoutingTopology()
                .ConnectionString(rabbitMqConnectionString);

            var holder = new EndpointHolder(Endpoint.Start(config));
            var channel = new NServiceBusChannel(holder, endpointDestination);

            return channel;
        }
    }
}
