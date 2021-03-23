namespace artiso.AdsdHotel.Yellow.Events
{
    public record OrderCancellationFeeAuthorizationAcquired(string OrderId);

    public record AuthorizeOrderCancellationFeeFailed(string OrderId);
}
