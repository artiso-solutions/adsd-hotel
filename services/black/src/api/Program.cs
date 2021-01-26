using System;
using System.Diagnostics;
using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Api;
using artiso.AdsdHotel.Infrastructure.DataStorage;
using artiso.AdsdHotel.Infrastructure.MongoDataStorage;
using artiso.AdsdHotel.Infrastructure.NServiceBus;
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
        EndpointConfiguration endpointConfiguration = new ("Black.Api");
        var rabbitUri = ctx.Configuration.GetServiceUri("rabbit", "rabbit");
        var connectionString = CreateRabbitMqConnectionString(rabbitUri);
        endpointConfiguration
            .ConfigureDefaults(connectionString)
            .WithServerCallbacks();
        //endpointConfiguration.DefineCriticalErrorAction(OnCriticalError);


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

static async Task OnCriticalError(ICriticalErrorContext context)
{
    var fatalMessage = $"The following critical error was " +
                       $"encountered: {Environment.NewLine}{context.Error}{Environment.NewLine}Process is shutting down. " +
                       $"StackTrace: {Environment.NewLine}{context.Exception.StackTrace}";

    EventLog.WriteEntry(".NET Runtime", fatalMessage, EventLogEntryType.Error);

    try
    {
        await context.Stop().ConfigureAwait(false);
    }
    finally
    {
        Environment.FailFast(fatalMessage, context.Exception);
    }
}
