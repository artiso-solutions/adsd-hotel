using System;
using System.Collections.Generic;

namespace artiso.AdsdHotel.Black.Commands
{
    public record OrderIdRequest(string? FirstName, string? LastName, string? EMail);

    public record OrderIdRespone(List<Guid> OrderIds);
}
