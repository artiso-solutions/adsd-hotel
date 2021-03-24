using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Contracts.Commands
{
    /// <summary>
    /// Proceeds to charge the CancellationFee amount defined in the matching <see cref="Order"/>
    /// </summary>
    public record ChargeForOrderCancellationFeeRequest(string OrderId)
    {
        public PaymentMethod? AlternativePaymentMethod { get; init; }
    }
}
