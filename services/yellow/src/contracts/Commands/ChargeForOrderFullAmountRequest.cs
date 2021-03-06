using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Contracts.Commands
{
    /// <summary>
    /// Proceeds to charge the Full amount defined in the matching <see cref="Order"/>
    /// </summary>
    public record ChargeForOrderFullAmountRequest(string OrderId)
    {
        public PaymentMethod? AlternativePaymentMethod { get; set; }
    };
}
