using System;

namespace artiso.AdsdHotel.Red.Contracts
{
    public class CancellationFee
    {
        public DateTime DeadLine { get; init; }

        public float FeeInPercentage { get; init; }
    }
}
