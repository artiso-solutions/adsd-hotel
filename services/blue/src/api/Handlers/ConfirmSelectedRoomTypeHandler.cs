using System;
using System.Data;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Commands;
using artiso.AdsdHotel.Blue.Contracts;
using artiso.AdsdHotel.Blue.Validation;
using NServiceBus;
using RepoDb;
using static artiso.AdsdHotel.Blue.Api.DatabaseTableNames;

namespace artiso.AdsdHotel.Blue.Api.Handlers
{
    internal class ConfirmSelectedRoomTypeHandler : IHandleMessages<ConfirmSelectedRoomType>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ConfirmSelectedRoomTypeHandler(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task Handle(ConfirmSelectedRoomType message, IMessageHandlerContext context)
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
            using var transaction = connection.BeginTransaction();

            var pendingReservation = await FindPendingReservationAsync(connection, message.OrderId);

            if (pendingReservation is null)
            {
                await context.Reply(new Response<bool>(new Exception(
                    $"Could not find pending reservation for {nameof(message.OrderId)} {message.OrderId}")));
                
                return;
            }

            var isValid = await IsPendingReservationValidAsync(connection, pendingReservation);

            if (!isValid)
            {
                await context.Reply(new Response<bool>(new Exception(
                    "Pending reservation is not valid")));

                return;
            }

            await CreateReservationAsync(connection, pendingReservation);

            await MarkPendingReservationAsConfirmed(connection, pendingReservation.Id);

            await transaction.CommitAsync();

            await context.Reply(new Response<bool>(true));
        }

        private async Task<PendingReservation?> FindPendingReservationAsync(
            IDbConnection connection,
            string orderId)
        {
            var query = $@"
SELECT Id, OrderId, RoomTypeId, Start, End, CreatedAt
FROM {PendingReservations}
WHERE OrderId = @orderId";

            using var reader = await connection.ExecuteReaderAsync(query, new { orderId });

            if (!reader.Read())
                return null;

            var i = 0;
            var pendingReservation = new PendingReservation(
                Id: reader.GetString(i++),
                OrderId: reader.GetString(i++),
                RoomTypeId: reader.GetString(i++),
                Start: reader.GetDateTime(i++),
                End: reader.GetDateTime(i++),
                CreatedAt: reader.GetDateTime(i++));

            return pendingReservation;
        }

        private async Task<bool> IsPendingReservationValidAsync(
            IDbConnection connection,
            PendingReservation pendingReservation)
        {
            var query = $@"
SELECT Id FROM {RoomTypes}
WHERE Id = @RoomTypeId AND Id NOT IN (
    SELECT DISTINCT RoomTypeId FROM {Reservations}
    WHERE Start >= @Start AND Start <= @End)";

            using var reader = await connection.ExecuteReaderAsync(query,
                new { pendingReservation.RoomTypeId, pendingReservation.Start, pendingReservation.End });

            if (!reader.Read())
                return false;

            return true;
        }

        private async Task<Reservation> CreateReservationAsync(
            IDbConnection connection,
            PendingReservation pendingReservation)
        {
            var reservationId = Guid.NewGuid().ToString();

            var reservation = new Reservation(
                reservationId,
                pendingReservation.OrderId,
                pendingReservation.RoomTypeId,
                pendingReservation.Start,
                pendingReservation.End,
                DateTime.UtcNow);

            await connection.InsertAsync(Reservations, reservation);

            return reservation;
        }

        private async Task MarkPendingReservationAsConfirmed(
            IDbConnection connection,
            string pendingReservationId)
        {
            var query = $@"
UPDATE {PendingReservations}
SET Confirmed = True
WHERE PendingReservationId = @pendingReservationId";

            await connection.ExecuteNonQueryAsync(query, new { pendingReservationId });
        }
    }
}
