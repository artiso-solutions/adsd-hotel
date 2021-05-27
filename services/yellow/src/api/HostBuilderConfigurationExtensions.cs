using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;
using artiso.AdsdHotel.ITOps.NoSql;
using artiso.AdsdHotel.Yellow.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using NServiceBus;

namespace artiso.AdsdHotel.Yellow.Api
{
    internal static class HostBuilderConfigurationExtensions
    {
        public static IHostBuilder ConfigureApp(this IHostBuilder builder)
        {
            builder.UseConsoleLifetime();
            ConfigureOptions(builder);
            builder.ConfigureStorage();
            builder.ConfigureServiceBus();
            builder.ConfigureCustomServices();

            return builder;
        }

        private static void ConfigureOptions(this IHostBuilder builder)
        {
            builder.ConfigureServices(Configure);
            static void Configure(HostBuilderContext ctx, IServiceCollection services)
            {
                var cfg = ctx.Configuration.GetSection(key: nameof(RabbitMqConfig));
                var rabbitCfg = cfg.Get<RabbitMqConfig>();
                services
                    .Configure<RabbitMqConfig>(cfg)
                    .Configure<MongoDbConfig>(ctx.Configuration.GetSection(key: nameof(MongoDbConfig)));
            }
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
            
            static void Configure(HostBuilderContext ctx, IServiceCollection services)
            {
                services.TryAddSingleton(sp =>
                {
                    var config = sp.GetRequiredService<IOptions<MongoDbConfig>>();
                    // ToDo maybe use IOptions in factory
                    return new MongoDbClientFactory(config.Value);
                });
            }
        }
        
        
        
        private static void ConfigureServiceBus(this IHostBuilder builder)
        {
            builder.UseNServiceBus(ctx =>
            {
                var cfg = ctx.Configuration.GetSection(key: nameof(RabbitMqConfig)).Get<RabbitMqConfig>();
                var endpointConfiguration = NServiceBusEndpointConfigurationFactory.Create(
                    endpointName: "Yellow.Api",
                    rabbitMqConnectionString: cfg.Host,//.ToString(),
                    true);

                return endpointConfiguration;
            });
        }
    }
}
