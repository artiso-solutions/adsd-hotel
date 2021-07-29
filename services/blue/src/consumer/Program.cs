using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Ambassador;
using artiso.AdsdHotel.ITOps.Communication;
using Microsoft.Extensions.Configuration;

namespace artiso.AdsdHotel.Blue.Consumer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await Task.Delay(5000);

            var config = GetConfiguration(args);

            var rabbitMqConfig = config.GetSection(key: nameof(RabbitMqConfig)).Get<RabbitMqConfig>();
            var ambassador = BlueAmbassadorFactory.Create(rabbitMqConfig.AsConnectionString());

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

            await ambassador.ConfirmSelectedRoomTypeAsync(orderId);

            var roomNumber = await ambassador.GetReservationRoomNumberAsync(orderId);

            if (roomNumber is null)
                throw new Exception($"Unable to get room number for order '{orderId}'");

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
