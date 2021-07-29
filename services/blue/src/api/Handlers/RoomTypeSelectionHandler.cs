using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Commands;
using artiso.AdsdHotel.Blue.Contracts;
using artiso.AdsdHotel.Blue.Events;
using artiso.AdsdHotel.Blue.Validation;
using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.ITOps.Sql;
using Microsoft.Extensions.Logging;
using NServiceBus;
using RepoDb;
using static artiso.AdsdHotel.Blue.Api.DatabaseTableNames;

namespace artiso.AdsdHotel.Blue.Api
{
    internal class RoomTypeSelectionHandler : IHandleMessages<SelectRoomType>
    {
        private readonly ILogger<RoomTypeSelectionHandler> _logger;
        private readonly IDbConnectionFactory _connectionFactory;

        public RoomTypeSelectionHandler(
            ILogger<RoomTypeSelectionHandler> logger,
            IDbConnectionFactory connectionFactory)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
        }

        public async Task Handle(SelectRoomType message, IMessageHandlerContext context)
        {
            Ensure.Valid(message);

            await using var connection = await _connectionFactory.CreateAsync();

            var exists = await ExistsAsync(connection, message.RoomTypeId);

            if (!exists)
                throw new InvalidOperationException ($"Room type with {nameof(message.RoomTypeId)} {message.RoomTypeId} does not exist.");

            var isAvailable = await IsAvailableAsync(
                connection,
                message.RoomTypeId,
                message.Start,
                message.End);

            if (!isAvailable)
            {
                _logger.LogDebug(
                    $"Room type '{message.RoomTypeId}' is not available " +
                    $"anymore on the given period: {message.Start} - {message.End}");

                await context.Publish(new RoomTypeNotAvailable(
                    message.OrderId,
                    message.Start,
                    message.End,
                    message.RoomTypeId));

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
