using System.Threading.Tasks;
using artiso.AdsdHotel.Red.Contracts;
using Grpc.Net.Client;

namespace artiso.AdsdHotel.Red.Ambassador
{
    public class RedAmbassador
    {
        private readonly Rates.RatesClient _ratesClient;

        internal RedAmbassador(string uri)
        {
            using var channel = GrpcChannel.ForAddress(uri);
            _ratesClient = new Rates.RatesClient(channel);
        }

        public async Task<GetRoomRatesByRoomTypeReply> GetRoomRatesByRoomTypeAsync(GetRoomRatesByRoomTypeRequest getRoomRatesByRoomTypeRequest)
        {
            return await _ratesClient.GetRoomRatesByRoomTypeAsync(getRoomRatesByRoomTypeRequest);
        }

        public async Task<InputRoomRatesReply> InputRoomRatesAsync(InputRoomRatesRequest inputRoomRatesRequest)
        {
            return await _ratesClient.InputRoomRatesAsync(inputRoomRatesRequest);
        }
    }
}
