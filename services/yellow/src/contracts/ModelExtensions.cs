using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Contracts
{
    public static class ModelExtensions
    {
        public static StoredCreditCard GetOrderCreditCard(this CreditCard creditCard, string token)
        {
            var pan = creditCard.CardNumber.Substring(creditCard.CardNumber.Length - 4, 4).PadLeft(creditCard.CardNumber.Length, '*');
            
            return new StoredCreditCard(creditCard.IssuingNetwork,  creditCard.CardHolder, creditCard.ExpirationDate, pan, token);
        }
        
        public static OrderTransaction GetOrderTransaction(this Transaction transaction, StoredPaymentMethod usedPaymentMethod)
        {
            return new(transaction.Id, transaction.Amount, usedPaymentMethod, transaction.CreatedAt);
        }
    }
}
