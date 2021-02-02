using System;

namespace artiso.AdsdHotel.Yellow.Contracts.Models
{
    public record OrderCreditCard(
        IssuingNetwork IssuingNetwork,
        string CardNumber,
        string ProviderPaymentToken);
    
    public record CreditCard(
        IssuingNetwork IssuingNetwork,
        string CardHolder,
        long CardNumber,
        int Cvv,
        DateTime ExpirationDate);

    public enum IssuingNetwork
    {
        AmericanExpress,
        MasterCard,
        Visa,
        Maestro,
        DinersClub, 
        Jcb,
        Discover
    }
}
