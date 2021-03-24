namespace artiso.AdsdHotel.Red.Contracts
{
    public class Rate
    {
        public Rate(Price price)
        {
            Price = price;
        }

        public Price Price { get; }
    }
}