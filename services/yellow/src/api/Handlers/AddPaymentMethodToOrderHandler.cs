using System.Threading.Tasks;
using artiso.AdsdHotel.Yellow.Api.Handlers.Templates;
using artiso.AdsdHotel.Yellow.Api.Services;
using artiso.AdsdHotel.Yellow.Api.Validation;
using artiso.AdsdHotel.Yellow.Contracts;
using artiso.AdsdHotel.Yellow.Contracts.Commands;
using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Api.Handlers
{
    public class AddPaymentMethodToOrderHandler : AbstractPaymentHandler<AddPaymentMethodToOrderRequest, PaymentMethodToOrderAdded>
    {
        private readonly IOrderService _orderService;
        private readonly ICreditCardPaymentService _paymentService;

        public AddPaymentMethodToOrderHandler(IOrderService orderService, ICreditCardPaymentService paymentService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
        }

        protected async override Task<PaymentMethodToOrderAdded> Handle(AddPaymentMethodToOrderRequest message)
        {
            Order order = await Ensure(message, m => _orderService.FindOneById(m.OrderId));

            var paymentToken = await _paymentService.GetPaymentToken(message.PaymentMethod.CreditCard!);
            var orderPaymentMethod = new StoredPaymentMethod(message.PaymentMethod.CreditCard.GetOrderCreditCard(paymentToken));
            
            await _orderService.AddPaymentMethod(order, orderPaymentMethod);

            return new PaymentMethodToOrderAdded();
        }

        protected override Task AddPaymentMethod(Order order, StoredPaymentMethod paymentMethod)
        {
            return _orderService.AddPaymentMethod(order, paymentMethod);
        }

        protected override ValidationModelResult<AddPaymentMethodToOrderRequest> ValidateRequest(
            AddPaymentMethodToOrderRequest message)
        {
            return message.Validate()
                .HasData(r => r.OrderId, 
                    $"{nameof(AddPaymentMethodToOrderRequest.OrderId)} should contain data")
                .NotNull(r => r.PaymentMethod, 
                    $"{nameof(AddPaymentMethodToOrderRequest.PaymentMethod)} should not be null")
                .NotNull(r => r.PaymentMethod.CreditCard, 
                    $"{nameof(AddPaymentMethodToOrderRequest.PaymentMethod.CreditCard)} should not be null")
                .PaymentMethodIsValid(r => r.PaymentMethod);
        }
    }
}
