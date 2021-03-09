using System;
using artiso.AdsdHotel.Yellow.Api.Configuration;
using artiso.AdsdHotel.Yellow.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace artiso.AdsdHotel.Yellow.Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton(sp =>
            {
                var config = AppSettingsHelper.GetSettings<MongoDbConfig>();
                return new MongoDBClientFactory(config);
            });
            services.TryAddSingleton<IOrderService, OrderService>();
            services.TryAddSingleton<ICreditCardPaymentService, CreditCardPaymentService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //
        }
    }
}
