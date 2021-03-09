using System.Threading.Tasks;
using artiso.AdsdHotel.Yellow.Api.Handlers.Templates;
using artiso.AdsdHotel.Yellow.Api.Services;
using artiso.AdsdHotel.Yellow.Api.Validation;
using artiso.AdsdHotel.Yellow.Events;
using artiso.AdsdHotel.Yellow.Events.External;
using Price = artiso.AdsdHotel.Yellow.Contracts.Models.Price;

namespace artiso.AdsdHotel.Yellow.Api.Handlers
{
    public class OrderRatesSelectedHandler : AbstractHandler<OrderRateSelected, OrderRateSelectedCreated>
    {
        private readonly IOrderService _orderService;

        public OrderRatesSelectedHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        protected async override Task<OrderRateSelectedCreated> Handle(OrderRateSelected message)
        {
            await _orderService.Create(message.OrderId, new Price(message.Price.CancellationFee, message.Price.Amount));

            return new OrderRateSelectedCreated(message.OrderId);
        }

        protected override ValidationModelResult<OrderRateSelected> ValidateRequest(OrderRateSelected message)
        {
            return message.Validate()
                .HasData(r => r.OrderId, 
                    $"{nameof(OrderRateSelected.OrderId)} should contain data")
                .NotNull(r => r.Price, 
                    $"{nameof(OrderRateSelected.Price)} should not be null");
            
        }
    }
}
