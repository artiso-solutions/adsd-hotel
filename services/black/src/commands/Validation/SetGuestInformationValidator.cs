namespace artiso.AdsdHotel.Black.Commands.Validation
{
    public static class SetGuestInformationValidator
    {
        public static bool IsValid(SetGuestInformation sgi)
        {
            if (string.IsNullOrEmpty(sgi.GuestInformation.FirstName))
                return false;
            if (string.IsNullOrEmpty(sgi.GuestInformation.LastName))
                return false;
            if (string.IsNullOrEmpty(sgi.GuestInformation.EMail))
                return false;
            return true;
        }
    }
}
