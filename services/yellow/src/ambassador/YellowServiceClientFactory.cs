using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;

namespace artiso.AdsdHotel.Yellow.Ambassador
{
    public class YellowServiceClientFactory
    {
        public static YellowServiceClient Create()
        {
            var channel = NServiceBusChannelFactory.Create(
                channelName: "Yellow.Ambassador",
                endpointDestination: "Yellow.Api",
                "host=localhost;username=user;password=bitnami", 
                useCallbacks: true
            );

            var ambassador = new YellowServiceClient(channel);
            return ambassador;
        }
    }
}
