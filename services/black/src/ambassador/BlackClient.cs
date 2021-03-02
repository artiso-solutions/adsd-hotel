using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Black.Contracts;
using artiso.AdsdHotel.Black.Contracts.Validation;
using Flurl;
using NServiceBus;

namespace artiso.AdsdHotel.Black.Ambassador
{
    /// <summary>
    /// Client for communicating with the black service.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///     <item>
    ///        Call <see cref="StartAsync"/> to start the endpoint.
    ///     </item>
    ///     <item>
    ///         Implements <see cref="IAsyncDisposable"/> so you should use it with <b>await using</b> or call <b>DisposeAsync</b> explicitly.
    ///     </item>
    /// </list>
    /// Example usage:
    /// <code>
    /// await using ( var blackClient = new BlackClient ( "host=localhost" ) )<br/>
    /// {<br/>
    ///     await blackClient.StartAsync();<br/>
    /// 
    /// }
    /// </code>
    /// </remarks>
    public class BlackClient : IAsyncDisposable, IDisposable
    {
        private bool _disposedValue;
        private IEndpointInstance? _senderEndpoint;
        private HttpClient? _httpClient;

        /// <summary>
        /// Creates an object of type <see cref="BlackClient"/>.
        /// </summary>
        /// <param name="rabbitMqConnectionString">Connection string for a RabbitMQ instance.</param>
        /// <param name="httpClientFactory">HttpClientFactory to provide a HttpClient in StartAsync calls.</param>
        internal BlackClient(IEndpointInstance endpoint, HttpClient httpClient)
        {
            _senderEndpoint = endpoint;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Sets the <see cref="GuestInformation"/> for an order.
        /// </summary>
        /// <param name="orderId">The identifier of the order.</param>
        /// <param name="guestInformation">The guest information to set.</param>
        /// <returns>A task that can be awaited.</returns>
        /// <exception cref="InvalidOperationException"/>
        public async Task SetGuestInformationAsync(Guid orderId, GuestInformation guestInformation)
        {
            if (!GuestInformationValidator.IsValid(guestInformation))
                throw new InvalidOperationException($"{typeof(GuestInformation).Name} is invalid.");

            SetGuestInformation sgi = new(orderId, guestInformation);
            await _senderEndpoint.Send(sgi).ConfigureAwait(false);
        }

        /// <summary>
        /// Tries to get the <see cref="GuestInformation"/> for an order.
        /// </summary>
        /// <param name="orderId">The identifier of the order.</param>
        /// <returns>The guest information if the order was found, <c>null</c> otherwise.</returns>
        /// <exception cref="InvalidOperationException"/>
        public async Task<GuestInformation?> GetGuestInformationAsync(Guid orderId)
        {
            var query = _httpClient!.BaseAddress
                .AppendPathSegment("guestInformation")
                .SetQueryParams(new { orderId }, NullValueHandling.Ignore);
            var response = await _httpClient!.GetAsync(query).ConfigureAwait(false);
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
        public async Task<List<Guid>?> GetOrdersAsync(string? firstName, string? lastName, string? eMail)
        {
            var query = _httpClient!.BaseAddress
                .AppendPathSegment("order")
                .SetQueryParams(new { firstName, lastName, eMail }, NullValueHandling.Ignore);
            var response = await _httpClient!.GetAsync(query).ConfigureAwait(false);
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
                    _senderEndpoint?.Stop().GetAwaiter().GetResult();
                    _httpClient?.Dispose();
                }
                _senderEndpoint = null;
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
            if (_senderEndpoint is not null)
            {
                await _senderEndpoint.Stop().ConfigureAwait(false);
            }
            _senderEndpoint = null;
            _httpClient?.Dispose();
            _httpClient = null;
        }
    }
}
