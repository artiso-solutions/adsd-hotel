using System;

namespace artiso.AdsdHotel.Blue.Commands
{
    public record SelectRoomType(
        string OrderId,
        string RoomTypeId,
        DateTime Start,
        DateTime End);
}
