using System;
using artiso.AdsdHotel.Black.Contracts;
using NServiceBus;

namespace artiso.AdsdHotel.Black.Messages
{
    public class GuestInformationSet : IEvent
    {
        public Guid OrderId { get; set; }
        public GuestInformation GuestInformation { get; set; }
    }
}
