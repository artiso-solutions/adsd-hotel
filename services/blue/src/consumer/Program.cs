using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Ambassador;

namespace artiso.AdsdHotel.Blue.Consumer
{
    public class Program
    {
        public static async Task Main()
        {
            var ambassador = BlueAmbassadorFactory.Create();

            var start = DateTime.Today.AddDays(7);
            var end = start.AddDays(7);

            var availableRoomTypes = await ambassador.ListRoomTypesAvailableBetweenAsync(start, end);

            var orderId = Guid.NewGuid().ToString();
            var desiredRoomType = availableRoomTypes.PickRandom();

            var roomTypeWasSelected = await ambassador.SelectRoomTypeBetweenAsync(
                orderId,
                desiredRoomType.Id,
                start,
                end);

            if (!roomTypeWasSelected)
                throw new Exception($"Unable to select room type '{desiredRoomType.Id}'");

            var roomTypeWasConfirmed = await ambassador.ConfirmSelectedRoomTypeAsync(orderId);

            if (!roomTypeWasConfirmed)
                throw new Exception($"Unable to confirm room type '{desiredRoomType.Id}'");

            var roomNumber = await ambassador.GetReservationRoomNumberAsync(orderId);

            if (roomNumber is null)
                throw new Exception($"Unable to get room number for order '{orderId}'");

            var orderSummary = await ambassador.GetOrderSummaryAsync(orderId);

            Console.WriteLine(orderSummary);
        }
    }
}
