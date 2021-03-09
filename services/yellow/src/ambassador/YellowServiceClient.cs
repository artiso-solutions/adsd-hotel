using System;
using System.Threading;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.Communication;
using artiso.AdsdHotel.ITOps.Communication.Abstraction;
using artiso.AdsdHotel.Yellow.Contracts.Commands;
using artiso.AdsdHotel.Yellow.Contracts.Models;
using artiso.AdsdHotel.Yellow.Events;

namespace artiso.AdsdHotel.Yellow.Ambassador
{
    public class YellowServiceClient
    {
        private readonly IChannel _channel;

        public YellowServiceClient(IChannel channel)
        {
            _channel = channel;
        }

        public async Task AuthorizeOrderCancellationFee(string orderId,
            CreditCard creditCard,
            CancellationToken cancellationToken = default)
        {
            var request = new AuthorizeOrderCancellationFeeRequest(orderId, new PaymentMethod(creditCard));
            var (response, exception) =
                await _channel.Request<Response<OrderCancellationFeeAuthorizationAcquired>>(request, cancellationToken);

            if (exception is not null)
                throw exception;

            if (response is null)
                throw new Exception("Service not available");
        }

        public async Task ChargeCancellationFee(string orderId,
            CancellationToken cancellationToken = default)
        {
            var request = new ChargeForOrderCancellationFeeRequest(orderId);
            
            var (response, exception) =
                await _channel.Request<Response<OrderCancellationFeeCharged>>(request, cancellationToken);
        }
        
        public async Task ChargeFullAmount(string orderId,
            CancellationToken cancellationToken = default)
        {
            var request = new ChargeForOrderFullAmountRequest(orderId);
            
            var (response, exception) =
                await _channel.Request<Response<OrderFullAmountCharged>>(request, cancellationToken);
        }
    }
}
