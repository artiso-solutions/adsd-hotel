using System;
using System.Data;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Commands;
using artiso.AdsdHotel.Blue.Contracts;
using artiso.AdsdHotel.Blue.Events;
using artiso.AdsdHotel.Blue.Validation;
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
            try
            {
                Ensure.Valid(message);
            }
            catch (ValidationException validationEx)
            {
                await context.Reply(new Response<bool>(validationEx));
                return;
            }

            await using var connection = await _connectionFactory.CreateAsync();

            var exists = await ExistsAsync(connection, message.RoomTypeId);

            if (!exists)
                throw new Exception();

            var isAvailable = await IsAvailableAsync(
                connection,
                message.RoomTypeId,
                message.Start,
                message.End);

            if (!isAvailable)
            {
                await context.Reply(new Response<bool>(new Exception(
                    $"Room type '{message.RoomTypeId}' is not available " +
                    $"anymore on the given period: {message.Start} - {message.End}")));

                return;
            }

            var pendingReservation = new PendingReservation(
                Guid.NewGuid().ToString(),
                message.OrderId,
                message.RoomTypeId,
                message.Start,
                message.End,
                DateTime.UtcNow);

            await connection.InsertAsync(PendingReservations, pendingReservation);

            await context.Publish(new RoomTypeSelected(
                message.OrderId,
                message.RoomTypeId,
                message.Start,
                message.End,
                DateTime.UtcNow));

            await context.Reply(new Response<bool>(true));
        }

        private async Task<bool> ExistsAsync(IDbConnection connection, string roomTypeId)
        {
            var query = $@"
SELECT Id FROM {RoomTypes}
WHERE Id = @roomTypeId
LIMIT 1";

            using var reader = await connection.ExecuteReaderAsync(query, new { roomTypeId });

            if (!await reader.ReadAsync())
                return false;

            return true;
        }

        private async Task<bool> IsAvailableAsync(
            IDbConnection connection,
            string roomTypeId,
            DateTime start,
            DateTime end)
        {
            var query = $@"
SELECT Id FROM {RoomTypes}
WHERE Id = @roomTypeId AND Id NOT IN (
    SELECT DISTINCT RoomTypeId FROM {Reservations}
    WHERE Start >= @start AND Start <= @end)
LIMIT 1";

            using var reader = await connection.ExecuteReaderAsync(query, new
            {
                roomTypeId,
                start,
                end
            });

            if (!await reader.ReadAsync())
                return false;

            return true;
        }
    }
}
