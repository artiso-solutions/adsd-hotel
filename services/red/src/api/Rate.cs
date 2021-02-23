namespace artiso.AdsdHotel.Red.Api
{
    public class Rate
    {
        public Rate(Price price)
        {
            Price = price;
        }

        public Price Price { get; set; }
    }
}