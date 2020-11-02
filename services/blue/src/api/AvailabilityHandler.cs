using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Commands;
using artiso.AdsdHotel.Blue.Contracts;
using MySql.Data.MySqlClient;
using NServiceBus;
using RepoDb;

namespace artiso.AdsdHotel.Blue.Api
{
    class AvailabilityHandler : IHandleMessages<RequestAvailableRoomTypes>
    {
        public async Task Handle(RequestAvailableRoomTypes message, IMessageHandlerContext context)
        {
            using (var connection = new MySqlConnection("Server=localhost;Port=13306;Database=adsd-blue;"))
            {
                var query = @"
SELECT * FROM RoomTypes
WHERE Id NOT IN (
    SELECT DISTINCT RoomTypeId FROM Reservations
    WHERE Start >= @Start AND Start <= @End
);";

                var queryResult = await connection.ExecuteQueryAsync<RoomType>(query,
                    new { message.Start, message.End });

                var availableRoomTypes = queryResult.ToArray();

                await context.Reply(new AvailableRoomTypesResponse { RoomTypes = availableRoomTypes });
            }
        }
    }
}
