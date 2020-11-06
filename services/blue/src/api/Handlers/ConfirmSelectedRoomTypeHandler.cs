using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Commands;
using artiso.AdsdHotel.Blue.Contracts;
using NServiceBus;
using RepoDb;

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
            await using var connection = await _connectionFactory.CreateAsync();
            using var transaction = connection.BeginTransaction();

            var pendingReservation = await FindPendingReservationAsync(connection, message.OrderId);

            if (pendingReservation is null)
                // TODO: reply to caller with "fault" message.
                throw new Exception($"Could not find pending reservation for {nameof(message.OrderId)} {message.OrderId}");

            var pendindReservationIsValid = await IsPendingReservationValidAsync(connection, pendingReservation);

            if (!pendindReservationIsValid)
                // TODO: reply to caller with "fault" message.
                throw new Exception("Pending reservation is not valid");

            await CreateReservationAsync(connection, pendingReservation);

            await MarkPendingReservationAsConfirmed(connection, pendingReservation.Id);

            transaction.Commit();
        }

        private async Task<PendingReservation> FindPendingReservationAsync(
            IDbConnection connection,
            string orderId)
        {
            var query = @"
SELECT * FROM PendingReservations
WHERE OrderId = @OrderId;";

            var queryResult = await connection.ExecuteQueryAsync<PendingReservation>(query, new { orderId });

            return queryResult.ToArray().FirstOrDefault();
        }

        private async Task<bool> IsPendingReservationValidAsync(
            IDbConnection connection,
            PendingReservation pendingReservation)
        {
            var query = @"
SELECT * FROM RoomTypes
WHERE Id = @RoomTypeId AND Id NOT IN (
    SELECT DISTINCT RoomTypeId FROM Reservations
    WHERE Start >= @Start AND Start <= @End);";

            var queryResult = await connection.ExecuteQueryAsync<RoomType>(query,
                new { pendingReservation.RoomTypeId, pendingReservation.Start, pendingReservation.End });

            var availableRoomType = queryResult.FirstOrDefault();

            return !(availableRoomType is null);
        }

        private async Task MarkPendingReservationAsConfirmed(
            IDbConnection connection,
            string pendingReservationId)
        {
            var query = @"
UPDATE PendingReservations
SET Confirmed = True
WHERE PendingReservationId = @pendingReservationId;";

            await connection.ExecuteNonQueryAsync(query, new { pendingReservationId });
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

            await connection.InsertAsync(DatabaseTableNames.Reservations, reservation);

            return reservation;
        }
    }
}
