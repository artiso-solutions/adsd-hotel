#nullable disable

namespace artiso.AdsdHotel.Red.Contracts
{
    public record RateItem
    {
        public string Id { get; init; }

        public float Price { get; set; }
    }
}
