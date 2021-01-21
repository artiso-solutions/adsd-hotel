using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.Red.Api;
using Grpc.Net.Client;

namespace artiso.AdsdHotel.Red.Ambassador
{
    public class Program
    {
        public static async Task Main()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Rates.RatesClient(channel);
            var reply = await client.GetRoomRatesByRoomTypeAsync(
                new GetRoomRatesByRoomTypeRequest
                {
                    Name = "Rates1"
                });
            Console.WriteLine($"Test response: {reply.Message}");
            Console.ReadKey();
        }
    }
}