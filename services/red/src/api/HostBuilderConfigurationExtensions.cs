using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.ITOps.Communication.Abstraction;
using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;
using artiso.AdsdHotel.ITOps.NoSql;
using artiso.AdsdHotel.Red.Api.Handlers;
using artiso.AdsdHotel.Red.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace artiso.AdsdHotel.Red.Api
{
    internal static class HostBuilderConfigurationExtensions
    {
        public static IHostBuilder ConfigureApp(this IHostBuilder builder)
        {
            builder.UseConsoleLifetime();
            builder.ConfigureOptions();
            builder.ConfigureServiceBus();
            builder.ConfigureGrpc();
            builder.ConfigureCustomServices();

            return builder;
        }

        private static void ConfigureOptions(this IHostBuilder builder)
        {
            builder.ConfigureServices(Configure);

            static void Configure(HostBuilderContext ctx, IServiceCollection services)
            {
                services
                    .Configure<RabbitMqConfig>(ctx.Configuration.GetSection(key: nameof(RabbitMqConfig)))
                    .Configure<MongoDbConfig>(ctx.Configuration.GetSection(key: nameof(MongoDbConfig)))
                    .AddSingleton<IMongoDbClientFactory, MongoDbClientFactory>();
            }
        }


        private static void ConfigureGrpc(this IHostBuilder builder)
        {
            builder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<GrpcStartup>().ConfigureKestrel(options =>
                {
                    options.ListenAnyIP(5001 , listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http2;
                    });
                });
            });
        }

        private static void ConfigureServiceBus(this IHostBuilder builder)
        {
            builder.ConfigureServices(Configure);

            static void Configure(HostBuilderContext ctx, IServiceCollection services)
            {
                services.AddScoped<IChannel, NServiceBusChannel>(sp =>
                {
                    var config = sp.GetRequiredService<IOptions<RabbitMqConfig>>();
                    // This channel will only be used to publish messages.
                    // Any attempt to Send a message will move it into the "red-error" queue.
                    return NServiceBusChannelFactory.Create("Red.Api", "red-error", config.Value.ToString());
                });
            }
        }

        private static void ConfigureCustomServices(this IHostBuilder builder)
        {
            builder.ConfigureServices(Configure);

            static void Configure(HostBuilderContext ctx, IServiceCollection services)
            {
                services.AddSingleton<RabbitMqReadinessProbe>();
                services.AddScoped<IRoomRepository, RoomRepository>();
                services.AddScoped<GetRoomRatesByRoomTypeHandler>();
                services.AddScoped<RoomSelectedHandler>();
            }
        }
    }
}
