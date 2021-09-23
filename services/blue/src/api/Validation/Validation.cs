using System;
using System.ComponentModel.DataAnnotations;
using artiso.AdsdHotel.Blue.Api.Models;

namespace artiso.AdsdHotel.Blue.Validation
{
    internal static partial class Ensure
    {
        public static void Valid(string s, string? variableName = null)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new ValidationException($"{(variableName ?? "String variable")} must not be null or white space.");
        }

        public static void Valid(DateTime start, DateTime end)
        {
            if (end < start)
                throw new ValidationException($"{nameof(end)} must be greater than or equal to {nameof(start)}");
        }

        public static void Valid(AvailableRoomTypesRequest message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(AvailableRoomTypesRequest));

            if (message.Start == message.End)
                throw new ValidationException($"Dates range must not be 0.");

            if (message.End < message.Start)
                throw new ValidationException($"'{nameof(message.Start)}' must be before '{nameof(message.End)}'.");
        }
    }
}
