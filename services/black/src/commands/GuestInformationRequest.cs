using System;

namespace artiso.AdsdHotel.Black.Commands
{
    public class GuestInformationRequest
    {
        public GuestInformationRequest(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; }
    }
}
