using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Contracts.Commands
{
    public record ChargeCancellationFeeRequest(string OrderId, PaymentMethod? PaymentMethod);
}