using System;

namespace artiso.AdsdHotel.Yellow.Contracts.Models
{
    public record OrderCreditCard(
        IssuingNetwork IssuingNetwork,
        string CardHolder,
        DateTime ExpirationDate,
        string CardNumber,
        string? ProviderPaymentToken);
    
    public record CreditCard(
        IssuingNetwork IssuingNetwork,
        string CardHolder,
        string CardNumber,
        string Cvv,
        DateTime ExpirationDate);

    public enum IssuingNetwork
    {
        AmericanExpress,
        MasterCard,
        Visa,
        // Maestro,
        // DinersClub, 
        // Jcb,
        // Discover
    }
}
