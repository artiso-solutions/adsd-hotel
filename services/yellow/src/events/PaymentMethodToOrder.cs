namespace artiso.AdsdHotel.Yellow.Events
{

    public record PaymentMethodToOrderAdded(string OrderId);

    public record AddPaymentMethodToOrderFailed(string OrderId, string? Reason);
}
