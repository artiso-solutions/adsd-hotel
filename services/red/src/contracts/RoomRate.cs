using System.Collections.Generic;

#nullable disable

namespace artiso.AdsdHotel.Red.Contracts
{
    public record RoomRate
    {
        public string Id { get; init; }

        public IReadOnlyList<RateItem> RateItems { get; init; }

        public CancellationFee CancellationFee { get; init; }

        public float TotalPrice { get; init; }
    }
}
