using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus;

namespace artiso.AdsdHotel.Purple.Ambassador
{
    public class PurpleAmbassadorFactory
    {
        public static PurpleAmbassador Create()
        {
            var channel = NServiceBusChannelFactory.Create(
                channelName: "Purple.Ambassador",
                endpointDestination: "Purple.Api",
                "host=localhost",
                useCallbacks: true
                );

            var ambassador = new PurpleAmbassador(channel);
            return ambassador;
        }
    }
}
