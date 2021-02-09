using System.Collections.Generic;

namespace artiso.AdsdHotel.Yellow.Contracts.Models
{
    public record Order(string Id, Price Price)
    {
        /// <summary>
        /// Payment methods previously used
        /// </summary>
        public List<OrderPaymentMethod> OrderPaymentMethods { get; set; }
    }
    
    public record Price(decimal CancellationFee, decimal Amount);
}
