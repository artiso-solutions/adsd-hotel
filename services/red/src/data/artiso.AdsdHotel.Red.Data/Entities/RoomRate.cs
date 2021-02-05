using System;
using System.Collections.Generic;

namespace artiso.AdsdHotel.Red.Data.Entities
{
    public class RoomRate
    {
        public string OrderId { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public IEnumerable<Rate> Rates { get; private set; }

        public RoomRate(string orderId, DateTime startDate, DateTime endDate, IEnumerable<Rate> rates)
        {
            OrderId = orderId;
            StartDate = startDate;
            EndDate = endDate;
            Rates = rates;
        }
    }
}
