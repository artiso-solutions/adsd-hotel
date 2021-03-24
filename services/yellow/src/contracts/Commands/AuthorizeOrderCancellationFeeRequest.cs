using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Contracts.Commands
{
    /// <summary>
    /// Using the given <see cref="PaymentMethod"/> verifies if it can pay the amount of money
    /// defined as CancellationFee from the Order
    /// </summary>
    public record AuthorizeOrderCancellationFeeRequest(string OrderId);
}
