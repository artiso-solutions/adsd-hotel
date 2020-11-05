using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using artiso.AdsdHotel.Red.Api;
using Grpc.Net.Client;

namespace artiso.AdsdHotel.Red.Ambassador
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (var channel = GrpcChannel.ForAddress("https://localhost:5001"))
            {
                var client = new Rates.RatesClient(channel);
                var reply = await client.GetRoomRatesByRoomTypeAsync(
                    new GetRoomRatesByRoomTypeRequest
                    {
                        Name = "Rates1"
                    });
                Console.WriteLine("Test response" + reply.Message);
                Console.ReadKey();
            }
        }
    }
}