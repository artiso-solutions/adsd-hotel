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
                var config = ctx.Configuration;

                var endpointConfiguration = NServiceBusEndpointConfigurationFactory.Create(
                    endpointName: "Blue.Api",
                    rabbitMqConnectionString: config.GetValue("rabbitmq:cs", defaultValue: "host=localhost"),
                    true);

                return endpointConfiguration;
            });

            return builder;
        }
    }
}
