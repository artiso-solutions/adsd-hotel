using System;
using System.Data;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Contracts;
using artiso.AdsdHotel.Blue.Validation;
using artiso.AdsdHotel.ITOps.Sql;
using Microsoft.AspNetCore.Mvc;
using static artiso.AdsdHotel.Blue.Api.CommonQueries;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace artiso.AdsdHotel.Blue.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class OrderSummaryController : ControllerBase
    {
        [HttpGet]
        [Route("order/{orderId}/summary")]
        [ProducesResponseType(typeof(OrderSummary), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [ProducesResponseType(Status400BadRequest)]
        public async Task<ActionResult<OrderSummary>> GetOrderSummary(
            [FromServices] IDbConnectionFactory connectionFactory,
            [FromRoute] string orderId)
        {
            Ensure.Valid(orderId, nameof(orderId));

            OrderSummary? summary = null;

            await using var connection = await connectionFactory.CreateAsync();

            var (pendingReservation, reservation) = await FindReservationsAsync(connection, orderId);

            if (reservation is not null)
            {
                summary = await CreateOrderSummaryFromReservationAsync(connection, reservation, pendingReservation);
            }
            else if (pendingReservation is not null)
            {
                summary = await CreateOrderSummaryFromPendingReservationAsync(connection, pendingReservation);
            }

            if (summary is null)
                return NotFound($"No reservation found for order id: {orderId}");

            return Ok(summary);
        }

        private async Task<(PendingReservation? pendingReservation, Reservation? reservation)> FindReservationsAsync(
            IDbConnection connection,
            string orderId)
        {
            var reservationTask = FindReservationAsync(connection, orderId);
            var pendingReservationTask = FindPendingReservationAsync(connection, orderId);

            await Task.WhenAll(reservationTask, pendingReservationTask);

            var reservation = await reservationTask;
            var pendingReservation = await pendingReservationTask;

            return (pendingReservation, reservation);
        }

        private async Task<OrderSummary> CreateOrderSummaryFromReservationAsync(
           IDbConnection connection,
           Reservation reservation,
           PendingReservation? pendingReservation)
        {
            var roomType = await FindRoomTypeAsync(connection, reservation.RoomTypeId);

            var summary = new OrderSummary(
                reservation.OrderId,
                reservation.Start,
                reservation.End,
                roomType,
                Confirmed: true)
            {
                RequestedAt = pendingReservation?.CreatedAt,
                ConfirmedAt = reservation.CreatedAt,
            };

            return summary;
        }

        private async Task<OrderSummary> CreateOrderSummaryFromPendingReservationAsync(
            IDbConnection connection,
            PendingReservation pendingReservation)
        {
            var roomType = await FindRoomTypeAsync(connection, pendingReservation.RoomTypeId);

            var summary = new OrderSummary(
                pendingReservation.OrderId,
                pendingReservation.Start,
                pendingReservation.End,
                roomType,
                Confirmed: false)
            {
                RequestedAt = pendingReservation.CreatedAt
            };

            return summary;
        }
    }
}
