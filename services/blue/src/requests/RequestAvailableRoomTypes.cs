using System;
using System.Collections.Generic;
using artiso.AdsdHotel.Blue.Contracts;

namespace artiso.AdsdHotel.Blue.Commands
{
    public class RequestAvailableRoomTypes
    {
        public DateTime Start { get; set; }

        public DateTime End { get; set; }
    }

    public class AvailableRoomTypesResponse
    {
        public IReadOnlyCollection<RoomType> RoomTypes { get; set; } = Array.Empty<RoomType>();
    }
}
