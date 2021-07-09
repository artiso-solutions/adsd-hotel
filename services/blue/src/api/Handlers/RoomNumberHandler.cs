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
using NServiceBus;
using RepoDb;
using static artiso.AdsdHotel.Blue.Api.CommonQueries;
using static artiso.AdsdHotel.Blue.Api.DatabaseTableNames;

namespace artiso.AdsdHotel.Blue.Api.Handlers
{
    internal class RoomNumberHandler : IHandleMessages<GetRoomNumberRequest>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RoomNumberHandler(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task Handle(GetRoomNumberRequest message, IMessageHandlerContext context)
        {
            try
            {
                Ensure.Valid(message);
            }
            catch (ValidationException validationEx)
            {
                await context.Reply(new Response<GetRoomNumberResponse>(validationEx));
                return;
            }

            await using var connection = await _connectionFactory.CreateAsync();

            using var transaction = await connection.BeginTransactionAsync();

            var reservation = await FindReservationAsync(connection, message.OrderId);

            if (reservation is null)
            {
                await context.Reply(new Response<GetRoomNumberResponse>(new Exception(
                    $"Could not find a reservation with {nameof(message.OrderId)} {message.OrderId}")));

                return;
            }

            if (reservation.RoomId is not null)
            {
                var room = await FindRoomAsync(connection, reservation.RoomId);

                if (room is not null)
                {
                    // A room was already set on this reservation.
                    await transaction.CommitAsync();
                    await context.Reply(new Response<GetRoomNumberResponse>(new GetRoomNumberResponse(room.Number)));
                    return;
                }
            }

            // Select a room for this reservation.

            var availableRoom = await FindAnAvailableRoomFor(connection, reservation);

            if (availableRoom is null)
            {
                await context.Reply(new Response<GetRoomNumberResponse>(new Exception(
                    $"Could not find a free room for the reservation with {nameof(message.OrderId)} {message.OrderId}")));

                return;
            }

            await AssociateRoomToReservationAsync(connection, availableRoom, reservation);

            await transaction.CommitAsync();

            await context.Publish(new RoomNumberAssigned(message.OrderId, availableRoom.Number));
            await context.Reply(new Response<GetRoomNumberResponse>(new GetRoomNumberResponse(availableRoom.Number)));
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
