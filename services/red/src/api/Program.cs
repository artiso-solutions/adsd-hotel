using Microsoft.Extensions.Hosting;

namespace artiso.AdsdHotel.Red.Api
{
    public class Program
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
