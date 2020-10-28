using System;
using artiso.AdsdHotel.Black.Contracts;

namespace artiso.AdsdHotel.Black.Commands
{
    public class RequestGuestInformation
    {
        public Guid OrderId { get; set; }
    }

    public class GuestInformationResponse
    {
        public GuestInformation GuestInformation { get; set; }
    }
}
