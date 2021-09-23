using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Black.Contracts;
using artiso.AdsdHotel.ITOps.Validation;
using Flurl;

namespace artiso.AdsdHotel.Black.Ambassador
{
    /// <summary>
    /// Client for communicating with the black service.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///     <item>
    ///         Implements <see cref="IAsyncDisposable"/> and <see cref="IDisposable"/>, so keep in mind that the <see cref="HttpClient"/> gets disposed by this instance.
    ///     </item>
    /// </list>
    /// </remarks>
    public class BlackClient : IAsyncDisposable, IDisposable
    {
        private bool _disposedValue;
        private HttpClient? _httpClient;

        /// <summary>
        /// Creates an object of type <see cref="BlackClient"/>.
        /// </summary>
        /// <param name="httpClient">HttpClient to make API calls.</param>
        internal BlackClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Sets the <see cref="GuestInformation"/> for an order.
        /// </summary>
        /// <param name="orderId">The identifier of the order.</param>
        /// <param name="guestInformation">The guest information to set.</param>
        /// <returns>A task that can be awaited.</returns>
        /// <exception cref="ObjectDisposedException"/>
        /// <exception cref="MultiValidationException"/>
        public async Task<bool> SetGuestInformationAsync(Guid orderId, GuestInformation guestInformation)
        {
            if (_disposedValue)
                throw new ObjectDisposedException(nameof(BlackClient));
        
            Ensure.IsValid(guestInformation);
            SetGuestInformation sgi = new(orderId, guestInformation);
            var query = _httpClient!.BaseAddress
                .AppendPathSegment("guestInformation");
            var response = await _httpClient.PostAsJsonAsync(query, sgi);
            // ToDo Maybe return the errors
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Tries to get the <see cref="GuestInformation"/> for an order.
        /// </summary>
        /// <param name="orderId">The identifier of the order.</param>
        /// <returns>The guest information if the order was found, <c>null</c> otherwise.</returns>
        /// <exception cref="ObjectDisposedException"/>
        /// <exception cref="MultiValidationException"/>
        public async Task<GuestInformation?> GetGuestInformationAsync(Guid orderId)
        {
            if (_disposedValue)
                throw new ObjectDisposedException(nameof(BlackClient));

            var query = _httpClient!.BaseAddress
                .AppendPathSegment("guestInformation")
                .SetQueryParams(new { orderId }, NullValueHandling.Ignore);
            var response = await _httpClient.GetAsync(query).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var r = await response.Content.ReadFromJsonAsync<GuestInformationResponse>().ConfigureAwait(false);
                return r?.GuestInformation;
            }
            return null;
        }

        /// <summary>
        /// Searches for all orders where the guest information contains either the parameters.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="eMail"></param>
        /// <returns>All order ids where the guest information of the order contains any of the parametes.</returns>
        /// <exception cref="ObjectDisposedException"/>
        /// <exception cref="MultiValidationException"/>
        public async Task<IEnumerable<Guid>?> GetOrdersAsync(string? firstName, string? lastName, string? eMail)
        {
            if (_disposedValue)
                throw new ObjectDisposedException(nameof(BlackClient));

            var query = _httpClient!.BaseAddress
                .AppendPathSegment("order")
                .SetQueryParams(new { firstName, lastName, eMail }, NullValueHandling.Ignore);
            var response = await _httpClient.GetAsync(query).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var r = await response.Content.ReadFromJsonAsync<OrderIdRespone>().ConfigureAwait(false);
                return r?.OrderIds;
            }
            return null;
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _httpClient?.Dispose();
                }
                _httpClient = null;

                _disposedValue = true;
            }
        }

        /// <summary>
        /// Diposes this instance.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes this instance asynchronously.
        /// </summary>
        /// <returns>A ValueTask that can be awaited.</returns>
        public async ValueTask DisposeAsync()
        {
            // Perform async cleanup
            await DisposeAsyncCore();

            Dispose(false);
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        }

        /// <summary>
        /// Disposes this instance asynchronously. Can be overwritten in derived classes.
        /// </summary>
        /// <returns>A ValueTask that can be awaited.</returns>
        protected virtual async ValueTask DisposeAsyncCore()
        {
            _httpClient?.Dispose();
            _httpClient = null;
        }
    }
}
