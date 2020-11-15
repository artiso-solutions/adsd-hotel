﻿using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Black.Contracts;
using artiso.AdsdHotel.Black.Contracts.Validation;
using artiso.AdsdHotel.Infrastructure.NServiceBus;
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
    /// }
    /// </code>
    /// </remarks>
    public class BlackClient : IAsyncDisposable, IDisposable
    {
        private readonly EndpointConfiguration _senderConfiguration;
        private bool _disposedValue;
        private IEndpointInstance? _senderEndpoint;

        /// <summary>
        /// Creates an object of type <see cref="BlackClient"/>.
        /// </summary>
        /// <param name="rabbitMqConnectionString">Connection string for a RabbitMQ instance.</param>
        public BlackClient(string rabbitMqConnectionString)
        {
            _senderConfiguration = new ("Black.Ambassador");
            _senderConfiguration
                .ConfigureDefaults(
                    rabbitMqConnectionString,
                "Black.Api",
                    typeof(SetGuestInformation), typeof(GuestInformationRequest))
                .WithClientCallbacks($"Black.Ambassador.{Guid.NewGuid()}");
        }

        /// <summary>
        /// Starts the NServiceBus endpoint to send commands and make requests.
        /// </summary>
        /// <returns>A task that can be awaited.</returns>
        public async Task StartAsync()
        {
            _senderEndpoint = await Endpoint.Start(_senderConfiguration).ConfigureAwait(false);
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
            ThrowIfNotInitialized();
            if (!GuestInformationValidator.IsValid(guestInformation))
                throw new InvalidOperationException($"{typeof(GuestInformation).Name} is invalid.");

            SetGuestInformation sgi = new (orderId, guestInformation);
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
            ThrowIfNotInitialized();
            var response = await _senderEndpoint.Request<GuestInformationResponse>(new GuestInformationRequest(orderId)).ConfigureAwait(false);
            return response.GuestInformation;
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
                }
                _senderEndpoint = null;

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

        private void ThrowIfNotInitialized()
        {
            if (_senderEndpoint == null)
            {
                throw new InvalidOperationException($"Client not initialized. Call {nameof(StartAsync)} first.");
            }
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
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes this instance asynchronously. Can be overwritten in derived classes.
        /// </summary>
        /// <returns>A ValueTask that can be awaited.</returns>
        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_senderEndpoint != null)
            {
                await _senderEndpoint.Stop().ConfigureAwait(false);
            }
            _senderEndpoint = null;
        }
    }
}
