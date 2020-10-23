using System;
using artiso.AdsdHotel.Black.Contracts;

namespace artiso.AdsdHotel.Black.Events
{
    public class GuestInformationSet
    {
        public Guid OrderId { get; }

        public GuestInformation GuestInformation { get; }

        public GuestInformationSet(
            Guid guid,
            GuestInformation guestInformation)
        {
            OrderId = guid;
            GuestInformation = guestInformation;
        }
    }
}
