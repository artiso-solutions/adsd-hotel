using artiso.AdsdHotel.Blue.Api;
using artiso.AdsdHotel.ITOps.Communication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureApp()
    .Build();

using (var scope = host.Services.CreateScope())
{
    var readinessProbe = scope.ServiceProvider.GetRequiredService<RabbitMqReadinessProbe>();
    _ = await readinessProbe.IsServiceAliveAsync();

    await Startup.SetupDatabaseAsync(scope.ServiceProvider);
}

await host.RunAsync();
