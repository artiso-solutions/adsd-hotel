using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;
using artiso.AdsdHotel.ITOps.NoSql;
using artiso.AdsdHotel.Yellow.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace artiso.AdsdHotel.Yellow.Api
{
    internal static class HostBuilderConfigurationExtensions
    {
        public static IHostBuilder ConfigureApp(this IHostBuilder builder)
        {
            builder.UseConsoleLifetime();
            builder.ConfigureOptions();
            builder.ConfigureStorage();
            builder.ConfigureServiceBus();
            builder.ConfigureCustomServices();

            return builder;
        }

        private static void ConfigureOptions(this IHostBuilder builder)
        {
            builder.ConfigureServices(Configure);

            // Internal functions

            static void Configure(HostBuilderContext ctx, IServiceCollection services)
            {
                services
                    .Configure<RabbitMqConfig>(ctx.Configuration.GetSection(key: nameof(RabbitMqConfig)))
                    .Configure<MongoDbConfig>(ctx.Configuration.GetSection(key: nameof(MongoDbConfig)));
            }
        }

        private static void ConfigureCustomServices(this IHostBuilder builder)
        {
            builder.ConfigureServices(Configure);

            // Internal functions

            static void Configure(IServiceCollection services)
            {
                services.AddSingleton<RabbitMqReadinessProbe>();
                services.AddSingleton<IOrderService, OrderService>();
                services.AddSingleton<ICreditCardPaymentService, CreditCardPaymentService>();
            }
        }

        private static void ConfigureStorage(this IHostBuilder builder)
        {
            builder.ConfigureServices(Configure);

            // Internal functions

            static void Configure(HostBuilderContext ctx, IServiceCollection services)
            {
                services.AddSingleton<MongoDbClientFactory>();
            }
        }

        private static void ConfigureServiceBus(this IHostBuilder builder)
        {
            builder.UseNServiceBus(ctx =>
            {
                var rabbitMqConfig = ctx.Configuration.GetSection(key: nameof(RabbitMqConfig)).Get<RabbitMqConfig>();

                var endpointConfiguration = NServiceBusEndpointConfigurationFactory.Create(
                    endpointName: "Yellow.Api",
                    rabbitMqConnectionString: rabbitMqConfig.AsConnectionString(),
                    true);

                return endpointConfiguration;
            });
        }
    }
}
