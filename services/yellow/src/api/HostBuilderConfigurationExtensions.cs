using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;
using artiso.AdsdHotel.Yellow.Api.Configuration;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace artiso.AdsdHotel.Yellow.Api
{
    internal static class HostBuilderConfigurationExtensions
    {
        public static IHostBuilder ConfigureApp(this IHostBuilder builder)
        {
            var busConfiguration = AppSettingsHelper.GetSettings<RabbitMqConfig>();
            
            builder.UseConsoleLifetime();
        
            builder.UseNServiceBus(ctx =>
            {
                var endpointConfiguration = NServiceBusEndpointConfigurationFactory.Create(
                    endpointName: "Yellow.Api",
                    rabbitMqConnectionString: busConfiguration.ToString(), 
                    true);
        
                return endpointConfiguration;
            });
        
            return builder;
        }
    }
}
