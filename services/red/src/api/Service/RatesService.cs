using Grpc.Core;
using System.Threading.Tasks;

namespace artiso.AdsdHotel.Red.Api.Service
{
    public class RatesService : Rates.RatesBase
    {
        public override Task<GetRoomRatesByRoomTypeReply> GetRoomRatesByRoomType(GetRoomRatesByRoomTypeRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GetRoomRatesByRoomTypeReply
            {
                Message = request.Name
            });
        }

        public override Task<InputRoomRatesReply> InputRoomRates(InputRoomRatesRequest request, ServerCallContext context)
        {
            return Task.FromResult(new InputRoomRatesReply
            {
                Message = request.Name
            });
        }
    }
}