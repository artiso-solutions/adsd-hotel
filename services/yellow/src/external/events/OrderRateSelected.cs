namespace artiso.AdsdHotel.Yellow.Events.External
{
    public record OrderRateSelected(string OrderId, Price Price);
    
    public record OrderRateSelectedCreated(string OrderId);
}
