using System;
using artiso.AdsdHotel.Black.Contracts;

namespace artiso.AdsdHotel.Black.Events
{
    public record GuestInformationSet(Guid OrderId, GuestInformation GuestInformation);
}
