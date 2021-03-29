using System;
using System.Threading.Tasks;
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
            var config = ctx.Configuration;

            var dbConfig = new DatabaseConfiguration(
                Host: config.GetValue("mysql:host", defaultValue: "localhost"),
                Port: config.GetValue("mysql:port", defaultValue: 3306),
                Database: "adsd-blue",
                Username: config.GetValue("mysql:db:user", defaultValue: "root"),
                Password: config.GetValue("mysql:db:password", defaultValue: default(string?)));

            var cs = dbConfig.ToMySqlConnectionString();

            services.AddLogging(builder => builder.AddConsole());

            services.AddSingleton(dbConfig);

            services.AddTransient<MySqlDatabaseInitializer>();

            services.AddTransient<IDbConnectionFactory, MySqlConnectionFactory>();

            services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddMySql4()
                    .WithGlobalConnectionString(cs)
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
