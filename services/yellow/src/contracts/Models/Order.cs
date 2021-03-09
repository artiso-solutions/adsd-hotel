using System;
using System.Collections.Generic;

namespace artiso.AdsdHotel.Yellow.Contracts.Models
{
    public record Order(string Id, Price Price)
    {
        /// <summary>
        /// Payment methods previously used
        /// </summary>
        public List<StoredPaymentMethod>? PaymentMethods { get; set; }

        /// <summary>
        /// Summary of all the transactions of the Order
        /// </summary>
        public List<OrderTransaction>? Transactions { get; set; }
    }
    
    public record Price(decimal CancellationFee, decimal Amount);
    
    public record OrderTransaction(string TransactionId, decimal Amount, StoredPaymentMethod UsedPaymentMethod, DateTime CreatedAt);
}
