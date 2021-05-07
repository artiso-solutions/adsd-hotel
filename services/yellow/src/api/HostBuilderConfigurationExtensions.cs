using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;
using artiso.AdsdHotel.Yellow.Api.Configuration;
using artiso.AdsdHotel.Yellow.Api.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using NServiceBus;

namespace artiso.AdsdHotel.Yellow.Api
{
    internal static class HostBuilderConfigurationExtensions
    {
        public static IHostBuilder ConfigureApp(this IHostBuilder builder)
        {
            builder.ConfigureStorage();
            builder.ConfigureServiceBus();
            builder.ConfigureCustomServices();
            
            return builder;
        }

        private static void ConfigureCustomServices(this IHostBuilder builder)
        {
            builder.ConfigureServices(Configure);
            
            // Internal functions
            
            static void Configure(IServiceCollection services)
            {
                services.TryAddSingleton<IOrderService, OrderService>();
                services.TryAddSingleton<ICreditCardPaymentService, CreditCardPaymentService>();
            }
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
        
        
        
        private static void ConfigureServiceBus(this IHostBuilder builder)
        {
            var busConfiguration = AppSettingsHelper.GetSettings<RabbitMqConfig>();
            
            builder.UseConsoleLifetime();
            builder.UseNServiceBus(ctx =>
            {
                var endpointConfiguration = NServiceBusEndpointConfigurationFactory.Create(
                    endpointName: "Yellow.Api",
                    rabbitMqConnectionString: busConfiguration.Host,//.ToString(),
                    true);

                return endpointConfiguration;
            });
        }
    }
}
