using System;
using NServiceBus;

namespace artiso.AdsdHotel.Infrastructure.NServiceBus
{
    public class AdsdHotelMessageConventions : IMessageConvention
    {
        public string Name => "Message conventions for adsd hotel";

        public bool IsCommandType(Type type)
            => type.Namespace != null && type.Namespace.EndsWith(".Commands") && !type.Name.EndsWith("Response");

        public bool IsEventType(Type type)
            => type.Namespace != null && type.Namespace.EndsWith(".Events");

        public bool IsMessageType(Type type)
            => type.Namespace != null && type.Namespace.EndsWith(".Commands") && type.Name.EndsWith("Response");
    }
}
