using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.ITOps.Sql;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MySql.Data.MySqlClient;

namespace artiso.AdsdHotel.Blue.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<RabbitMqReadinessProbe>();

            services.Configure<RabbitMqConfig>(Configuration.GetSection(key: nameof(RabbitMqConfig)));
            services.Configure<SqlConfig>(Configuration.GetSection(key: nameof(SqlConfig)));

            services.AddTransient<MySqlDatabaseInitializer>();

            services.AddTransient<IDbConnectionFactory, MySqlConnectionFactory>();

            var sqlConfig = Configuration.GetSection(key: nameof(SqlConfig)).Get<SqlConfig>();

            services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddMySql4()
                    .WithGlobalConnectionString(sqlConfig.ToMySqlConnectionString())
                    .ScanIn(typeof(Startup).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole());

            services.AddControllers();
            services.AddRazorPages();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Blue.Service", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/v1/swagger.json", "Blue.Service v1");
                c.RoutePrefix = "docs";
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
        public static async Task SetupDatabaseAsync(IServiceProvider services)
        {
            var dbInitializer = services.GetRequiredService<MySqlDatabaseInitializer>();

            var retries = 3;

            while (--retries > 0)
            {
                try
                {
                    await dbInitializer.EnsureCreatedAsync();
                }
                catch (MySqlException) when (retries != 0)
                {
                    await Task.Delay(1500);
                }
            }

            var runner = services.GetRequiredService<IMigrationRunner>();

            if (runner.HasMigrationsToApplyUp())
            {
                runner.ListMigrations();
                runner.MigrateUp();
            }
        }
    }
}
