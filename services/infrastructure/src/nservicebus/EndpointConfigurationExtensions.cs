using System;
using System.Runtime.CompilerServices;
using NServiceBus;

namespace artiso.AdsdHotel.Infrastructure.NServiceBus
{
    public static class NServiceBusEndpointConfigurationExtensions
    {
        public static ConventionsBuilder ConfigureConventions(this EndpointConfiguration config)
        {
            var conventions = config.Conventions();

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

            return conventions;
        }
    }
}
