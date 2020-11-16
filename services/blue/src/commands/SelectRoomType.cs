using System;

namespace artiso.AdsdHotel.Blue.Commands
{
    public record SelectRoomType(
        string OrderId,
        DateTime Start,
        DateTime End,
        string RoomTypeId);
}
