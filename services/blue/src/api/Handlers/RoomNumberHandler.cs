using System;
using System.Data;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Commands;
using artiso.AdsdHotel.Blue.Contracts;
using artiso.AdsdHotel.Blue.Events;
using artiso.AdsdHotel.ITOps.Sql;
using NServiceBus;
using RepoDb;
using static artiso.AdsdHotel.Blue.Api.CommonQueries;
using static artiso.AdsdHotel.Blue.Api.DatabaseTableNames;

namespace artiso.AdsdHotel.Blue.Api.Handlers
{
    internal class RoomNumberHandler : IHandleMessages<SetRoomNumber>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RoomNumberHandler(IDbConnectionFactory connectionFactory) =>
            _connectionFactory = connectionFactory;

        public async Task Handle(SetRoomNumber command, IMessageHandlerContext context)
        {
            var orderId = command.OrderId;

            await using var connection = await _connectionFactory.CreateAsync();

            var reservation = await FindReservationAsync(connection, orderId);

            if (reservation is null)
                throw new InvalidOperationException($"Could not find a reservation with {nameof(orderId)} {orderId}");

            if (reservation.RoomId is not null)
            {
                var room = await FindRoomAsync(connection, reservation.RoomId);

                if (room is not null)
                    // A room was already set on this reservation.
                    return;
            }

            // Select a room for this reservation.

            using var transaction = await connection.BeginTransactionAsync();

            var availableRoom = await FindAnAvailableRoomFor(connection, reservation);

            if (availableRoom is null)
            {
                throw new InvalidOperationException(
                    $"Could not find a free room for the reservation with {nameof(orderId)} {orderId}");
            }

            await AssociateRoomToReservationAsync(connection, availableRoom, reservation);

            await transaction.CommitAsync();

            await context.Publish(new RoomNumberAssigned(orderId, availableRoom.Number));
        }

        private async Task<Room?> FindAnAvailableRoomFor(IDbConnection connection, Reservation reservation)
        {
            // A room should be:
            // - free given the reservation's period
            // - matching the reservation's room type

            var query = $@"
SELECT Id, RoomTypeId, Number
FROM {Rooms}
WHERE RoomTypeId = @RoomTypeId
AND Id NOT IN (
    SELECT RoomId
    FROM {V_Reservations}
    WHERE Start >= @Start AND Start <= @End)";

            using var reader = await connection.ExecuteReaderAsync(query, new
            {
                reservation.RoomTypeId,
                reservation.Start,
                reservation.End
            });

            if (!await reader.ReadAsync())
                return null;

            var i = 0;
            var room = new Room(
                Id: reader.GetString(i++),
                RoomTypeId: reader.GetString(i++),
                Number: reader.GetString(i++));

            return room;
        }

        private async Task AssociateRoomToReservationAsync(
            IDbConnection connection,
            Room availableRoom,
            Reservation reservation)
        {
            var query = $@"
UPDATE {Reservations}
SET RoomId = @roomId
WHERE Id = @reservationId";

            await connection.ExecuteNonQueryAsync(query, new
            {
                roomId = availableRoom.Id,
                reservationId = reservation.Id
            });
        }
    }
}
