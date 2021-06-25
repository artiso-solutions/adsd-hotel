using System.Threading;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.ITOps.Communication.Abstraction;
using artiso.AdsdHotel.Yellow.Contracts.Commands;
using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Ambassador
{
    public class YellowServiceClient
    {
        private readonly IChannel _channel;

        public YellowServiceClient(IChannel channel)
        {
            _channel = channel;
        }

        public async Task AddPaymentMethodToOrderRequest(string orderId, CreditCard creditCard, 
            CancellationToken cancellationToken = default)
        {
            var request = new AddPaymentMethodToOrderRequest(orderId, new PaymentMethod(creditCard));
            var (_, exception) =
                await _channel.Request<Response<bool>>(request, cancellationToken);

            if (exception is not null)
                throw exception;
        }
        
        public async Task AuthorizeOrderCancellationFee(string orderId,
            CancellationToken cancellationToken = default)
        {
            var request = new AuthorizeOrderCancellationFeeRequest(orderId);
            var (_, exception) =
                await _channel.Request<Response<bool>>(request, cancellationToken);

            if (exception is not null)
                throw exception;
        }

        public async Task ChargeCancellationFee(string orderId,
            CancellationToken cancellationToken = default)
        {
            var request = new ChargeForOrderCancellationFeeRequest(orderId);
            
            var (_, exception) =
                await _channel.Request<Response<bool>>(request, cancellationToken);
            
            if (exception is not null)
                throw exception;
        }
        
        public async Task ChargeFullAmount(string orderId,
            CancellationToken cancellationToken = default)
        {
            var request = new ChargeForOrderFullAmountRequest(orderId);
            
            var (_, exception) =
                await _channel.Request<Response<bool>>(request, cancellationToken);
            
            if (exception is not null)
                throw exception;
        }
    }
}
