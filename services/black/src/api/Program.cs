using System;
using artiso.AdsdHotel.Black.Api;
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
        var rabbitUri = ctx.Configuration.GetServiceUri("rabbit", "rabbit");
        if (rabbitUri is not null)
        {
            // this blocks further initialization until the rabbitmq instance is running
            services.AddSingleton<IHostedService>(new ProceedIfRabbitMqIsAlive(rabbitUri.Host, rabbitUri.Port));
        }

        var mongoUri = ctx.Configuration.GetServiceUri("mongodb", "mongodb");
        if (mongoUri is not null)
        {
            //string mongoConnectionString = $"{mongoUri.Scheme}://{mongoUri.Host}:{mongoUri.Port}";
            var dbName = ctx.Configuration.GetValue<string>("BLACK_API_DBNAME");
            var collectionName = ctx.Configuration.GetValue<string>("BLACK_API_COLLECTIONNAME");
            services.AddScoped<IDataStoreClient, MongoDataStoreClient>(sp => new MongoDataStoreClient(mongoUri, dbName, collectionName));
        }
    });

    builder.UseNServiceBus(ctx =>
    {
        var rabbitUri = ctx.Configuration.GetServiceUri("rabbit", "rabbit");
        var connectionString = CreateRabbitMqConnectionString(rabbitUri);

        var endpointConfiguration = NServiceBusEndpointConfigurationFactory.Create(
            endpointName: "Black.Api",
            connectionString);

        return endpointConfiguration;
    });

    builder.ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    });

    return builder;
}

static string CreateRabbitMqConnectionString(Uri? uri)
{
    var host = uri?.Host ?? "localhost";
    return $"host={host}";
}