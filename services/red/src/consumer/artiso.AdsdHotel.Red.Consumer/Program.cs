using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using artiso.AdsdHotel.Red.Ambassador;
using artiso.AdsdHotel.Red.Contracts;

namespace artiso.AdsdHotel.Red.Consumer
{
    public class RedConsumer
    {
        public static async Task Main()
        {
            var ambassador = RedAmbassadorFactory.Create();

            List<string> cases = new() { "BedNBreakfast", "Honeymoon" };
            foreach (string s in cases)
            {
                var reply = await ambassador.GetRoomRatesByRoomTypeAsync(new GetRoomRatesByRoomTypeRequest
                {
                    RoomType = s
                });

                Console.WriteLine($"Test response for {s}:");
                foreach (var replyRoomRate in reply.RateItems)
                {
                    Console.WriteLine($"{replyRoomRate.Id} - {replyRoomRate.Price} Euro");
                }

                Console.WriteLine($"Total price - {reply.TotalPrice} Euro");
                Console.WriteLine();
                var inputRoomRatesRequest = new InputRoomRatesRequest()
                {
                    OrderId = Guid.NewGuid().ToString(),
                    StartDate = new Date(DateTime.Now),
                    EndDate = new Date(DateTime.Now + new TimeSpan(7))
                };
                inputRoomRatesRequest.RateItems.AddRange(reply.RateItems);
                var inputRoomRatesResponse = await ambassador.InputRoomRatesAsync(inputRoomRatesRequest);
                if (inputRoomRatesResponse.Success)
                {
                    Console.WriteLine($"Order for {s} successfully written");
                }
            }

            Console.ReadKey();
        }
    }
}
