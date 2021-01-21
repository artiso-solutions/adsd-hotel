using System;
using artiso.AdsdHotel.Black.Contracts;

namespace artiso.AdsdHotel.Black.Commands
{
    public record SetGuestInformation(Guid OrderId, GuestInformation GuestInformation);
}
