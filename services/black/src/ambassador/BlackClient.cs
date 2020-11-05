using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Black.Commands.Validation;
using artiso.AdsdHotel.Black.Contracts;
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
    ///         <description>Call <see cref="StartAsync"/> to start the endpoint.</description>
    ///     </item>
    ///     <item>
    ///         <description>Implements <see cref="IAsyncDisposable"/> so you should use it with <b>await using</b> and don't forget <b>ConfigureAwait(false)</b>.</description>
    ///     </item>
    /// </list>
    /// Example usage:
    /// <code>
    /// var blackClient = new BlackClient ( "host=localhost" );<br/>
    /// await using ( blackClient.ConfigureAwait(false) )<br/>
    /// {<br/>
    ///     await blackClient.StartAsync();<br/>
    /// }
    /// </code>
    /// </remarks>
    public class BlackClient : IAsyncDisposable, IDisposable
    {
        private readonly EndpointConfiguration senderConfiguration;
        private bool disposedValue;
        private IEndpointInstance? senderEndpoint;

        /// <summary>
        /// Creates an object of type <see cref="BlackClient"/>.
        /// </summary>
        /// <param name="rabbitMqConnectionString">Connection string for a RabbitMQ instance.</param>
        public BlackClient(string rabbitMqConnectionString)
        {
            senderConfiguration = new EndpointConfiguration("Black.Ambassador");
            senderConfiguration
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
            this.senderEndpoint = await Endpoint.Start(senderConfiguration).ConfigureAwait(false);
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
            var sgi = new SetGuestInformation(orderId, guestInformation);
            if (!SetGuestInformationValidator.IsValid(sgi))
                throw new InvalidOperationException($"{typeof(GuestInformation).Name} is invalid.");

            await senderEndpoint.Send(sgi).ConfigureAwait(false);
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
            var response = await senderEndpoint.Request<GuestInformationResponse>(new GuestInformationRequest { OrderId = orderId }).ConfigureAwait(false);
            return response.GuestInformation;
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    senderEndpoint?.Stop().GetAwaiter().GetResult();
                }
                senderEndpoint = null;

                disposedValue = true;
            }
        }

        /// <summary>
        /// Diposes this instance.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void ThrowIfNotInitialized()
        {
            if (this.senderEndpoint == null)
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
            if (senderEndpoint != null)
            {
                await senderEndpoint.Stop().ConfigureAwait(false);
            }
            senderEndpoint = null;
        }
    }
}
