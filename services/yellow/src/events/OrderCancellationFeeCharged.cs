namespace artiso.AdsdHotel.Yellow.Events
{
    public record OrderCancellationFeeCharged(string OrderId);
    
    public record ChargeOrderCancellationFeeFailed(string OrderId, string? Reason);
}
