using System;

namespace artiso.AdsdHotel.Blue.Api.Models
{
    public record SelectRoomType(
        string RoomTypeId,
        DateTime Start,
        DateTime End);
}
