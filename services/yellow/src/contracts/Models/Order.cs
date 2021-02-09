namespace artiso.AdsdHotel.Yellow.Contracts.Models
{
    public record Order(string Id, Price Price)
    {
        public OrderPaymentMethod? OrderPaymentMethod { get; set; }
    }
    
    public record Price(decimal CancellationFee, decimal Amount);
}
