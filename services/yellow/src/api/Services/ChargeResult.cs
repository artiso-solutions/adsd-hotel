using System;
using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Api.Services
{
    public record ChargeResult
    {
        public Transaction transaction { get; init; }
        
        public string AuthorizePaymentToken { get; init; }
        
        public Exception? Exception { get; init; }
        
        public bool IsSuccess => Exception is null;
    }
}
