using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;
using artiso.AdsdHotel.ITOps.NoSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace artiso.AdsdHotel.Purple.Api
{
    internal static class HostBuilderConfigurationExtensions
    {
        public static IHostBuilder ConfigureApp(this IHostBuilder builder)
        {
            builder.UseConsoleLifetime();

            builder.ConfigureServices((ctx, services) =>
            {
                services.AddSingleton<RabbitMqReadinessProbe>();
                services.Configure<RabbitMqConfig>(ctx.Configuration.GetSection(key: nameof(RabbitMqConfig)));
                services.Configure<MongoDbConfig>(ctx.Configuration.GetSection(key: nameof(MongoDbConfig)));
            });

            builder.UseNServiceBus(ctx =>
            {
                var rabbitMqConfig = ctx.Configuration.GetSection(key: nameof(RabbitMqConfig)).Get<RabbitMqConfig>();
                var mongoDbConfig = ctx.Configuration.GetSection(key: nameof(MongoDbConfig)).Get<MongoDbConfig>();

                var endpointConfiguration = NServiceBusEndpointConfigurationFactory.Create(
                    endpointName: "Purple.Api",
                    rabbitMqConnectionString: rabbitMqConfig.AsConnectionString(),
                    mongoDbConnectionString: mongoDbConfig.AsConnectionString(),
                    true);

                return endpointConfiguration;
            });

            return builder;
        }
    }
}
