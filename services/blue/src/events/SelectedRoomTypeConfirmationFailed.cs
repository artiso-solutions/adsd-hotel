namespace artiso.AdsdHotel.Blue.Events
{
    public record SelectedRoomTypeConfirmationFailed(
        string OrderId,
        string? Reason);
}
