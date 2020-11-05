using artiso.AdsdHotel.Black.Contracts;

namespace artiso.AdsdHotel.Black.Commands
{
    public class GuestInformationResponse
    {
        public GuestInformationResponse(GuestInformation guestInformation)
        {
            GuestInformation = guestInformation;
        }

        public GuestInformationResponse(string error)
        {
            Error = error;
        }

        public GuestInformation? GuestInformation { get; }

        public string? Error { get; }
    }
}
