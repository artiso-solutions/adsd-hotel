using System;

namespace artiso.AdsdHotel.Blue.Contracts
{
    public record PendingReservation(
        string Id,
        string OrderId,
        string RoomTypeId,
        DateTime Start,
        DateTime End,
        DateTime CreatedAt)
    {
        public bool Confirmed { get; init; }
    }
}
