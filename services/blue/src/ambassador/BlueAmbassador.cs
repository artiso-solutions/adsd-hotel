using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using artiso.AdsdHotel.Blue.Api.Models;
using artiso.AdsdHotel.Blue.Contracts;
using artiso.AdsdHotel.Blue.Validation;
using static System.Net.HttpStatusCode;

namespace artiso.AdsdHotel.Blue.Ambassador
{
    public class BlueAmbassador
    {
        private readonly Uri _apiBaseAddress;
        private readonly HttpClient _httpClient;

        internal BlueAmbassador(
            string apiBaseAddress,
            HttpClient httpClient)
        {
            _apiBaseAddress = new Uri(apiBaseAddress);
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<RoomType>> ListRoomTypesAvailableBetweenAsync(
            DateTime start,
            DateTime end,
            CancellationToken cancellationToken = default)
        {
            Ensure.Valid(start, end);

            var request = new AvailableRoomTypesRequest(start, end);

            var response = await _httpClient
                .PostAsJsonAsync(Uri("api/availability/search"), request, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw await CreateErrorFromAsync(response).ConfigureAwait(false);

            var availableRoomTypes = await response.Content
                .ReadFromJsonAsync<IReadOnlyList<RoomType>>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return availableRoomTypes ?? Array.Empty<RoomType>();
        }

        public async Task SelectRoomTypeBetweenAsync(
            string orderId,
            string roomTypeId,
            DateTime start,
            DateTime end,
            CancellationToken cancellationToken = default)
        {
            Ensure.Valid(orderId, nameof(orderId));
            Ensure.Valid(roomTypeId, nameof(roomTypeId));
            Ensure.Valid(start, end);

            var request = new SelectRoomType(roomTypeId, start, end);

            var response = await _httpClient
                .PostAsJsonAsync(Uri($"api/order/{orderId}/room-type/select"), request, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw await CreateErrorFromAsync(response).ConfigureAwait(false);
        }

        public async Task<string?> GetReservationRoomNumberAsync(
            string orderId,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient
                .GetAsync(Uri($"api/order/{orderId}/room-number"), cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == UnprocessableEntity)
                    return null;
                else
                    throw await CreateErrorFromAsync(response).ConfigureAwait(false);
            }

            var reply = await response.Content
                .ReadFromJsonAsync<GetRoomNumberReply>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return reply?.RoomNumber;
        }

        public async Task<OrderSummary> GetOrderSummaryAsync(
            string orderId,
            CancellationToken cancellationToken = default)
        {
            Ensure.Valid(orderId, nameof(orderId));

            var response = await _httpClient
                .GetAsync(Uri($"api/order/{orderId}/summary"), cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw await CreateErrorFromAsync(response).ConfigureAwait(false);

            var summary = await response.Content
                .ReadFromJsonAsync<OrderSummary>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return summary!;
        }

        private Uri Uri(string apiPath) => new(_apiBaseAddress, apiPath);

        private async Task<ServiceUnavailableException> CreateErrorFromAsync(HttpResponseMessage response)
        {
            string? content = null;
            try { content = await response.Content.ReadAsStringAsync().ConfigureAwait(false); } catch { }
            return new ServiceUnavailableException(content ?? $"Code {response.StatusCode} does not indicate success");
        }
    }
}
