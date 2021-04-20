using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using artiso.AdsdHotel.Red.Ambassador;

namespace artiso.AdsdHotel.Red.Consumer
{
    public class RedConsumer
    {
        public static async Task Main()
        {
            var ambassador = RedAmbassadorFactory.Create();

            List<string> cases = new() { "RMTY-001", "RMTY-002" };

            foreach (var roomType in cases)
            {
                var roomRates = await ambassador.GetRoomRatesByRoomTypeAsync(roomType, DateTime.Today, DateTime.Today.AddDays(7));

                Console.WriteLine($"Test response for {roomType}:");

                foreach (var roomRate in roomRates)
                {
                    foreach (var replyRoomRate in roomRate.RateItems)
                    {
                        Console.WriteLine($"{replyRoomRate.Id} - {replyRoomRate.Price} Euro");
                    }

                    Console.WriteLine($"Total price - {roomRate.TotalPrice} Euro");
                    Console.WriteLine();

                    var inputRoomRatesResponse = await ambassador.InputRoomRatesAsync(roomRate.Id, Guid.NewGuid().ToString(),
                        DateTime.Now, DateTime.Now + new TimeSpan(7));
                    if (inputRoomRatesResponse.Success)
                    {
                        Console.WriteLine($"Order for {roomType} successfully written");
                    }
                }
            }

            Console.ReadKey();
        }
    }
}
