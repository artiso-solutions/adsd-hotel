using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.Yellow.Ambassador;
using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Consumer
{
    class Program
    {
        private static async Task Main()
        {
            var ambassador = YellowServiceClientFactory.Create();

            var orderId = Guid.NewGuid().ToString();
            const decimal cancellationFee = 10;
            const decimal totalAmount = 100;

            Console.WriteLine("Start operation");
            
            await ambassador.OrderRateSelectedCommand(orderId, cancellationFee, totalAmount);
            
            Console.WriteLine($"Order created: {orderId}");
            
            await ambassador.AuthorizeOrderCancellationFee(orderId, new CreditCard(
                IssuingNetwork.AmericanExpress,
                "John Doe", 
                "349621197422556", 
                "0000", 
                DateTime.MaxValue));
            
            Console.WriteLine("CancellationFeeAuthorized");

            await ambassador.ChargeCancellationFee(orderId);
            
            Console.WriteLine("CancellationFeeCharged");
            
            await ambassador.ChargeFullAmount(orderId);
        }
    }
}
