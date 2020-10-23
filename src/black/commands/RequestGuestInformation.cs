using System;
using NServiceBus;

namespace artiso.AdsdHotel.Black.Commands
{
    public class RequestGuestInformation : ICommand
    {
        public Guid OrderId { get; set; }
    }
}
