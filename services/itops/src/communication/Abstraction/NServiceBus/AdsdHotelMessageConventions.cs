using System;
using NServiceBus;

namespace artiso.AdsdHotel.ITOps.Communication.Abstraction.NServiceBus
{
    internal class AdsdHotelMessageConventions : IMessageConvention
    {
        public string Name => "Message conventions for adsd hotel";

        public bool IsCommandType(Type type) => IsACommand(type) && !IsGenericResponse(type);

        public bool IsEventType(Type type) => IsAnEvent(type);

        public bool IsMessageType(Type type) => IsAResponse(type) || IsGenericResponse(type);

        public static bool IsACommand(Type t) =>
            t.Namespace is not null &&
            t.Namespace.EndsWith(".Commands") &&
            !t.Name.EndsWith("Response");

        public static bool IsAResponse(Type t) =>
            t.Namespace is not null &&
            t.Namespace.EndsWith(".Commands") &&
            t.Name.EndsWith("Response");

        public static bool IsGenericResponse(Type t)
        {
            if (!t.IsGenericType) return false;
            var generic = t.GetGenericTypeDefinition();
            return generic == typeof(Response<>);
        }

        public static bool IsAnEvent(Type t) =>
            t.Namespace is not null &&
            t.Namespace.EndsWith(".Events");
    }
}
