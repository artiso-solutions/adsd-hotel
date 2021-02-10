using System;

namespace artiso.AdsdHotel.Blue.Contracts
{
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
