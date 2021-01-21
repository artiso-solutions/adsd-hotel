using System;

namespace artiso.AdsdHotel.Blue.Events
{
    public record RoomTypeNotAvailable(
        string OrderId,
        DateTime Start,
        DateTime End,
        string RoomTypeId);
}
