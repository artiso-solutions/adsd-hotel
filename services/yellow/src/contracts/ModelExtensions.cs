using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Contracts
{
    public static class ModelExtensions
    {
        public static OrderCreditCard GetOrderCreditCard(this CreditCard creditCard, string token)
        {
            var pan = creditCard.CardNumber.Substring(creditCard.CardNumber.Length - 4, 4).PadLeft(creditCard.CardNumber.Length, '*');
            
            return new OrderCreditCard(creditCard.IssuingNetwork,  creditCard.CardHolder, creditCard.ExpirationDate, pan, token);
        }
    }
}
