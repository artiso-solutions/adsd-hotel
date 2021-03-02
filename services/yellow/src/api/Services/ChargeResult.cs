using System;

namespace artiso.AdsdHotel.Yellow.Api.Services
{
    public record ChargeResult()
    {
        public string? AuthorizePaymentToken { get; init; }
        
        public Exception? Exception { get; init; }
        
        public bool IsSuccess => Exception is null;
    }
}
