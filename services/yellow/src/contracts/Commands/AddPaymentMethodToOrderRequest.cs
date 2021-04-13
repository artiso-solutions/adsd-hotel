using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Contracts.Commands
{
    /// <summary>
    /// Adds the given <see cref="PaymentMethod"/> to the order
    /// </summary>
    public record AddPaymentMethodToOrderRequest(
        string OrderId,
        PaymentMethod PaymentMethod);

    public record PaymentMethodToOrderAdded(string OrderId);
    
    public record AddPaymentMethodToOrderFailed(string OrderId);
}
