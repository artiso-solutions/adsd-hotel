using artiso.AdsdHotel.ITOps.Communication.Abstraction;
using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;
using artiso.AdsdHotel.Red.Api.Configuration;
using artiso.AdsdHotel.Red.Api.Handlers;
using artiso.AdsdHotel.Red.Persistence;
using artiso.AdsdHotel.Red.Persistence.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;

namespace artiso.AdsdHotel.Red.Api
{
    internal static class HostBuilderConfigurationExtensions
    {
        public static IHostBuilder ConfigureApp(this IHostBuilder builder)
        {
            builder.UseConsoleLifetime();
            ConfigureOptions(builder);

            ConfigureStorage(builder);
            ConfigureServiceBus(builder);
            ConfigureGrpc(builder);
            ConfigureCustomServices(builder);

            return builder;
        }

        private static void ConfigureOptions(this IHostBuilder builder)
        {
            builder.ConfigureServices(Configure);
            static void Configure(HostBuilderContext ctx, IServiceCollection services)
            {
                services
                    .Configure<RabbitMqConfig>(ctx.Configuration.GetSection(key: nameof(RabbitMqConfig)))
                    .Configure<MongoDbConfig>(ctx.Configuration.GetSection(key: nameof(MongoDbConfig)));
            }
        }


        private static void ConfigureGrpc(this IHostBuilder builder)
        {
            builder.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<GrpcStartup>(); });
        }

        private static void ConfigureServiceBus(this IHostBuilder builder)
        {
            builder.ConfigureServices(Configure);

            static void Configure(HostBuilderContext ctx, IServiceCollection services)
            {
                services.AddScoped<IChannel, NServiceBusChannel>(sp =>
                {
                    var config = sp.GetRequiredService<IOptions<RabbitMqConfig>>();
                    return NServiceBusChannelFactory.Create("Red.Api", "error", config.Value.ToString());

                });
            }
        }

        private static void ConfigureStorage(this IHostBuilder builder)
        {
            builder.ConfigureServices(Configure);

            static void Configure(HostBuilderContext ctx, IServiceCollection services)
            {
                services.AddSingleton(sp =>
                {
                    var config = sp.GetRequiredService<IOptions<MongoDbConfig>>();
                    return new MongoDbClientFactory(config.Value);
                });

                var conventions = new ConventionPack
                {
                    new IgnoreExtraElementsConvention(true),
                    new CamelCaseElementNameConvention(),
                    new EnumRepresentationConvention(BsonType.String),
                };

                ConventionRegistry.Register("DefaultConventions", conventions, filter: _ => true);
            }
        }

        private static void ConfigureCustomServices(this IHostBuilder builder)
        {
            builder.ConfigureServices(Configure);

            static void Configure(HostBuilderContext ctx, IServiceCollection services)
            {
                services.TryAddSingleton<IRoomRepository, RoomRepository>();
                services.AddScoped<GetRoomRatesByRoomTypeHandler>();
                services.AddScoped<RoomSelectedHandler>();
            }
        }
    }
}
