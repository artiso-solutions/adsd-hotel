using System;

namespace artiso.AdsdHotel.Yellow.Contracts.Models
{
    public class PaymentAuthorizationToken
    {
        public PaymentAuthorizationToken()
        {
            
        }

        public PaymentAuthorizationToken(TimeSpan duration)
        {
            Token = Guid.NewGuid().ToString();
            CreatedAt = DateTime.Now;
            ExpirationDate = CreatedAt.Value.Add(duration);
            Active = true;
        }

        public string? Token { get; init; }
        
        public DateTime? CreatedAt { get; }
        
        public DateTime? ExpirationDate { get; }
        
        public bool Active { get; set; }
    }
}
