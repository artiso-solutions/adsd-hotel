namespace artiso.AdsdHotel.Black.Contracts.Validation
{
    public static class GuestInformationValidator
    {
        public static bool IsValid(GuestInformation gi)
        {
            if (string.IsNullOrEmpty(gi.FirstName))
                return false;
            if (string.IsNullOrEmpty(gi.LastName))
                return false;
            if (string.IsNullOrEmpty(gi.EMail))
                return false;
            return true;
        }
    }
}
