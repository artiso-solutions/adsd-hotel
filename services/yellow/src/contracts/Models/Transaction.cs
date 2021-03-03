using System;

namespace artiso.AdsdHotel.Yellow.Contracts.Models
{
    public record Transaction(string Id, string PaymentCode, decimal Amount, DateTime CreatedAt);
}