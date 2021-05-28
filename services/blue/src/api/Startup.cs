using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.ITOps.Sql;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace artiso.AdsdHotel.Blue.Api
{
    internal class Startup
    {
        public static void ConfigureServices(
            HostBuilderContext ctx,
            IServiceCollection services)
        {
            services.AddSingleton<RabbitMqReadinessProbe>();

            var config = ctx.Configuration;

            services.Configure<RabbitMqConfig>(config.GetSection(key: nameof(RabbitMqConfig)));
            services.Configure<SqlConfig>(config.GetSection(key: nameof(SqlConfig)));

            services.AddLogging(builder => builder.AddConsole());

            services.AddTransient<MySqlDatabaseInitializer>();

            services.AddTransient<IDbConnectionFactory, MySqlConnectionFactory>();

            var sqlConfig = config.GetSection(key: nameof(SqlConfig)).Get<SqlConfig>();

            services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddMySql4()
                    .WithGlobalConnectionString(sqlConfig.ToMySqlConnectionString())
                    .ScanIn(typeof(Startup).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole());
        }

        public static async Task SetupDatabaseAsync(IServiceProvider services)
        {
            var dbInitializer = services.GetRequiredService<MySqlDatabaseInitializer>();
            await dbInitializer.EnsureCreatedAsync();

            var runner = services.GetRequiredService<IMigrationRunner>();

            if (runner.HasMigrationsToApplyUp())
            {
                runner.ListMigrations();
                runner.MigrateUp();
            }
        }
    }
}
