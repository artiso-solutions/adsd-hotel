using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace artiso.AdsdHotel.Blue.Api
{
    internal static class HostBuilderConfigurationExtensions
    {
        public static IHostBuilder ConfigureApp(this IHostBuilder builder)
        {
            builder.UseConsoleLifetime();
            ConfigureOptions(builder); 
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

        private static void ConfigureOptions(this IHostBuilder builder)
        {
            builder.ConfigureServices(Configure);
            static void Configure(HostBuilderContext ctx, IServiceCollection services)
            {
                services
                    .Configure<RabbitMqConfig>(ctx.Configuration.GetSection(key: nameof(RabbitMqConfig)));
            }
        }
    }
}
