using artiso.AdsdHotel.Red.Contracts;

namespace artiso.AdsdHotel.Red.Events
{
    public record OrderRateSelected(string OrderId, Rate GuestInformation);
}
