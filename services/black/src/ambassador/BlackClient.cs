using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Infrastructure.NServiceBus;
using NServiceBus;

namespace artiso.AdsdHotel.Black.Ambassador
{
    public class BlackClient
    {
        private EndpointConfiguration senderConfiguration;

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
            routing.RouteToEndpoint(typeof(RequestGuestInformation), "Black.Api");
        }

        public async Task SetGuestInformationAsync(SetGuestInformation guestInformation)
        {
            // ToDo I don't know if we should start and stop the endpoint in this call or do it outside
            // maybe make the ambassador disposable?
            var senderEndpoint = await Endpoint.Start(senderConfiguration).ConfigureAwait(false);
            await senderEndpoint.Send(guestInformation).ConfigureAwait(false);
            await senderEndpoint.Stop().ConfigureAwait(false);
        }

        public async Task<GuestInformationResponse> GetGuestInformationAsync(Guid orderId)
        {
            var senderEndpoint = await Endpoint.Start(senderConfiguration).ConfigureAwait(false);
            var response = await senderEndpoint.Request<GuestInformationResponse>(new RequestGuestInformation { OrderId = orderId }).ConfigureAwait(false);
            await senderEndpoint.Stop().ConfigureAwait(false);
            return response;
        }
    }
}
