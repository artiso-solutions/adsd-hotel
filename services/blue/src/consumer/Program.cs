using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Ambassador;
using Microsoft.Extensions.Configuration;

namespace artiso.AdsdHotel.Blue.Consumer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await Task.Delay(5000);

            var config = GetConfiguration(args);

            var baseAddress = config.GetSection(key: "ApiConfig").GetValue<string>("BaseAddress");
            var ambassador = BlueAmbassadorFactory.Create(baseAddress);

            var start = DateTime.Today.AddDays(7);
            var end = start.AddDays(7);

            var availableRoomTypes = await ambassador.ListRoomTypesAvailableBetweenAsync(start, end);

            var orderId = Guid.NewGuid().ToString();
            var desiredRoomType = availableRoomTypes.PickRandom();

            await ambassador.SelectRoomTypeBetweenAsync(
                orderId,
                desiredRoomType.Id,
                start,
                end);

            var orderSummary = await ambassador.GetOrderSummaryAsync(orderId);

            Console.WriteLine(orderSummary);
        }

        private static IConfiguration GetConfiguration(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            return configuration;
        }
    }
}
