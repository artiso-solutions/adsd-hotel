using System;
using Microsoft.Extensions.Configuration;

namespace artiso.AdsdHotel.Yellow.Api
{
    internal static class AppSettingsHelper{
        
        internal static T GetSettings<T>()
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
#if DEBUG
                .AddJsonFile("appsettings.Development.json", true, true)
                .AddJsonFile("appsettings.Local.json", true, true)
#endif
                .Build();
            
            var section = config.GetSection(typeof(T).Name);
            
            return section.Get<T>();
        }
    }
}