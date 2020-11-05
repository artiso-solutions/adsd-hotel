using System;
using artiso.AdsdHotel.Black.Contracts;

namespace artiso.AdsdHotel.Black.Events
{
    public class GuestInformationSet
    {
        public Guid OrderId { get; set; }

        public GuestInformation GuestInformation { get; set; }

        public GuestInformationSet(Guid guid, GuestInformation guestInformation)
        {
            OrderId = guid;
            GuestInformation = guestInformation;
        }
    }
}
