using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Ambassador;
using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;
using artiso.AdsdHotel.Purple.Ambassador;
using artiso.AdsdHotel.Yellow.Ambassador;
using artiso.AdsdHotel.Yellow.Contracts.Models;
using artiso.AdsdHotel.Yellow.Events.External;

namespace artiso.AdsdHotel.Purple.Consumer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello Hotel!");
            var orderId = Guid.NewGuid().ToString();

            // blue
            var blueAmbassador = BlueAmbassadorFactory.Create();
            var start = DateTime.Today.AddDays(7);
            var end = start.AddDays(7);

            var availableRoomTypes = await blueAmbassador.ListRoomTypesAvailableBetweenAsync(start, end);

            var desiredRoomType = availableRoomTypes.PickRandom();

            var roomTypeWasSelected = await blueAmbassador.SelectRoomTypeBetweenAsync(
                orderId,
                desiredRoomType.Id,
                start,
                end);

            //red
            await EmulateSelectionOfRate();

            // yellow
            var yelowAmbassador = YellowServiceClientFactory.Create();

            var creditCard = new CreditCard(
                IssuingNetwork.AmericanExpress,
                "John Doe",
                "349621197422556",
                "0000",
                DateTime.MaxValue);


            await yelowAmbassador.AddPaymentMethodToOrderRequest(orderId, creditCard);

            var purpleAmbassador = PurpleAmbassadorFactory.Create();
            await purpleAmbassador.CompleteReservationAsync(orderId);

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
            await channel.Publish(new OrderRateSelected(orderId, new Yellow.Events.External.Price(10, 100)));
            return orderId;
        }

       
    }

    public static class Extensions
    {
        public static T PickRandom<T>(this IReadOnlyList<T> list)
        {
            if (!list.Any()) throw new Exception("Cannot pick a random item from an empty list");
            var index = new Random().Next(0, list.Count);
            return list[index];
        }
    }
}
