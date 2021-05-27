using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace artiso.AdsdHotel.Blue.Api
{
    internal static class HostBuilderConfigurationExtensions
    {
        public static IHostBuilder ConfigureApp(this IHostBuilder builder)
        {
            builder.UseConsoleLifetime();

            builder.ConfigureServices(Startup.ConfigureServices);

            builder.UseNServiceBus(ctx =>
            {
                var rabbitMqConfig = ctx.Configuration.GetSection(key: nameof(RabbitMqConfig)).Get<RabbitMqConfig>();

                var endpointConfiguration = NServiceBusEndpointConfigurationFactory.Create(
                    endpointName: "Blue.Api",
                    rabbitMqConnectionString: rabbitMqConfig.AsConnectionString(),
                    true);

                return endpointConfiguration;
            });

            return builder;
        }
    }
}
