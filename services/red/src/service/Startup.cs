using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace artiso.AdsdHotel.Red.Api
{
    internal class Startup
    {
        public static void ConfigureServices(
            HostBuilderContext _,
            IServiceCollection services)
        {
            services.AddLogging(builder => builder.AddConsole());
        }
    }
}
