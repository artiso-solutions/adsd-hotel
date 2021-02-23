using artiso.AdsdHotel.ITOps.Abstraction.NServiceBus;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace artiso.AdsdHotel.Red.Service
{
    internal static class HostBuilderConfigurationExtensions
    {
        public static IHostBuilder ConfigureNServiceBus(this IHostBuilder builder)
        {
            builder.UseNServiceBus(_ =>
            {
                var endpointConfiguration = NServiceBusEndpointConfigurationFactory.Create(
                    endpointName: "Red.Api",
                    rabbitMqConnectionString: "host=localhost");

                return endpointConfiguration;
            });

            return builder;
        }
    }
}
