using System;
using artiso.AdsdHotel.Black.Contracts;

namespace artiso.AdsdHotel.Black.Commands
{
    public record SetGuestInformation(Guid OrderId, GuestInformation GuestInformation);

    public record GuestInformationRequest(Guid OrderId);

    public record GuestInformationResponse
    {
        public GuestInformation? GuestInformation { get; init; }

        public string? Error { get; init; }
    }
}
