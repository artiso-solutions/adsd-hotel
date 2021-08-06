using artiso.AdsdHotel.ITOps.Validation;

namespace artiso.AdsdHotel.Black.Contracts
{
    public record GuestInformation(string FirstName, string LastName, string EMail);

    public static class Ensure
    {
        /// <summary>
        /// Validates the <see cref="GuestInformation"/>.
        /// Throws a <see cref="MultiValidationException"/>, which can contain multiple errors, if anything is not valid.
        /// </summary>
        /// <param name="gi">The GuestInformation to validate.</param>
        public static void IsValid(GuestInformation gi)
        {
            var validation = new MultiValidationException();
            if (string.IsNullOrEmpty(gi.FirstName))
                validation.Add($"{nameof(GuestInformation)}.{nameof(GuestInformation.FirstName)}", $"The {nameof(GuestInformation.FirstName)} field cannot be empty.");
            if (string.IsNullOrEmpty(gi.LastName))
                validation.Add($"{nameof(GuestInformation)}.{nameof(GuestInformation.LastName)}", $"The {nameof(GuestInformation.LastName)} field cannot be empty.");
            if (string.IsNullOrEmpty(gi.EMail))
                validation.Add($"{nameof(GuestInformation)}.{nameof(GuestInformation.EMail)}", $"The {nameof(GuestInformation.EMail)} field cannot be empty.");
            if (validation.Errors.Count > 0)
                throw validation;
        }
    }
}
