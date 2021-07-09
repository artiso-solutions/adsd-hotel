using System;
using MongoDB.Driver;
using NServiceBus;

namespace artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus
{
    public class NServiceBusEndpointConfigurationFactory
    {
        public static EndpointConfiguration Create(
            string endpointName,
            string? rabbitMqConnectionString = null,
            string? mongoDbConnectionString = null,
            bool useCallbacks = false)
        {
            var config = new EndpointConfiguration(endpointName);

            Configure(config);

            if (rabbitMqConnectionString is not null)
                ConfigureRabbitMq(config, rabbitMqConnectionString);

            if (mongoDbConnectionString is not null)
                ConfigureMongoDb(config, mongoDbConnectionString, endpointName);

            if (useCallbacks)
                ConfigureCallbacks(config, endpointName);

            return config;
        }

        private static void Configure(EndpointConfiguration config)
        {
            config.EnableInstallers();
            config.UseSerialization<NewtonsoftSerializer>();
            config.Conventions().Add(new AdsdHotelMessageConventions());
        }

        private static void ConfigureRabbitMq(EndpointConfiguration config, string rabbitMqConnectionString)
        {
            config.UseTransport<RabbitMQTransport>()
                .UseConventionalRoutingTopology()
                .ConnectionString(rabbitMqConnectionString);
        }

        private static void ConfigureMongoDb(
            EndpointConfiguration config,
            string mongoDbConnectionString,
            string endpointName)
        {
            var persistence = config.UsePersistence<MongoPersistence>(); // or InMemoryPersistence
            persistence.MongoClient(new MongoClient(mongoDbConnectionString));

            // need to disable transaction atm because we don't have a replica set,
            // which is needed for distributed transaction within mongo
            persistence.UseTransactions(false);

            var databaseName = AsDbName(endpointName);
            persistence.DatabaseName(databaseName);

            // Local functions.

            static string AsDbName(string name) =>
                name.ToLower().Replace('.', '-');
        }

        private static void ConfigureCallbacks(EndpointConfiguration config, string endpointName)
        {
            config.EnableCallbacks();
            config.MakeInstanceUniquelyAddressable($"{endpointName}.{Guid.NewGuid()}");
        }
    }
}
