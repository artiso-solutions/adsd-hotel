using System;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Commands;
using artiso.AdsdHotel.Blue.Contracts;
using MySql.Data.MySqlClient;
using NServiceBus;
using RepoDb;

namespace artiso.AdsdHotel.Blue.Api
{
    public class RoomTypeSelectionHandler : IHandleMessages<SelectRoomType>
    {
        public async Task Handle(SelectRoomType message, IMessageHandlerContext context)
        {
            using (var connection = new MySqlConnection("Server=localhost;Port=13306;Database=adsd-blue;"))
            {
                var query = @"
SELECT * FROM RoomTypes
WHERE Id = @RoomTypeId AND Id NOT IN (
    SELECT DISTINCT RoomTypeId FROM Reservations
    WHERE Start >= @Start AND Start <= @End
);";

                var queryResult = await connection.ExecuteQueryAsync<RoomType>(query,
                    new { message.RoomTypeId, message.Start, message.End });

                var availableRoomType = queryResult.ToArray().FirstOrDefault();

                if (availableRoomType is null)
                    // TODO: reply to caller with "fault" message.
                    throw new Exception(
                        $"Room type '{message.RoomTypeId}' is not available " +
                        $"anymore on the given period: {message.Start} - {message.End}");

                var pendingReservation = new PendingReservation(
                    Guid.NewGuid().ToString(),
                    message.OrderId,
                    message.RoomTypeId,
                    message.Start,
                    message.End,
                    DateTime.UtcNow);

                await connection.InsertAsync(pendingReservation);
            }
        }
    }
}
