using System;
using System.Collections.Generic;

namespace artiso.AdsdHotel.Red.Persistence.Entities
{
    public class RoomRateEntity
    {
        public string OrderId { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public IEnumerable<RateItemEntity> RateItems { get; private set; }

        public RoomRateEntity(string orderId, DateTime startDate, DateTime endDate, IEnumerable<RateItemEntity> rateItems)
        {
            OrderId = orderId;
            StartDate = startDate;
            EndDate = endDate;
            RateItems = rateItems;
        }
    }
}
