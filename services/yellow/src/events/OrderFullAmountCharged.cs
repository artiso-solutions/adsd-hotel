namespace artiso.AdsdHotel.Yellow.Events
{
    public record OrderFullAmountCharged(string OrderId);
    
    public record ChargeForOrderFullAmountFailed(string OrderId);
}
