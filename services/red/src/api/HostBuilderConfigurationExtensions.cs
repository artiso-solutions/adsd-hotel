using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;
using artiso.AdsdHotel.Red.Api.Configuration;
using artiso.AdsdHotel.Red.Persistence;
using artiso.AdsdHotel.Red.Persistence.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using NServiceBus;

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
                services.Configure<RabbitMqConfig>(
                    ctx.Configuration.GetSection(key: nameof(RabbitMqConfig))
                );
                services.Configure<MongoDbConfig>(
                    ctx.Configuration.GetSection(key: nameof(MongoDbConfig))
                );
            }
        }


        private static void ConfigureGrpc(this IHostBuilder builder)
        {
            builder.ConfigureServices(Configure);
            builder.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<GrpcStartup>(); });

            static void Configure(HostBuilderContext ctx, IServiceCollection services)
            {
            }
        }

        private static void ConfigureServiceBus(this IHostBuilder builder)
        {
            builder.UseNServiceBus(ctx =>
            {
                var busConfiguration = new RabbitMqConfig();
                ctx.Configuration.Bind(nameof(RabbitMqConfig), busConfiguration);
                
                var endpointConfiguration = NServiceBusEndpointConfigurationFactory.Create(
                    endpointName: "Red.Api",
                    rabbitMqConnectionString: busConfiguration.ToString(),
                    false);

                return endpointConfiguration;
            });
        }

        private static void ConfigureStorage(this IHostBuilder builder)
        {
            builder.ConfigureServices(Configure);

            // Internal functions

            static void Configure(HostBuilderContext ctx, IServiceCollection services)
            {
                var mongoConfig = new MongoDbConfig();
                ctx.Configuration.Bind(nameof(MongoDbConfig), mongoConfig);
                services.AddSingleton(_ =>
                {
                    return new MongoDbClientFactory(mongoConfig);
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

            // Internal functions

            static void Configure(HostBuilderContext ctx, IServiceCollection services)
            {
                services.TryAddSingleton<IRoomRepository, RoomRepository>();
            }
        }
    }
}
