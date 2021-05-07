using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;

namespace artiso.AdsdHotel.Yellow.Ambassador
{
    public class YellowServiceClientFactory
    {
        public static YellowServiceClient Create(string? rabbitMqConnectionString = null)
        {
            var channel = NServiceBusChannelFactory.Create(
                channelName: "Yellow.Ambassador",
                endpointDestination: "Yellow.Api",
                rabbitMqConnectionString ?? "host=localhost", 
                useCallbacks: true
            );

            var ambassador = new YellowServiceClient(channel);
            return ambassador;
        }
    }
}
