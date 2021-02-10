using artiso.AdsdHotel.Blue.Contracts;

namespace artiso.AdsdHotel.Blue.Commands
{
    public record OrderSummaryRequest(string OrderId);

    public record OrderSummaryResponse(OrderSummary? OrderSummary)
    {
        public bool Found => OrderSummary is not null;
    }
}
