using NServiceBus;

namespace artiso.AdsdHotel.Blue.Ambassador
{
    public class NServiceBusEndpointConfigurator
    {
        public static void Configure(EndpointConfiguration config)
        {
            config.EnableCallbacks();
            config.EnableInstallers();

            config.UseSerialization<NewtonsoftSerializer>();
            config.UsePersistence<InMemoryPersistence>();

            config.Conventions()
                .DefiningCommandsAs(t =>
                    t.Namespace != null &&
                    t.Namespace.EndsWith(".Commands") &&
                    !t.Name.EndsWith("Response"))
                .DefiningMessagesAs(t =>
                    t.Namespace != null &&
                    t.Namespace.EndsWith(".Commands") &&
                    t.Name.EndsWith("Response"))
                .DefiningEventsAs(t =>
                    t.Namespace != null &&
                    t.Namespace.EndsWith(".Events"));
        }
    }
}
