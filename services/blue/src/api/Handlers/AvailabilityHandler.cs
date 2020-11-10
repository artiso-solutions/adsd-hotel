using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Commands;
using artiso.AdsdHotel.Blue.Contracts;
using NServiceBus;
using RepoDb;
using static artiso.AdsdHotel.Blue.Api.DatabaseTableNames;

namespace artiso.AdsdHotel.Blue.Api
{
    internal class AvailabilityHandler : IHandleMessages<RequestAvailableRoomTypes>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AvailabilityHandler(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task Handle(RequestAvailableRoomTypes message, IMessageHandlerContext context)
        {
            await using var connection = await _connectionFactory.CreateAsync();

            var query = $@"
SELECT * FROM {RoomTypes}
WHERE Id NOT IN (
    SELECT DISTINCT RoomTypeId FROM Reservations
    WHERE Start >= @Start AND Start <= @End)";

            var queryResult = await connection.ExecuteQueryAsync<RoomType>(query, new { message.Start, message.End });

            var availableRoomTypes = queryResult.ToArray();

            await context.Reply(new AvailableRoomTypesResponse { RoomTypes = availableRoomTypes });
        }
    }
}
