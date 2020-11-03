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
            senderConfiguration.EnableCallbacks();
            senderConfiguration.EnableInstallers();
            senderConfiguration.MakeInstanceUniquelyAddressable($"Black.Ambassador.{Guid.NewGuid()}");
            senderConfiguration.UseSerialization<NewtonsoftSerializer>();

            senderConfiguration.ConfigureConventions();

            // ToDo Use mongo persistence etc.
            senderConfiguration.UsePersistence<InMemoryPersistence>();
            var senderTransport = senderConfiguration.UseTransport<RabbitMQTransport>();
            senderTransport.UseConventionalRoutingTopology();
            senderTransport.ConnectionString(rabbitMqConnectionString);
            var routing = senderTransport.Routing();
            routing.RouteToEndpoint(typeof(SetGuestInformation), "Black.Api");
            routing.RouteToEndpoint(typeof(GuestInformationRequest), "Black.Api");
            
        }

        public async Task Start()
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
                throw new InvalidOperationException($"Client not initialized. Call {nameof(Start)} first.");
            }
        }
    }
}
