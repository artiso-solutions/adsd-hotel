using System;
using artiso.AdsdHotel.Black.Contracts;
using NServiceBus;

namespace artiso.AdsdHotel.Black.Commands
{
    public class SetGuestInformation : ICommand
    {
        public Guid OrderId { get; set; }
        public GuestInformation GuestInformation { get; set; }
    }
}
