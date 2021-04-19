using System;

namespace artiso.AdsdHotel.Red.Persistence.Entities
{
    public class CancellationFeeEntity
    {
        public DateTime DeadLine { get; set; }

        public float FeeInPercentage { get; set; }
    }
}