using System;

namespace artiso.AdsdHotel.Red.Persistence.Entities
{
    public class RoomRateEntity
    {
        public string OrderId { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public string RoomRateId { get; private set; }

        public RoomRateEntity(string orderId, DateTime startDate, DateTime endDate, string roomRateId)
        {
            OrderId = orderId;
            StartDate = startDate;
            EndDate = endDate;
            RoomRateId = roomRateId;
        }
    }
}
