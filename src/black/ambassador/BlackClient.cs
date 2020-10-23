using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Commands;
using artiso.AdsdHotel.Black.Contracts;
using NServiceBus;

namespace artiso.AdsdHotel.Black.Ambassador
{
    public class BlackClient
    {
        private EndpointConfiguration senderConfiguration;

        public BlackClient(string connectionString)
        {
            senderConfiguration = new EndpointConfiguration("Black.Ambassador");
            //senderConfiguration.SendOnly();
            // ToDo I don't like this callback stuff with nservicebus, we could just use a webapi
            senderConfiguration.EnableCallbacks();
            senderConfiguration.EnableInstallers();
            senderConfiguration.MakeInstanceUniquelyAddressable($"Black.Ambassador.{Guid.NewGuid()}");
            senderConfiguration.UseSerialization<NewtonsoftSerializer>();

            var conventions = senderConfiguration.Conventions();

            conventions.DefiningCommandsAs(t =>
                t.Namespace != null &&
                t.Namespace.EndsWith(".Commands") &&
                !t.Name.EndsWith("Response"));

            conventions.DefiningMessagesAs(t =>
                t.Namespace != null &&
                t.Namespace.EndsWith(".Commands") &&
                t.Name.EndsWith("Response"));

            conventions.DefiningEventsAs(t =>
                t.Namespace != null &&
                t.Namespace.EndsWith(".Events"));

            // ToDo Use mongo persistence etc.
            senderConfiguration.UsePersistence<InMemoryPersistence>();
            var senderTransport = senderConfiguration.UseTransport<RabbitMQTransport>();
            senderTransport.UseConventionalRoutingTopology();
            senderTransport.ConnectionString(connectionString);
            // Use that to automatically route all SetGuestInformation to Black.Api instead of having to define it in the call.
            var routing = senderTransport.Routing();
            routing.RouteToEndpoint(typeof(SetGuestInformation), "Black.Api");
            routing.RouteToEndpoint(typeof(RequestGuestInformation), "Black.Api");
        }

        public async Task SetGuestInformationAsync(SetGuestInformation guestInformation)
        {
            // ToDo I don't know if we should start and stop the endpoint in this call or do it outside
            var senderEndpoint = await Endpoint.Start(senderConfiguration).ConfigureAwait(false);
            //var response = senderEndpoint.Request<GuestInformationSet>(guestInformation).ConfigureAwait(false);
            // ToDo maybe use routing in constructor hence we can omit the endpoint here
            await senderEndpoint.Send(guestInformation).ConfigureAwait(false);
            await senderEndpoint.Stop().ConfigureAwait(false);
        }

        public async Task<GuestInformationResponse> GetGuestInformationAsync(Guid orderId)
        {
            var senderEndpoint = await Endpoint.Start(senderConfiguration).ConfigureAwait(false);
            var response = await senderEndpoint.Request<GuestInformationResponse>(new RequestGuestInformation()).ConfigureAwait(false);
            await senderEndpoint.Stop().ConfigureAwait(false);
            return response;
        }
    }
}
