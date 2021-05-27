using artiso.AdsdHotel.Black.Api;
using artiso.AdsdHotel.Black.Contracts;
using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;
using artiso.AdsdHotel.ITOps.NoSql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;

await CreateHostBuilder(args).Build().RunAsync();

static IHostBuilder CreateHostBuilder(string[] args)
{
    var builder = Host.CreateDefaultBuilder(args);

    builder.ConfigureServices((ctx, services) =>
    {
        services
            .Configure<RabbitMqConfig>(ctx.Configuration.GetSection(key: nameof(RabbitMqConfig)))
            .Configure<MongoDbConfig>(ctx.Configuration.GetSection(key: nameof(MongoDbConfig)));

        var rabbitMqConfig = ctx.Configuration.GetSection(key: nameof(RabbitMqConfig)).Get<RabbitMqConfig>();

        // This blocks further initialization until the rabbitmq instance is running
        // TODO: move in ITOps
        services.AddSingleton<IHostedService>(new ProceedIfRabbitMqIsAlive(rabbitMqConfig.Host, 5672));

        services.AddSingleton<MongoDbClientFactory>();

        services.AddScoped<IDataStoreClient, MongoDataStoreClient>(sp =>
        {
            var mongoDbClientFactory = sp.GetRequiredService<MongoDbClientFactory>();
            return mongoDbClientFactory.GetClient(typeof(GuestInformation));
        });
    });

    builder.UseNServiceBus(ctx =>
    {
        var rabbitMqConfig = ctx.Configuration.GetSection(key: nameof(RabbitMqConfig)).Get<RabbitMqConfig>();

        var endpointConfiguration = NServiceBusEndpointConfigurationFactory.Create(
            endpointName: "Black.Api",
            rabbitMqConnectionString: rabbitMqConfig.AsConnectionString(),
            true);

        return endpointConfiguration;
    });

    builder.ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    });

    return builder;
}