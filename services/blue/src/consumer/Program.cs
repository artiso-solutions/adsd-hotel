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

            var rabbitMqConnectionString = config.GetValue("rabbitmq:cs", defaultValue: "host=localhost");
            var ambassador = BlueAmbassadorFactory.Create(rabbitMqConnectionString);

            var start = DateTime.Today.AddDays(7);
            var end = start.AddDays(7);

            var availableRoomTypes = await ambassador.ListRoomTypesAvailableBetweenAsync(start, end);

            var orderId = Guid.NewGuid().ToString();
            var desiredRoomType = availableRoomTypes.PickRandom();

            var roomTypeWasSelected = await ambassador.SelectRoomTypeBetweenAsync(
                orderId,
                desiredRoomType.Id,
                start,
                end);

            if (!roomTypeWasSelected)
                throw new Exception($"Unable to select room type '{desiredRoomType.Id}'");

            var roomTypeWasConfirmed = await ambassador.ConfirmSelectedRoomTypeAsync(orderId);

            if (!roomTypeWasConfirmed)
                throw new Exception($"Unable to confirm room type '{desiredRoomType.Id}'");

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
