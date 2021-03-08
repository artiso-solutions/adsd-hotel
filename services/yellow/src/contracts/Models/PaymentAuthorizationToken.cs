using System;

namespace artiso.AdsdHotel.Yellow.Contracts.Models
{
    public class PaymentAuthorizationToken
    {
        public PaymentAuthorizationToken(TimeSpan duration)
        {
            Token = Guid.NewGuid().ToString();
            CreatedAt = DateTime.Now;
            ExpirationDate = CreatedAt.Add(duration);
            Active = true;
        }

        public string Token { get; }
        
        public DateTime CreatedAt { get; }
        
        public DateTime ExpirationDate { get; }
        
        public bool Active { get; set; }
    }
}
