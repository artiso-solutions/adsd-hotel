using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Contracts.Commands
{
    public record OrderRateSelectedRequest(string OrderId, Price Price);
}