using System;
using System.Threading.Tasks;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace artiso.AdsdHotel.Blue.Api
{
    public class Program
    {
        public static async Task Main()
        {
            var dbConfig = new DatabaseConfiguration(
                host: "localhost",
                port: 3306,
                database: "adsd-blue",
                username: "root",
                password: null);

            var services = ConfigureServices(dbConfig);

            using var scope = services.CreateScope();

            await SetupDatabaseAsync(scope.ServiceProvider);

            Console.ReadLine();
        }

        private static IServiceProvider ConfigureServices(DatabaseConfiguration dbConfig)
        {
            var cs = dbConfig.ToMySqlConnectionString();

            var services = new ServiceCollection();

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

            return services.BuildServiceProvider();
        }

        private static async Task SetupDatabaseAsync(IServiceProvider services)
        {
            var dbInitializer = services.GetRequiredService<MySqlDatabaseInitializer>();
            await dbInitializer.EnsureCreatedAsync();

            var runner = services.GetRequiredService<IMigrationRunner>();
            runner.ListMigrations();
            runner.MigrateUp();
        }
    }
}
