namespace artiso.AdsdHotel.Yellow.Contracts.Models
{
    public record PaymentMethod(CreditCard? CreditCard);

    public record StoredPaymentMethod(StoredCreditCard CreditCard);
}
