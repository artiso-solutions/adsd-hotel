using System;

namespace artiso.AdsdHotel.Yellow.Contracts.Models
{
    public class PaymentAuthorizationToken
    {
        public PaymentAuthorizationToken(TimeSpan duration)
        {
            Id = Guid.NewGuid().ToString();
            CreatedAt = DateTime.Now;
            ExpirationDate = CreatedAt.Add(duration);
            Active = true;
        }
        
        public string Id { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime ExpirationDate { get; set; }
        
        public bool Active { get; set; }
    }
}
