using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System.Diagnostics;
using MongoDB.Driver;
//using artiso.AdsdHotel.Infrastructure.DataStorage;
//using artiso.AdsdHotel.Infrastructure.MongoDataStorage;

namespace artiso.AdsdHotel.Black.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);
            builder.UseConsoleLifetime();
            builder.ConfigureServices((ctx, services) =>
            {
                var rabbitUri = ctx.Configuration.GetServiceUri("rabbit", "rabbit");
                if (rabbitUri != null)
                {
                    // this blocks further initialization until the rabbitmq instance is running
                    services.AddSingleton<IHostedService>(new ProceedIfRabbitMqIsAlive(rabbitUri.Host, rabbitUri.Port));
                }


                //var mongoUri = ctx.Configuration.GetServiceUri("mongodb", "mongodb");
                //string mongoConnectionString = $"{mongoUri.Scheme}://{mongoUri.Host}:{mongoUri.Port}";
                //var dbName = ctx.Configuration.GetValue<string>("BLACK_API_DBNAME");
                //var collectionName = ctx.Configuration.GetValue<string>("BLACK_API_COLLECTIONNAME");
            });

            builder.UseNServiceBus(ctx =>
            {
                var endpointConfiguration = new EndpointConfiguration("Black.Api");
                // endpointConfiguration.EnableCallbacks(makesRequests: false);
                endpointConfiguration.EnableInstallers();

                // ToDo use durable persistence in production
                // InMemoryPersistence may loose messages if the transport does not support it natively
                endpointConfiguration.UsePersistence<InMemoryPersistence>();
                endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
                endpointConfiguration.DefineCriticalErrorAction(OnCriticalError);

                var rabbitUri = ctx.Configuration.GetServiceUri("rabbit", "rabbit");
                if (rabbitUri != null)
                {
                    var connectionString = CreateRabbitMqConnectionString(rabbitUri);
                    var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
                    transport.ConnectionString(connectionString);
                    transport.UseConventionalRoutingTopology();
                }
                else
                {
                    endpointConfiguration.UseTransport<LearningTransport>();
                }

                var conventions = endpointConfiguration.Conventions();

                conventions.DefiningCommandsAs(t =>
                    t.Namespace != null &&
                    t.Namespace.EndsWith(".Commands") &&
                    !t.Name.EndsWith("Response"));

                conventions.DefiningMessagesAs(t =>
                    t.Namespace != null &&
                    t.Namespace.EndsWith(".Commands") &&
                    t.Name.EndsWith("Response"));

                conventions.DefiningEventsAs(t =>
                    t.Namespace != null &&
                    t.Namespace.EndsWith(".Events"));

                return endpointConfiguration;
            });

            return builder;
        }

        private static string CreateRabbitMqConnectionString(Uri uri)
        {
            return $"host={uri.Host}";
        }

        private static async Task OnCriticalError(ICriticalErrorContext context)
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
    }
}
