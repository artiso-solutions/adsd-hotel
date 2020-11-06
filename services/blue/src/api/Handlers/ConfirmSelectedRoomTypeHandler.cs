using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Commands;
using artiso.AdsdHotel.Blue.Contracts;
using MySql.Data.MySqlClient;
using NServiceBus;
using RepoDb;

namespace artiso.AdsdHotel.Blue.Api.Handlers
{
    class ConfirmSelectedRoomTypeHandler : IHandleMessages<ConfirmSelectedRoomType>
    {
        private MySqlConnection? _connection;
        private IDbTransaction? _transaction;

        public async Task Handle(ConfirmSelectedRoomType message, IMessageHandlerContext context)
        {
            _connection = new MySqlConnection("Server=localhost;Port=13306;Database=adsd-blue;");
            _transaction = _connection.EnsureOpen().BeginTransaction();

            using var connection = _connection;
            using var transaction = _transaction;

            var pendingReservation = await FindPendingReservationAsync(message.OrderId);

            if (pendingReservation is null)
                // TODO: reply to caller with "fault" message.
                throw new Exception($"Could not find pending reservation for {nameof(message.OrderId)} {message.OrderId}");

            var valid = await IsPendingReservationValidAsync(pendingReservation);

            if (!valid)
                // TODO: reply to caller with "fault" message.
                throw new Exception("Pending reservation is not valid");

            await CreateReservationAsync(pendingReservation);

            await ConfirmPendingReservationAsync(pendingReservation.Id);

            transaction.Commit();
        }

        private async Task<PendingReservation> FindPendingReservationAsync(string orderId)
        {
            var query = @"
SELECT * FROM PendingReservations
WHERE OrderId = @OrderId;
";

            var queryResult = await _connection.ExecuteQueryAsync<PendingReservation>(query, new { orderId });

            return queryResult.ToArray().FirstOrDefault();
        }

        private async Task<bool> IsPendingReservationValidAsync(PendingReservation pendingReservation)
        {
            var query = @"
SELECT * FROM RoomTypes
WHERE Id = @RoomTypeId AND Id NOT IN (
    SELECT DISTINCT RoomTypeId FROM Reservations
    WHERE Start >= @Start AND Start <= @End
);";

            var queryResult = await _connection.ExecuteQueryAsync<RoomType>(query,
                new { pendingReservation.RoomTypeId, pendingReservation.Start, pendingReservation.End });

            var availableRoomType = queryResult.ToArray().FirstOrDefault();

            return availableRoomType != null;
        }

        private async Task ConfirmPendingReservationAsync(string pendingReservationId)
        {
            var query = @"
UPDATE PendingReservations
SET Confirmed = True
WHERE PendingReservationId = @PendingReservationId;
";

            await _connection.ExecuteNonQueryAsync(query, new { PendingReservationId = pendingReservationId }, transaction: _transaction);
        }

        private async Task<Reservation> CreateReservationAsync(PendingReservation pendingReservation)
        {
            var reservationId = Guid.NewGuid().ToString();

            var reservation = new Reservation(
                reservationId,
                pendingReservation.OrderId,
                pendingReservation.RoomTypeId,
                pendingReservation.Start,
                pendingReservation.End,
                DateTime.UtcNow);

            await _connection.InsertAsync(DatabaseTableNames.Reservations, reservation, transaction: _transaction);

            return reservation;
        }
    }
}
