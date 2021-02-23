using System;
using artiso.AdsdHotel.ITOps.Communication;
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
                ConfigureRabbitMq(config, rabbitMqConnectionString);

            return config;
        }

        private static void Configure(EndpointConfiguration config)
        {
            config.EnableCallbacks();
            config.EnableInstallers();

            config.UseSerialization<NewtonsoftSerializer>();
            config.UsePersistence<InMemoryPersistence>();

            config.Conventions()
                .DefiningCommandsAs(t => IsACommand(t) && !IsGenericResponse(t))
                .DefiningMessagesAs(t => IsAResponse(t) || IsGenericResponse(t))
                .DefiningEventsAs(IsAnEvent);

            static bool IsACommand(Type t) =>
                t.Namespace is not null &&
                t.Namespace.EndsWith(".Commands") &&
                !t.Name.EndsWith("Response");

            static bool IsAResponse(Type t) =>
                t.Namespace is not null &&
                t.Namespace.EndsWith(".Commands") &&
                t.Name.EndsWith("Response");

            static bool IsGenericResponse(Type t)
            {
                if (!t.IsGenericType) return false;
                var generic = t.GetGenericTypeDefinition();
                return generic == typeof(Response<>);
            }

            static bool IsAnEvent(Type t) =>
                t.Namespace is not null &&
                t.Namespace.EndsWith(".Events");
        }

        private static void ConfigureRabbitMq(EndpointConfiguration config, string rabbitMqConnectionString)
        {
            config.UseTransport<RabbitMQTransport>()
                .UseConventionalRoutingTopology()
                .ConnectionString(rabbitMqConnectionString);
        }
    }
}
