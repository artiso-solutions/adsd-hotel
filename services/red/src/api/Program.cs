using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.Red.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureApp()
    .Build();

using (var scope = host.Services.CreateScope())
{
    var readinessProbe = scope.ServiceProvider.GetRequiredService<RabbitMqReadinessProbe>();
    _ = await readinessProbe.IsServiceAliveAsync();
}

await host.RunAsync();
