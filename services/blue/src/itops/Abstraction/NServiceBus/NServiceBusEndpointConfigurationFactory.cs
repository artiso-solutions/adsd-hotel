using System;
using NServiceBus;

namespace artiso.AdsdHotel.ITOps.Abstraction.NServiceBus
{
    public class NServiceBusEndpointConfigurationFactory
    {
        public static EndpointConfiguration Create(
            string endpointName,
            string? rabbitMqConnectionString = null)
        {
            var config = new EndpointConfiguration(endpointName);

            config.MakeInstanceUniquelyAddressable($"{endpointName}.{Guid.NewGuid()}");

            Configure(config);

            if (rabbitMqConnectionString is not null)
                ConfigureRabbitMQ(config, rabbitMqConnectionString);

            return config;
        }

        private static void Configure(EndpointConfiguration config)
        {
            config.EnableCallbacks();
            config.EnableInstallers();

            config.UseSerialization<NewtonsoftSerializer>();
            config.UsePersistence<InMemoryPersistence>();

            config.Conventions()
                .Add(new AdsdHotelMessageConventions());
        }

        private static void ConfigureRabbitMQ(EndpointConfiguration config, string rabbitMqConnectionString)
        {
            config.UseTransport<RabbitMQTransport>()
                .UseConventionalRoutingTopology()
                .ConnectionString(rabbitMqConnectionString);
        }
    }
}
