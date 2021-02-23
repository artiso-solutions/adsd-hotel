using System;

namespace artiso.AdsdHotel.Blue.Events
{
    public record RoomTypeSelected(
        string OrderId,
        string RoomTypeId,
        DateTime Start,
        DateTime End,
        DateTime SelectedAt);
}
