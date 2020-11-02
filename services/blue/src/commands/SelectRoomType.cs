using System;

namespace artiso.AdsdHotel.Blue.Commands
{
    public class SelectRoomType
    {
        public SelectRoomType(string orderId, DateTime start, DateTime end, string roomTypeId)
        {
            OrderId = orderId;
            Start = start;
            End = end;
            RoomTypeId = roomTypeId;
        }

        public string OrderId { get; }

        public DateTime Start { get; }

        public DateTime End { get; }

        public string RoomTypeId { get; }
    }
}
