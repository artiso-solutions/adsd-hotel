using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using artiso.AdsdHotel.Red.Api;
using Grpc.Net.Client;
using MongoDB.Driver;

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
            List<string> cases = new List<string>(){"Bed & Breakfast", "Honeymoon"};
            foreach(string s in cases)
            {
                var reply = await client.GetRoomRatesByRoomTypeAsync(new GetRoomRatesByRoomTypeRequest
                {
                    RoomType = s
                });

                Console.WriteLine($"Test response for {s}:");
                foreach (var replyRoomRate in reply.RoomRates)
                {
                    Console.WriteLine($"{replyRoomRate.Name} - {replyRoomRate.Price} €");
                }
                Console.WriteLine();
            }
            Console.ReadKey();
        }
    }
}