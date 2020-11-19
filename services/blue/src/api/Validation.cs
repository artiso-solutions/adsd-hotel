using System;
using artiso.AdsdHotel.Blue.Commands;

namespace artiso.AdsdHotel.Blue.Validation
{
    internal static class Ensure
    {
        public static void Valid(AvailableRoomTypesRequest message)
        {
            if (message is null)
                throw new NullReferenceException(nameof(AvailableRoomTypesRequest));

            if (message.Start == message.End)
                throw new ValidationException($"Dates range must not be 0.");

            if (message.End < message.Start)
                throw new ValidationException($"'{nameof(message.Start)}' must be before '{nameof(message.End)}'.");
        }

        public static void Valid(ConfirmSelectedRoomType message)
        {
            if (message is null)
                throw new NullReferenceException(nameof(ConfirmSelectedRoomType));

            if (string.IsNullOrWhiteSpace(message.OrderId))
                throw new ValidationException($"Missing '{nameof(message.OrderId)}'.");
        }

        public static void Valid(SelectRoomType message)
        {
            if (message is null)
                throw new NullReferenceException(nameof(ConfirmSelectedRoomType));

            if (string.IsNullOrWhiteSpace(message.OrderId))
                throw new ValidationException($"Missing '{nameof(message.OrderId)}'.");

            if (string.IsNullOrWhiteSpace(message.RoomTypeId))
                throw new ValidationException($"Missing '{nameof(message.RoomTypeId)}'.");

            if (message.Start == message.End)
                throw new ValidationException($"Dates range must not be 0.");

            if (message.End < message.Start)
                throw new ValidationException($"'{nameof(message.Start)}' must be before '{nameof(message.End)}'.");
        }

        public static void Valid(GetRoomNumberRequest message)
        {
            if (message is null)
                throw new NullReferenceException(nameof(ConfirmSelectedRoomType));

            if (string.IsNullOrWhiteSpace(message.OrderId))
                throw new ValidationException($"Missing '{nameof(message.OrderId)}'.");
        }
    }

    public class ValidationException : Exception
    {
        public ValidationException(string message)
            : base(message)
        {
        }
    }
}
