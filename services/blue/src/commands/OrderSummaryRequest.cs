using System;
using artiso.AdsdHotel.Blue.Contracts;

namespace artiso.AdsdHotel.Blue.Commands
{
    public record OrderSummaryRequest(string OrderId);

    public record OrderSummaryResponse(
        OrderSummary? OrderSummary)
    {
        public bool Found => OrderSummary is not null;
    }

    public record OrderSummary(
        string OrderId,
        DateTime Start,
        DateTime End,
        RoomType? RoomType,
        bool Confirmed)
    {
        public DateTime? RequestedAt { get; init; }

        public DateTime? ConfirmedAt { get; init; }
    }
}
