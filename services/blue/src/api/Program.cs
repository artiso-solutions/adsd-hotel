using artiso.AdsdHotel.Blue.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(config =>
    {
        config.AddEnvironmentVariables();
    })
    .ConfigureApp()
    .Build();

using var scope = host.Services.CreateScope();
await Startup.SetupDatabaseAsync(scope.ServiceProvider);

await host.RunAsync();
