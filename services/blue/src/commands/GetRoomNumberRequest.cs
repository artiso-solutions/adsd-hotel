
namespace artiso.AdsdHotel.Blue.Commands
{
    public record GetRoomNumberRequest(string OrderId);

    public record GetRoomNumberResponse(string RoomNumber);
}
