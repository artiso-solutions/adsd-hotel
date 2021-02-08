namespace artiso.AdsdHotel.Yellow.Events.External
{
    // TODO : To be reviewed according to red service contracts definition
    public record Rate(Price Price);
    public record Price(decimal CancellationFee, decimal Amount);
}
