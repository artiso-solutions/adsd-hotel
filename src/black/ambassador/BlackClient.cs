using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using artiso.AdsdHotel.Black.Commands;
using NServiceBus;

namespace artiso.AdsdHotel.Black.Ambassador
{
    public class BlackClient
    {
        private EndpointConfiguration senderConfiguration;

        public BlackClient()
        {
            senderConfiguration = new EndpointConfiguration("Black.Ambassador");
            senderConfiguration.SendOnly();
            // ToDo Use mongo persistence etc.
            senderConfiguration.UsePersistence<InMemoryPersistence>();
            var senderTransport = senderConfiguration.UseTransport<RabbitMQTransport>();
            senderTransport.UseConventionalRoutingTopology();
            senderTransport.ConnectionString("host=localhost");
            // Use that to automatically route all SetGuestInformation to Black.Api instead of having to define it in the call.
            //var routing = senderTransport.Routing();
            //routing.RouteToEndpoint(typeof(SetGuestInformation), "Black.Api");
        }

        public async Task SetGuestInformationAsync(SetGuestInformation guestInformation)
        {
            // ToDo I don't know if we should start and stop the endpoint in this call or do it outside
            var senderEndpoint = await Endpoint.Start(senderConfiguration).ConfigureAwait(false);
            // ToDo maybe use routing in constructor hence we can omit the endpoint here
            await senderEndpoint.Send("Black.Api", guestInformation).ConfigureAwait(false);
            await senderEndpoint.Stop().ConfigureAwait(false);
        }
    }
}
