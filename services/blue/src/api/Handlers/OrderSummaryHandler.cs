using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Commands;
using artiso.AdsdHotel.Blue.Contracts;
using artiso.AdsdHotel.Blue.Validation;
using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.ITOps.Sql;
using NServiceBus;
using static artiso.AdsdHotel.Blue.Api.CommonQueries;

namespace artiso.AdsdHotel.Blue.Api.Handlers
{
    internal class OrderSummaryHandler : IHandleMessages<OrderSummaryRequest>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public OrderSummaryHandler(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task Handle(OrderSummaryRequest message, IMessageHandlerContext context)
        {
            try
            {
                Ensure.Valid(message);
            }
            catch (ValidationException validationEx)
            {
                await context.Reply(new Response<OrderSummaryResponse>(validationEx));
                return;
            }

            await using var connection = await _connectionFactory.CreateAsync();

            OrderSummary? summary = null;

            var reservation = await FindReservationAsync(connection, message.OrderId);

            if (reservation is not null)
            {
                summary = await CreateOrderSummaryFromReservationAsync(connection, reservation);
            }
            else
            {
                var pendingReservation = await FindPendingReservationAsync(connection, message.OrderId);

                if (pendingReservation is not null)
                {
                    summary = await CreateOrderSummaryFromPendingReservationAsync(connection, pendingReservation);
                }
            }

            await context.Reply(new Response<OrderSummaryResponse>(new OrderSummaryResponse(summary)));
        }

        private async Task<OrderSummary> CreateOrderSummaryFromReservationAsync(
            IDbConnection connection,
            Reservation reservation)
        {
            var roomType = await FindRoomTypeAsync(connection, reservation.RoomTypeId);

            var summary = new OrderSummary(
                reservation.OrderId,
                reservation.Start,
                reservation.End,
                roomType,
                Confirmed: true)
            {
                ConfirmedAt = reservation.CreatedAt
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
