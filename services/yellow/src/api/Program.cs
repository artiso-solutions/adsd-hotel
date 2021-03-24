using Microsoft.Extensions.Hosting;

namespace artiso.AdsdHotel.Yellow.Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureApp()
                .Build()
                .Run();
        }
    }
}
