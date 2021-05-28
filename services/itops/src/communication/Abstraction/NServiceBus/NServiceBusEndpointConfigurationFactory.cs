using System;
using NServiceBus;

namespace artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus
{
    public class NServiceBusEndpointConfigurationFactory
    {
        public static EndpointConfiguration Create(
            string endpointName,
            string? rabbitMqConnectionString = null,
            bool useCallbacks = false)
        {
            var config = new EndpointConfiguration(endpointName);

            Configure(config);

            if (rabbitMqConnectionString is not null)
                ConfigureRabbitMq(config, rabbitMqConnectionString);

            if (useCallbacks)
                ConfigureCallbacks(config, endpointName);

            return config;
        }

        private static void Configure(EndpointConfiguration config)
        {
            config.EnableInstallers();
            
            config.UseSerialization<NewtonsoftSerializer>();
            config.UsePersistence<InMemoryPersistence>();

            config.Conventions()
                .Add(new AdsdHotelMessageConventions());
        }

        private static void ConfigureCallbacks(EndpointConfiguration config, string endpointName)
        {
            config.EnableCallbacks();
            config.MakeInstanceUniquelyAddressable($"{endpointName}.{Guid.NewGuid()}");
        }

        private static void ConfigureRabbitMq(EndpointConfiguration config, string rabbitMqConnectionString)
        {
            config.UseTransport<RabbitMQTransport>()
                .UseConventionalRoutingTopology()
                .ConnectionString(rabbitMqConnectionString);
        }
    }
}
