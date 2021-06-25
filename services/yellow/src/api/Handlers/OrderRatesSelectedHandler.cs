using System.Threading.Tasks;
using artiso.AdsdHotel.Yellow.Api.Services;
using artiso.AdsdHotel.Yellow.Api.Validation;
using artiso.AdsdHotel.Yellow.Events.External;
using NServiceBus;
using Price = artiso.AdsdHotel.Yellow.Contracts.Models.Price;

namespace artiso.AdsdHotel.Yellow.Api.Handlers
{
    public class OrderRatesSelectedHandler : IHandleMessages<OrderRateSelected>
    {
        private readonly IOrderService _orderService;

        public OrderRatesSelectedHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task Handle(OrderRateSelected message, IMessageHandlerContext context)
        {
            var validateResult = ValidateRequest(message);

            if (!validateResult.IsValid())
                throw new ValidationException(validateResult);
            
            await _orderService.Create(message.OrderId, new Price(message.Price.CancellationFee, message.Price.Amount));
        }

        private static ValidationModelResult<OrderRateSelected> ValidateRequest(OrderRateSelected message)
        {
            return message.Validate()
                .HasData(r => r.OrderId,
                    $"{nameof(OrderRateSelected.OrderId)} should contain data")
                .NotNull(r => r.Price,
                    $"{nameof(OrderRateSelected.Price)} should not be null");

        }
    }
}
