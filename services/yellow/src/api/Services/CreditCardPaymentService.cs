using System;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.NoSql;
using artiso.AdsdHotel.Yellow.Api.Configuration;
using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Api.Services
{
    public class CreditCardPaymentService : ICreditCardPaymentService
    {
        private readonly bool _failAllPayments;
        private readonly IDataStoreClient _transactionClient;
        private readonly IDataStoreClient _paymentAuthorizationTokenDataClient;

        public CreditCardPaymentService(MongoDBClientFactory factory, bool failAllPayments)
        {
            _transactionClient = factory.GetClient(typeof(Transaction));
            _paymentAuthorizationTokenDataClient = factory.GetClient(typeof(PaymentAuthorizationToken));
            _failAllPayments = failAllPayments;
        }
        
        public CreditCardPaymentService(MongoDBClientFactory factory)
        {
            _transactionClient = factory.GetClient(typeof(Transaction));
            _paymentAuthorizationTokenDataClient = factory.GetClient(typeof(PaymentAuthorizationToken));
        }

        /// <inheritdoc/>
        public async Task<AuthorizeResult> Authorize(decimal amount, string authToken)
        {
            var (isValid, chargeResult) = await ValidateToken(authToken);

            if (!isValid)
                return new AuthorizeResult(authToken, chargeResult.Exception);
            
            if (!CreditCardHasTheRequiredAmount(amount, authToken))
                return new AuthorizeResult(null, new InvalidOperationException("The given credit card has not the required amount"));

            return new AuthorizeResult(authToken, null);
        }

        /// <inheritdoc/>
        public async Task<AuthorizeResult> Authorize(decimal amount, CreditCard creditCard)
        {
            if (!CreditCardHasTheRequiredAmount(amount, creditCard))
                return new AuthorizeResult(null, new InvalidOperationException("The given credit card has not the required amount"));

            var paymentAuthorizationToken = await CreatePaymentAuthorizationToken(creditCard);

            return new AuthorizeResult(paymentAuthorizationToken.Id, null);
        }
        
        /// <inheritdoc/>
        public async Task<string> GetPaymentToken(CreditCard creditCard)
        {
            var paymentAuthorizationToken = await CreatePaymentAuthorizationToken(creditCard);

            return paymentAuthorizationToken.Id;
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
            
            var paymentAuthorizationToken = await CreatePaymentAuthorizationToken(creditCard);

            return await Charge(amount, paymentAuthorizationToken.Id!);
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

                await _transactionClient.InsertOneAsync(transaction);

                return new ChargeResult
                {
                    Transaction = transaction,
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
            var t = await _paymentAuthorizationTokenDataClient
                .GetAsync<PaymentAuthorizationToken>(ExpressionCombinationOperator.And,
                    token => token.Id == authorizePaymentToken);
            
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
            
            if (t.ExpirationDate <= DateTime.Now)
            {
                return (false, new ChargeResult
                {
                    Exception = new InvalidOperationException($"{nameof(PaymentAuthorizationToken)} is expired")
                });
            }

            return (true, null!);
        }

        private async Task<PaymentAuthorizationToken> CreatePaymentAuthorizationToken(CreditCard creditCard)
        {
            // TODO : Make something with the creditcard
            var paymentAuthorizationToken = new PaymentAuthorizationToken(TimeSpan.FromDays(30));
            await _paymentAuthorizationTokenDataClient.InsertOneAsync(paymentAuthorizationToken);
            return paymentAuthorizationToken;
        }
        
        private bool CreditCardHasTheRequiredAmount(decimal amount, CreditCard creditCard)
        {
            // args not used in this mock implementation:
            _ = amount;
            _ = creditCard;
            
            return !MustPaymentFail();
        }
        
        private bool CreditCardHasTheRequiredAmount(decimal amount, string authToken)
        {
            // args not used in this mock implementation:
            _ = amount;
            _ = authToken;
            
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
