using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Black.Commands.Validation;
using artiso.AdsdHotel.Black.Contracts;
using artiso.AdsdHotel.Infrastructure.NServiceBus;
using NServiceBus;

namespace artiso.AdsdHotel.Black.Ambassador
{
    public class BlackClient : IDisposable
    {
        private EndpointConfiguration senderConfiguration;
        private bool disposedValue;
        private IEndpointInstance? senderEndpoint;

        public BlackClient(string rabbitMqConnectionString)
        {
            senderConfiguration = new EndpointConfiguration("Black.Ambassador");
            senderConfiguration
                .ConfigureDefaults(
                    rabbitMqConnectionString, 
                "Black.Api", 
                    typeof(SetGuestInformation), typeof(GuestInformationRequest))
                .WithCallbacks($"Black.Ambassador.{Guid.NewGuid()}");
        }

        public async Task StartAsync()
        {
            this.senderEndpoint = await Endpoint.Start(senderConfiguration).ConfigureAwait(false);
        }

        public async Task SetGuestInformationAsync(Guid orderId, GuestInformation guestInformation)
        {
            ThrowIfNotInitialized();
            var sgi = new SetGuestInformation(orderId, guestInformation);
            if (!SetGuestInformationValidator.IsValid(sgi))
                throw new InvalidOperationException($"{typeof(GuestInformation).Name} is invalid.");

            await senderEndpoint.Send(sgi).ConfigureAwait(false);
        }

        public async Task<GuestInformation?> GetGuestInformationAsync(Guid orderId)
        {
            ThrowIfNotInitialized();
            var response = await senderEndpoint.Request<GuestInformationResponse>(new GuestInformationRequest { OrderId = orderId }).ConfigureAwait(false);
            return response.GuestInformation;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // ToDo maybe use NetStandard2.1 (no .NetFramework support!) and IAsyncDisposable
                    senderEndpoint?.Stop().GetAwaiter().GetResult();
                    senderEndpoint = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void ThrowIfNotInitialized()
        {
            if ( this.senderEndpoint == null)
            {
                throw new InvalidOperationException($"Client not initialized. Call {nameof(StartAsync)} first.");
            }
        }
    }
}
