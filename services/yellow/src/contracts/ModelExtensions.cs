using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Contracts
{
    public static class ModelExtensions
    {
        public static OrderCreditCard GetOrderCreditCard(this CreditCard creditCard, string token)
        {
            var pan = "************1650";
            
            return new OrderCreditCard(creditCard.IssuingNetwork, pan, token);
        }
    }
}
