﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Ambassador;
using artiso.AdsdHotel.Purple.Ambassador;
using artiso.AdsdHotel.Red.Ambassador;
using artiso.AdsdHotel.Yellow.Ambassador;
using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Purple.Consumer
{
    public class Program
    {
        public static async Task Main()
        {
            Console.WriteLine("Hello Hotel!");
            var orderId = Guid.NewGuid().ToString();

            // blue
            var blueAmbassador = BlueAmbassadorFactory.Create();
            var start = DateTime.Today.AddDays(7);
            var end = start.AddDays(7);

            var availableRoomTypes = await blueAmbassador.ListRoomTypesAvailableBetweenAsync(start, end);

            var desiredRoomType = availableRoomTypes.PickRandom();

            _ = await blueAmbassador.SelectRoomTypeBetweenAsync(
                orderId,
                desiredRoomType.Id,
                start,
                end);

            // red
            var redAmbassador = RedAmbassadorFactory.Create();
            _ = await redAmbassador.InputRoomRatesAsync(
                desiredRoomType.Id,
                orderId,
                DateTime.Now,
                DateTime.Now + TimeSpan.FromDays(14));

            // yellow
            var yellowAmbassador = YellowServiceClientFactory.Create();

            var creditCard = new CreditCard(
                IssuingNetwork.AmericanExpress,
                "John Doe",
                "349621197422556",
                "0000",
                DateTime.MaxValue);

            await yellowAmbassador.AddPaymentMethodToOrderRequest(orderId, creditCard);

            var purpleAmbassador = PurpleAmbassadorFactory.Create();
            await purpleAmbassador.CompleteReservationAsync(orderId);
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
