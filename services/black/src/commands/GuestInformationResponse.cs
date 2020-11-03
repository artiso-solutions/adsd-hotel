using artiso.AdsdHotel.Black.Contracts;

namespace artiso.AdsdHotel.Black.Commands
{
    public class GuestInformationResponse
    {
        public GuestInformation? GuestInformation { get; set; }

        public string? Error { get; set; }
    }
}
