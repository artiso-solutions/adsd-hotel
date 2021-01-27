using System;
using System.Threading.Tasks;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace artiso.AdsdHotel.Blue.Api
{
    internal class Startup
    {
        public static void ConfigureServices(
            HostBuilderContext _,
            IServiceCollection services)
        {
            var dbConfig = new DatabaseConfiguration(
                host: "localhost",
                port: 3306,
                database: "adsd-blue",
                username: "root",
                password: null);

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
                    .ScanIn(typeof(MySqlConnectionFactory).Assembly).For.Migrations())
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
