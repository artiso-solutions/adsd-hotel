using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Sql;
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
                Host: "localhost",
                Port: 3306,
                Database: "adsd-blue",
                Username: "root",
                Password: null);

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
