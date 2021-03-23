using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Communication.Abstraction;
using artiso.AdsdHotel.Purple.Commands;

namespace artiso.AdsdHotel.Purple.Ambassador
{
    public class PurpleAmbassador
    {
        private readonly IChannel _channel;

        internal PurpleAmbassador(IChannel channel)
        {
            _channel = channel;
        }

        public async Task CompleteReservationAsync(string orderId)
        {
            await _channel.Send(new CompleteReservation(orderId));
        }
    }
}
