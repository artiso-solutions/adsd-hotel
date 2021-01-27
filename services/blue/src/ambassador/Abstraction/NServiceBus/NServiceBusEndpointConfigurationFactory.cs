using System;
using NServiceBus;

namespace artiso.AdsdHotel.Blue.Ambassador
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
                .DefiningCommandsAs(t =>
                    t.Namespace is not null &&
                    t.Namespace.EndsWith(".Commands") &&
                    !t.Name.EndsWith("Response"))
                .DefiningMessagesAs(t =>
                    t.Namespace is not null &&
                    t.Namespace.EndsWith(".Commands") &&
                    t.Name.EndsWith("Response"))
                .DefiningEventsAs(t =>
                    t.Namespace is not null &&
                    t.Namespace.EndsWith(".Events"));
        }

        private static void ConfigureRabbitMQ(EndpointConfiguration config, string rabbitMqConnectionString)
        {
            config.UseTransport<RabbitMQTransport>()
                .UseConventionalRoutingTopology()
                .ConnectionString(rabbitMqConnectionString);
        }
    }
}
