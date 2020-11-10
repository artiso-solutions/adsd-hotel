using System;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Commands;
using artiso.AdsdHotel.Blue.Contracts;
using NServiceBus;
using RepoDb;
using static artiso.AdsdHotel.Blue.Api.DatabaseTableNames;

namespace artiso.AdsdHotel.Blue.Api
{
    internal class RoomTypeSelectionHandler : IHandleMessages<SelectRoomType>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RoomTypeSelectionHandler(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task Handle(SelectRoomType message, IMessageHandlerContext context)
        {
            await using var connection = await _connectionFactory.CreateAsync();

            var query = $@"
SELECT * FROM {RoomTypes}
WHERE Id = @RoomTypeId AND Id NOT IN (
    SELECT DISTINCT RoomTypeId FROM {Reservations}
    WHERE Start >= @Start AND Start <= @End)";

            var queryResult = await connection.ExecuteQueryAsync<RoomType>(query, new
            {
                message.RoomTypeId,
                message.Start,
                message.End
            });

            var availableRoomType = queryResult.FirstOrDefault();

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

            await connection.InsertAsync(PendingReservations, pendingReservation);
        }
    }
}
