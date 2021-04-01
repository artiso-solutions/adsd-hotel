using System;

namespace artiso.AdsdHotel.Red.Persistence.Entities
{
    public class CancellationFee
    {
        public DateTime DeadLine { get; set; }

        public float FeeInPercentage { get; set; }
    }
}