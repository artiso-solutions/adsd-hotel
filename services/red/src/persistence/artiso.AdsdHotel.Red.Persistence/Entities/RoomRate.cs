using System;
using System.Collections.Generic;

namespace artiso.AdsdHotel.Red.Persistence.Entities
{
    public class RoomRate
    {
        public string OrderId { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public IEnumerable<RateItem> RateItems { get; private set; }

        public RoomRate(string orderId, DateTime startDate, DateTime endDate, IEnumerable<RateItem> rateItems)
        {
            OrderId = orderId;
            StartDate = startDate;
            EndDate = endDate;
            RateItems = rateItems;
        }
    }
}
