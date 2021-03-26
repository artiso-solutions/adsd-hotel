using System.Threading.Tasks;
using artiso.AdsdHotel.Red.Events;
using NServiceBus;

namespace artiso.AdsdHotel.Red.Api.Handler
{
    public class OrderRateSelectedHandler : IHandleMessages<OrderRateSelected>
    {
        public Task Handle(OrderRateSelected orderRateSelected, IMessageHandlerContext context)
        {
            return context.Publish(orderRateSelected);
        }
    }
}
