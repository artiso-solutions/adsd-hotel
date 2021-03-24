using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using artiso.AdsdHotel.Red.Contracts;
using Grpc.Net.Client;
using Date = artiso.AdsdHotel.Red.Contracts.Date;

namespace artiso.AdsdHotel.Red.Ambassador
{
    public class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter")]
        // ReSharper disable once UnusedParameter.Local
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Rates.RatesClient(channel);
            List<string> cases = new List<string>(){"BedNBreakfast", "Honeymoon"};
            foreach (string s in cases)
            {
                var reply = await client.GetRoomRatesByRoomTypeAsync(new GetRoomRatesByRoomTypeRequest
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
                var inputRoomRatesResponse = await client.InputRoomRatesAsync(inputRoomRatesRequest);
                if (inputRoomRatesResponse.Success)
                {
                    Console.WriteLine($"Order for {s} successfully written");
                }
            }

            Console.ReadKey();
        }
    }
}
