using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;

namespace artiso.AdsdHotel.Blue.Ambassador
{
    public class BlueAmbassadorFactory
    {
        public static BlueAmbassador Create()
        {
            var channel = NServiceBusChannelFactory.Create(
                channelName: "Blue.Ambassador",
                endpointDestination: "Blue.Api",
                "host=localhost");

            var ambassador = new BlueAmbassador(channel);
            return ambassador;
        }
    }
}
