using System;

namespace artiso.AdsdHotel.Yellow.Contracts.Models
{
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
