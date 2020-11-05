using System;
using artiso.AdsdHotel.Black.Contracts;

namespace artiso.AdsdHotel.Black.Commands
{
    public class SetGuestInformation
    {
        public SetGuestInformation(Guid orderId, GuestInformation guestInformation)
        {
            OrderId = orderId;
            GuestInformation = guestInformation;
        }

        public Guid OrderId { get; }

        public GuestInformation GuestInformation { get; }
    }
}
