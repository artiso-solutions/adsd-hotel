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

namespace artiso.AdsdHotel.Black.Api
{
    public class Program
    {
        public static async Task  Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder= Host.CreateDefaultBuilder(args);
            builder.UseConsoleLifetime();

            builder.UseNServiceBus(ctx =>
            {
                var endpointConfiguration = new EndpointConfiguration("Black.Api");
                var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
                transport.ConnectionString("host=localhost");
                transport.UseConventionalRoutingTopology();
                // ToDo use durable persistence in production
                // InMemoryPersistence may loose messages if the transport does not support it natively
                endpointConfiguration.UsePersistence<InMemoryPersistence>();

                endpointConfiguration.DefineCriticalErrorAction(OnCriticalError);

                return endpointConfiguration;
            });

            return builder;
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
