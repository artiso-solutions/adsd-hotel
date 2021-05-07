using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;

namespace artiso.AdsdHotel.Blue.Ambassador
{
    public class BlueAmbassadorFactory
    {
        public static BlueAmbassador Create(string? rabbitMqConnectionString = null)
        {
            var channel = NServiceBusChannelFactory.Create(
                channelName: "Blue.Ambassador",
                endpointDestination: "Blue.Api",
                rabbitMqConnectionString ?? "host=localhost", 
                useCallbacks: true
                );

            var ambassador = new BlueAmbassador(channel);
            return ambassador;
        }
    }
}
