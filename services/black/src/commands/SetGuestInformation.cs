using System;
using artiso.AdsdHotel.Black.Contracts;

namespace artiso.AdsdHotel.Black.Commands
{
    public class SetGuestInformation
    {
        public Guid OrderId { get; set; }

        public GuestInformation GuestInformation { get; set; }
    }
}
