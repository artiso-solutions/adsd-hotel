using artiso.AdsdHotel.Red.Api;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;

namespace artiso.AdsdHotel.Red.Ambassador
{
    class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter")]
        // ReSharper disable once UnusedParameter.Local
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
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