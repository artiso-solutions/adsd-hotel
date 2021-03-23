using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;
using Microsoft.Extensions.Hosting;
using NServiceBus;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureApp()
    .Build();


await host.RunAsync();


internal static class HostBuilderConfigurationExtensions
{
    public static IHostBuilder ConfigureApp(this IHostBuilder builder)
    {
        builder.UseConsoleLifetime();

        builder.UseNServiceBus(ctx =>
        {
            var endpointConfiguration = NServiceBusEndpointConfigurationFactory.Create(
                endpointName: "Purple.Api",
                rabbitMqConnectionString: "host=localhost");

            return endpointConfiguration;
        });

        return builder;
    }
}
