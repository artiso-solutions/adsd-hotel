using System;
using System.ComponentModel.DataAnnotations;
using artiso.AdsdHotel.Blue.Commands;

namespace artiso.AdsdHotel.Blue.Validation
{
    internal static partial class Ensure
    {
        public static void Valid(ConfirmSelectedRoomType message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(ConfirmSelectedRoomType));

            if (string.IsNullOrWhiteSpace(message.OrderId))
                throw new ValidationException($"Missing '{nameof(message.OrderId)}'.");
        }

        public static void Valid(SelectRoomType message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(SelectRoomType));

            if (string.IsNullOrWhiteSpace(message.RoomTypeId))
                throw new ValidationException($"Missing '{nameof(message.RoomTypeId)}'.");

            if (message.Start == message.End)
                throw new ValidationException($"Dates range must not be 0.");

            if (message.End < message.Start)
                throw new ValidationException($"'{nameof(message.Start)}' must be before '{nameof(message.End)}'.");
        }

        public static void Valid(SetRoomNumber message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(SetRoomNumber));

            if (string.IsNullOrWhiteSpace(message.OrderId))
                throw new ValidationException($"Missing '{nameof(message.OrderId)}'.");
        }
    }
}
