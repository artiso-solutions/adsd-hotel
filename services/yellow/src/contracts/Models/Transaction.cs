using System;

namespace artiso.AdsdHotel.Yellow.Contracts.Models
{
    public record Transaction(string Id, 
        string PaymentAuthorizationTokenId, 
        decimal Amount, 
        DateTime CreatedAt);
}