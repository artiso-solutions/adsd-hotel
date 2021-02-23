using System;
using artiso.AdsdHotel.Red.Api;

namespace artiso.AdsdHotel.Red.Events
{
    public record OrderRateSelected(Guid OrderId, Rate GuestInformation);
}
