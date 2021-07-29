using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Commands;
using artiso.AdsdHotel.Blue.Contracts;
using artiso.AdsdHotel.Blue.Validation;
using artiso.AdsdHotel.ITOps.Communication.Abstraction;
using artiso.AdsdHotel.ITOps.Communication;

namespace artiso.AdsdHotel.Blue.Ambassador
{
    public class BlueAmbassador
    {
        private readonly IChannel _channel;

        internal BlueAmbassador(IChannel channel)
        {
            _channel = channel;
        }

        public async Task<IReadOnlyList<RoomType>> ListRoomTypesAvailableBetweenAsync(
            DateTime start,
            DateTime end,
            CancellationToken cancellationToken = default)
        {
            Ensure.Valid(start, end);

            var request = new AvailableRoomTypesRequest(start, end);
            var (response, exception) =
                await _channel.Request<Response<AvailableRoomTypesResponse>>(request, cancellationToken);

            if (exception is not null)
                throw exception;

            if (response is null)
                throw new ServiceUnavailableException();

            return response.RoomTypes;
        }

        public async Task SelectRoomTypeBetweenAsync(
            string orderId,
            string roomTypeId,
            DateTime start,
            DateTime end)
        {
            Ensure.Valid(orderId, nameof(orderId));
            Ensure.Valid(roomTypeId, nameof(roomTypeId));
            Ensure.Valid(start, end);

            var request = new SelectRoomType(orderId, roomTypeId, start, end);
            await _channel.Send(request);
        }

        public async Task ConfirmSelectedRoomTypeAsync(string orderId)
        {
            Ensure.Valid(orderId, nameof(orderId));

            var request = new ConfirmSelectedRoomType(orderId);
            await _channel.Send(request);
        }

        public async Task<string> GetReservationRoomNumberAsync(
            string orderId,
            CancellationToken cancellationToken = default)
        {
            Ensure.Valid(orderId, nameof(orderId));

            var request = new GetRoomNumberRequest(orderId);
            var (response, exception) =
                await _channel.Request<Response<GetRoomNumberResponse>>(request, cancellationToken);

            if (exception is not null)
                throw exception;

            if (response is null)
                throw new ServiceUnavailableException();

            return response.RoomNumber;
        }

        public async Task<OrderSummary?> GetOrderSummaryAsync(
            string orderId,
            CancellationToken cancellationToken = default)
        {
            Ensure.Valid(orderId, nameof(orderId));

            var request = new OrderSummaryRequest(orderId);
            var (response, exception) =
                await _channel.Request<Response<OrderSummaryResponse>>(request, cancellationToken);

            if (exception is not null)
                throw exception;

            if (response is null)
                throw new ServiceUnavailableException();

            return response.OrderSummary;
        }
    }
}
