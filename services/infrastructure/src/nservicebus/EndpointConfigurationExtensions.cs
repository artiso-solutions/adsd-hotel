using System;
using NServiceBus;

namespace artiso.AdsdHotel.Infrastructure.NServiceBus
{
    public static class NServiceBusEndpointConfigurationExtensions
    {
        /// <summary>
        /// Configures the defaults for the NServiceBus endpoints. <br/>
        /// - enables installers <br/>
        /// - use NewtonsoftSerializer <br/>
        /// - configures default conventions <br/>
        /// - use InMemoryPersistence <br/>
        /// - use RabbitMQTransport with conventional routing <br/>
        /// - configures the routing if endpoint and types are passed 
        /// </summary>
        /// <param name="config">The endpoint configuration.</param>
        /// <param name="rabbitMqConnectionString">The connection string for the rabbitMQ endpoint.</param>
        /// <param name="targetEndpoint">The name of the endpoint where messages should be sent to.</param>
        /// <param name="typesToRoute">The types of messages which are routed to <paramref name="targetEndpoint"/>. </param>
        /// <returns>The configured endpoint configuration.</returns>
        public static EndpointConfiguration ConfigureDefaults(
            this EndpointConfiguration config, 
            string rabbitMqConnectionString, 
            string? targetEndpoint, 
            params Type[] typesToRoute)
        {
            if (string.IsNullOrEmpty(rabbitMqConnectionString))
                throw new InvalidOperationException($"{nameof(rabbitMqConnectionString)} cannot be empty.");

            config.EnableInstallers();
            config.UseSerialization<NewtonsoftSerializer>();
            config.ConfigureConventions();
            config.UsePersistence<InMemoryPersistence>();
            var senderTransport = config.UseTransport<RabbitMQTransport>();
            senderTransport.UseConventionalRoutingTopology();
            senderTransport.ConnectionString(rabbitMqConnectionString);
            var routing = senderTransport.Routing();
            if (typesToRoute is { Length: > 0})
            {
                if (string.IsNullOrEmpty(targetEndpoint))
                {
                    throw new InvalidOperationException($"If types to route are set, {nameof(targetEndpoint)} cannot be empty.");
                }
                foreach (var item in typesToRoute)
                {
                    routing.RouteToEndpoint(item, targetEndpoint);
                    routing.RouteToEndpoint(item, targetEndpoint);
                }
            }
            return config;
        }

        /// <summary>
        /// Configures the NServiceBus endpoint to support callbacks. Use this in <b>client-side</b> endpoints!
        /// </summary>
        /// <param name="config">The endpoint configuration.</param>
        /// <param name="uniqueEndpointName">Mandatory unique name for this endpoint, so callbacks can be delivered correctly.</param>
        /// <returns>The endpoint configuration.</returns>
        public static EndpointConfiguration WithClientCallbacks(this EndpointConfiguration config, string uniqueEndpointName)
        {
            if (string.IsNullOrEmpty(uniqueEndpointName))
            {
                throw new InvalidOperationException($"Non-empty {nameof(uniqueEndpointName)} must be present.");
            }
            config.EnableCallbacks(true);
            config.MakeInstanceUniquelyAddressable(uniqueEndpointName);
            return config;
        }

        /// <summary>
        /// Configures the NServiceBus endpoint to support <b>server-side</b> callbacks.
        /// </summary>
        /// <param name="config">The endpoint configuration.</param>
        /// <returns>The endpoint configuration.</returns>
        public static EndpointConfiguration WithServerCallbacks(this EndpointConfiguration config)
        {
            config.EnableCallbacks(false);
            return config;
        }

        private static ConventionsBuilder ConfigureConventions(this EndpointConfiguration config)
        {
            var conventions = config.Conventions();
            conventions.Add(new AdsdHotelMessageConventions());
            return conventions;
        }
    }
}
