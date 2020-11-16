using System;

namespace artiso.AdsdHotel.Black.Commands
{
    public record OrderIdRequest(string? FirstName, string? LastName, string? EMail);

    public record OrderIdRespone(Guid OrderId);
}
