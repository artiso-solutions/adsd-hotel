using System;

namespace artiso.AdsdHotel.Blue.Contracts
{
    public record Reservation(
        string Id,
        string OrderId,
        string RoomTypeId,
        DateTime Start,
        DateTime End,
        DateTime CreatedAt)
    {
        public string? RoomId { get; init; }
    }
}
