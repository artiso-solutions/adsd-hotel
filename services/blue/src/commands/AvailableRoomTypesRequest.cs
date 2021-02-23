using System;
using System.Collections.Generic;
using artiso.AdsdHotel.Blue.Contracts;

namespace artiso.AdsdHotel.Blue.Commands
{
    public record AvailableRoomTypesRequest(DateTime Start, DateTime End);

    public record AvailableRoomTypesResponse(IReadOnlyList<RoomType> RoomTypes);
}
