using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Contracts.Commands
{
    public record AuthorizeCancellationFeeRequest(string OrderId, PaymentMethod PaymentMethod);
}