using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;
using artiso.AdsdHotel.Red.Api.Configuration;
using artiso.AdsdHotel.Red.Persistence;
using artiso.AdsdHotel.Red.Persistence.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using NServiceBus;

namespace artiso.AdsdHotel.Red.Api
{
    internal static class HostBuilderConfigurationExtensions
    {
        public static IHostBuilder ConfigureApp(this IHostBuilder builder)
        {
            ConfigureStorage(builder);
            ConfigureServiceBus(builder);
            ConfigureCustomServices(builder);

            return builder;
        }

        private static void ConfigureServiceBus(this IHostBuilder builder)
        {
            var busConfiguration = AppSettingsHelper.GetSettings<RabbitMqConfig>();

            builder.UseConsoleLifetime();
            builder.UseNServiceBus(ctx =>
            {
                var endpointConfiguration = NServiceBusEndpointConfigurationFactory.Create(
                    endpointName: "Red.Api",
                    rabbitMqConnectionString: busConfiguration.ToString(),
                    true);

                return endpointConfiguration;
            });
        }

        private static void ConfigureStorage(this IHostBuilder builder)
        {
            builder.ConfigureServices(Configure);

            // Internal functions

            static void Configure(IServiceCollection services)
            {
                services.TryAddSingleton(sp =>
                {
                    var config = AppSettingsHelper.GetSettings<MongoDbConfig>();
                    return new MongoDBClientFactory(config);
                });

                var conventions = new ConventionPack
                {
                    new IgnoreExtraElementsConvention(true),
                    new CamelCaseElementNameConvention(),
                    new EnumRepresentationConvention(BsonType.String),
                };

                ConventionRegistry.Register("DefaultConventions", conventions, filter: type => true);
            }
        }

        private static void ConfigureCustomServices(this IHostBuilder builder)
        {
            builder.ConfigureServices(Configure);

            // Internal functions

            static void Configure(IServiceCollection services)
            {
                services.TryAddSingleton<IRoomPriceService, RoomPriceService>();
            }
        }
    }
}
