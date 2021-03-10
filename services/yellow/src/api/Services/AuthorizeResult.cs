using System;

namespace artiso.AdsdHotel.Yellow.Api.Services
{
    public record AuthorizeResult(string? AuthorizePaymentToken, Exception? Exception)
    {
        public bool IsSuccess => Exception is null;
    }
}
