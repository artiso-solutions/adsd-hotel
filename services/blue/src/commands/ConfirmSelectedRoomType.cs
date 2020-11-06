using System;

namespace artiso.AdsdHotel.Blue.Commands
{
    public class ConfirmSelectedRoomType
    {
        public ConfirmSelectedRoomType(string orderId)
        {
            OrderId = orderId;
        }

        public string OrderId { get; }
    }
}
