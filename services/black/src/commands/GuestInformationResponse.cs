using artiso.AdsdHotel.Black.Contracts;

namespace artiso.AdsdHotel.Black.Commands
{
    public record GuestInformationResponse
    {
        public GuestInformation? GuestInformation { get; init; }

        public string? Error { get; init; }
    }
}
