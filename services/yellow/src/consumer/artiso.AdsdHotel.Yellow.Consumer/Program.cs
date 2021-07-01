using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;
using artiso.AdsdHotel.Red.Contracts;
using artiso.AdsdHotel.Red.Events;
using artiso.AdsdHotel.Yellow.Ambassador;
using artiso.AdsdHotel.Yellow.Contracts.Models;
using Price = artiso.AdsdHotel.Red.Contracts.Price;

namespace artiso.AdsdHotel.Yellow.Consumer
{
    public class Program
    {
        private static async Task Main()
        {
            Console.WriteLine("Start operation");

            var orderId = await EmulateSelectionOfRate();

            await Task.Delay(1000);

            var ambassador = YellowServiceClientFactory.Create();

            var creditCard = new CreditCard(
                IssuingNetwork.AmericanExpress,
                "John Doe",
                "349621197422556",
                "0000",
                DateTime.MaxValue);


            await ambassador.AddPaymentMethodToOrderRequest(orderId, creditCard);

            Console.WriteLine($"Add created: {orderId}");

            await ambassador.AuthorizeOrderCancellationFee(orderId);

            Console.WriteLine($"Order created: {orderId}");

            await ambassador.AuthorizeOrderCancellationFee(orderId);

            Console.WriteLine("CancellationFeeAuthorized");

            await ambassador.ChargeCancellationFee(orderId);

            Console.WriteLine("CancellationFeeCharged");

            await ambassador.ChargeFullAmount(orderId);
            
            Console.WriteLine("End operation");
        }

        private static async Task<string> EmulateSelectionOfRate()
        {
            var channel = NServiceBusChannelFactory.Create(
                channelName: "Mock.Red.Ambassador",
                endpointDestination: "Yellow.Api",
                "host=localhost;username=user;password=bitnami",
                useCallbacks: true
            );

            var orderId = Guid.NewGuid().ToString();

            await channel.Publish(new OrderRateSelected(orderId, new Rate(
                new Price
                {
                    Amount = 100,
                    CancellationFee = 10
                }
            )));

            return orderId;
        }
    }
}
