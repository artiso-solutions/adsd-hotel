using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.NoSql;
using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Api.Services
{
    public class CreditCardPaymentService : ICreditCardPaymentService
    {
        private readonly IDataStoreClient _dataStoreClient;
        private readonly bool _failAllPayments;
        
        public CreditCardPaymentService(IDataStoreClient dataStoreClient, bool failAllPayments)
        {
            _dataStoreClient = dataStoreClient;
            _failAllPayments = failAllPayments;
        }
        
        public CreditCardPaymentService(IDataStoreClient dataStoreClient)
        {
            _dataStoreClient = dataStoreClient;
        }
        
        /// <inheritdoc/>
        public async Task<AuthorizeResult> Authorize(decimal amount, CreditCard creditCard)
        {
            if (!CreditCardHasTheRequiredAmount(amount, creditCard))
                return new AuthorizeResult(null, new InvalidOperationException("The given credit card has not the required amount"));

            var paymentAuthorizationToken = await GetPaymentAuthorizationToken();

            return new AuthorizeResult(paymentAuthorizationToken.Token, null);
        }

        /// <inheritdoc/>
        public async Task<ChargeResult> Charge(decimal amount, CreditCard creditCard)
        {
            if (!CreditCardHasTheRequiredAmount(amount, creditCard))
            {
                return new ChargeResult()
                {
                    Exception = new InvalidOperationException("The given credit card has not the required amount")
                };
            }
            
            var paymentAuthorizationToken = await GetPaymentAuthorizationToken();

            return await Charge(amount, paymentAuthorizationToken.Token!);
        }

        /// <inheritdoc/>
        public async Task<ChargeResult> Charge(decimal amount, string authorizePaymentToken)
        {
            var (isValid, chargeResult) = await ValidateToken(authorizePaymentToken);

            if (!isValid)
                return chargeResult;

            try
            {
                var paymentCode = await Pay(amount, authorizePaymentToken);
                
                var transaction = new Transaction(Guid.NewGuid().ToString(), paymentCode, amount, DateTime.Now);

                await _dataStoreClient.InsertOneAsync(transaction);

                return new ChargeResult
                {
                    transaction = transaction,
                    AuthorizePaymentToken = authorizePaymentToken
                };
            }
            catch (Exception e)
            {
                return new ChargeResult
                {
                    Exception = e
                };
            }
        }

        private async Task<(bool isValid, ChargeResult chargeResult)> ValidateToken(string authorizePaymentToken)
        {
            var t = await _dataStoreClient
                .GetAsync<PaymentAuthorizationToken>(ExpressionCombinationOperator.And,
                    token => token.Token == authorizePaymentToken);
            
            if (t is null)
            {
                return (false, new ChargeResult
                {
                    Exception = new InvalidOperationException($"{nameof(PaymentAuthorizationToken)} not existing")
                });
            }

            if (!t.Active)
            {
                return (false, new ChargeResult
                {
                    Exception = new InvalidOperationException($"{nameof(PaymentAuthorizationToken)} is not active")
                });
            }
            
            if (t.ExpirationDate.HasValue && t.ExpirationDate.Value <= DateTime.Now)
            {
                return (false, new ChargeResult
                {
                    Exception = new InvalidOperationException($"{nameof(PaymentAuthorizationToken)} is expired")
                });
            }

            return (true, null!);
        }

        private async Task<PaymentAuthorizationToken> GetPaymentAuthorizationToken()
        {
            var paymentAuthorizationToken = new PaymentAuthorizationToken(TimeSpan.FromDays(30));
            await _dataStoreClient.InsertOneAsync(paymentAuthorizationToken);
            return paymentAuthorizationToken;
        }
        
        private bool CreditCardHasTheRequiredAmount(decimal amount, CreditCard creditCard)
        {
            // args not used in this mock implementation:
            _ = amount;
            _ = creditCard;
            
            return !MustPaymentFail();
        }
        
        private async Task<string> Pay(decimal amount, string authToken)
        {
            // args not used in this mock implementation:
            _ = amount;
            _ = authToken;
            
            await Task.Delay(1000);
            
            if (MustPaymentFail())
                throw new InvalidOperationException("Payment fails. Method has not the required amount");
            
            var paymentCode = Guid.NewGuid().ToString();

            return paymentCode;
        }

        private bool MustPaymentFail()
        {
            return _failAllPayments;
        }
    }
}
