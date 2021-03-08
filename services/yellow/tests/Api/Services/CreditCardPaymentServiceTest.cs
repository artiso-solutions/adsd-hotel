using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.NoSql;
using artiso.AdsdHotel.Yellow.Api.Services;
using artiso.AdsdHotel.Yellow.Contracts.Models;
using Moq;
using NUnit.Framework;

namespace artiso.AdsdHotel.Yellow.Tests.Api.Services
{
    [TestFixture]
    public class CreditCardPaymentServiceTest
    {
        private CreditCardPaymentService? _service;
        private Mock<IDataStoreClient>? _dataStoreClient;

        [SetUp]
        public void Setup()
        {
            _dataStoreClient = new Mock<IDataStoreClient>();
            _service = new CreditCardPaymentService(_dataStoreClient.Object);
        }
        
        [Test]
        public void AuthorizeOK()
        {
            AuthorizeResult? result = null;

            _dataStoreClient!.Setup(d => d.InsertOneAsync(It.IsAny<PaymentAuthorizationToken>()));
            
            Assert.DoesNotThrowAsync(async () =>
            {
                 result = await _service!.Authorize(10, TestUtility.CreditCardStore.AMEX1);
            });

            _dataStoreClient
                .Verify(d => d.InsertOneAsync(
                    It.Is<PaymentAuthorizationToken>(p => 
                        !string.IsNullOrEmpty(p.Token) &&
                        p.CreatedAt < p.ExpirationDate &&
                        p.Active)), 
                    Times.AtMostOnce);
                
            Assert.True(result?.IsSuccess);
            Assert.IsNull(result?.Exception);
            Assert.IsNotEmpty(result?.AuthorizePaymentToken);
        }

        [Test]
        public void ChargeWithTokenNotActive()
        {
            ChargeResult? result = null;
            var amount = 10;
            var paymentAuthToken = new PaymentAuthorizationToken(TimeSpan.FromDays(1)) {Active = false};
            var token = paymentAuthToken.Token;
            
            _dataStoreClient!.Setup(d => d.GetAsync(ExpressionCombinationOperator.And, 
                    It.IsAny<Expression<Func<PaymentAuthorizationToken,bool>>>()))
                .Returns<ExpressionCombinationOperator,Expression<Func<PaymentAuthorizationToken,bool>>[]>(
                    (_, _) => Task.FromResult(paymentAuthToken)!);
            
            Assert.DoesNotThrowAsync(async () =>
            {
                result = await _service!.Charge(amount, token);
            });
            
            _dataStoreClient!
                .Verify(d => 
                        d.GetAsync(ExpressionCombinationOperator.And, 
                            It.IsAny<Expression<Func<PaymentAuthorizationToken,bool>>>()),
                    Times.Once);
            
            Assert.False(result?.IsSuccess);
            Assert.IsNotNull(result?.Exception);
            Assert.IsInstanceOf<InvalidOperationException>(result?.Exception);
        }
        
        [Test]
        public void ChargeWithTokenNotFound()
        {
            ChargeResult? result = null;
            var amount = 10;
            var token = "token";
            
            Assert.DoesNotThrowAsync(async () =>
            {
                result = await _service!.Charge(amount, token);
            });
            
            _dataStoreClient!
                .Verify(d => 
                        d.GetAsync(ExpressionCombinationOperator.And, 
                            It.IsAny<Expression<Func<PaymentAuthorizationToken,bool>>>()),
                    Times.Once);
            
            Assert.False(result?.IsSuccess);
            Assert.IsNotNull(result?.Exception);
            Assert.IsInstanceOf<InvalidOperationException>(result?.Exception);
        }
        
        [Test]
        public void ChargeWithTokenExpired()
        {
            ChargeResult? result = null;
            var amount = 10;
            var token = "token";
            
            _dataStoreClient!.Setup(d => d.GetAsync(ExpressionCombinationOperator.And, 
                    It.IsAny<Expression<Func<PaymentAuthorizationToken,bool>>>()))
                .Returns<ExpressionCombinationOperator,Expression<Func<PaymentAuthorizationToken,bool>>[]>(
                    (_, _) => Task.FromResult(new PaymentAuthorizationToken(TimeSpan.FromDays(-1)))!);
            
            Assert.DoesNotThrowAsync(async () =>
            {
                result = await _service!.Charge(amount, token);
            });
            
            _dataStoreClient!
                .Verify(d => 
                        d.GetAsync(ExpressionCombinationOperator.And, 
                            It.IsAny<Expression<Func<PaymentAuthorizationToken,bool>>>()),
                    Times.Once);
            
            Assert.False(result?.IsSuccess);
            Assert.IsNotNull(result?.Exception);
            Assert.IsInstanceOf<InvalidOperationException>(result?.Exception);
        }
        
        [Test]
        public void ChargeWithTokenOk()
        {
            ChargeResult? result = null;
            var amount = 10;
            var token = "token";

            _dataStoreClient!.Setup(d => d.InsertOneAsync(It.IsAny<PaymentAuthorizationToken>()));
            _dataStoreClient!.Setup(d => d.InsertOneAsync(It.IsAny<Transaction>()));
            _dataStoreClient!.Setup(d => d.GetAsync(ExpressionCombinationOperator.And, 
                It.IsAny<Expression<Func<PaymentAuthorizationToken,bool>>>()))
                .Returns<ExpressionCombinationOperator,Expression<Func<PaymentAuthorizationToken,bool>>[]>(
                    (_, _) => Task.FromResult(new PaymentAuthorizationToken(TimeSpan.FromDays(30)))!);
            
            Assert.DoesNotThrowAsync(async () =>
            {
                result = await _service!.Charge(amount, token);
            });

            _dataStoreClient
                .Verify(d => d.InsertOneAsync(
                    It.Is<Transaction>(t =>
                        t.Amount == amount &&
                        t.CreatedAt >= DateTime.Today)),
            Times.Once);
            
            _dataStoreClient
                .Verify(d => 
                        d.GetAsync(ExpressionCombinationOperator.And, 
                            It.IsAny<Expression<Func<PaymentAuthorizationToken,bool>>>()),
            Times.Once);
            
            Assert.True(result?.IsSuccess);
            Assert.IsNull(result?.Exception);
            Assert.IsNotEmpty(result?.AuthorizePaymentToken);
        }

        [Test]
        public void ChargeWithCreditCardOk()
        {
            ChargeResult? result = null;
            var amount = 10;
            var creditCard = TestUtility.CreditCardStore.AMEX1;
            
            _dataStoreClient!.Setup(d => d.InsertOneAsync(It.IsAny<PaymentAuthorizationToken>()));
            _dataStoreClient!.Setup(d => d.InsertOneAsync(It.IsAny<Transaction>()));
            _dataStoreClient!.Setup(d => d.GetAsync(ExpressionCombinationOperator.And, 
                    It.IsAny<Expression<Func<PaymentAuthorizationToken,bool>>>()))
                .Returns<ExpressionCombinationOperator,Expression<Func<PaymentAuthorizationToken,bool>>[]>(
                    (_, _) => Task.FromResult(new PaymentAuthorizationToken(TimeSpan.FromDays(30)))!);
            
            Assert.DoesNotThrowAsync(async () =>
            {
                result = await _service!.Charge(amount, creditCard);
            });
            
            // This method is supposed to create a new PaymentAuthorizationToken
            _dataStoreClient
                .Verify(d => d.InsertOneAsync(
                        It.Is<PaymentAuthorizationToken>(p => 
                            !string.IsNullOrEmpty(p.Token) &&
                            p.CreatedAt < p.ExpirationDate &&
                            p.Active)), 
                    Times.Once);
            
            // This method is supposed to get the newly created PaymentAuthorizationToken
            _dataStoreClient
                .Verify(d => 
                        d.GetAsync(ExpressionCombinationOperator.And, 
                            It.IsAny<Expression<Func<PaymentAuthorizationToken,bool>>>()),
                    Times.Once);
            
            // This method is supposed to create a transaction
            _dataStoreClient
                .Verify(d => d.InsertOneAsync(
                        It.Is<Transaction>(t =>
                            t.Amount == amount &&
                            t.CreatedAt >= DateTime.Today)),
                    Times.Once);
            
            Assert.True(result?.IsSuccess);
            Assert.IsNull(result?.Exception);
            Assert.IsNotEmpty(result?.AuthorizePaymentToken);
        }
        
        [Test]
        public void ChargeWithCreditCardKO()
        {
            ChargeResult? result = null;
            const int amount = 10;
            var creditCard = TestUtility.CreditCardStore.AMEX1;
            
            _dataStoreClient!.Setup(d => d.InsertOneAsync(It.IsAny<PaymentAuthorizationToken>()));
            _dataStoreClient!.Setup(d => d.InsertOneAsync(It.IsAny<Transaction>()));
            _dataStoreClient!.Setup(d => d.GetAsync(ExpressionCombinationOperator.And, 
                    It.IsAny<Expression<Func<PaymentAuthorizationToken,bool>>>()))
                .Returns<ExpressionCombinationOperator,Expression<Func<PaymentAuthorizationToken,bool>>[]>(
                    (_, _) => Task.FromResult(new PaymentAuthorizationToken(TimeSpan.FromDays(30)))!);
            
            var failAbleService = new CreditCardPaymentService(new Mock<IDataStoreClient>().Object, true);
            
            Assert.DoesNotThrowAsync(async () =>
            {
                result = await failAbleService!.Charge(amount, creditCard);
            });
            
            // This method is supposed NOT to create a new PaymentAuthorizationToken
            _dataStoreClient
                .Verify(d => d.InsertOneAsync(
                        It.Is<PaymentAuthorizationToken>(p => 
                            !string.IsNullOrEmpty(p.Token) &&
                            p.CreatedAt < p.ExpirationDate &&
                            p.Active)), 
                    Times.Never);
            
            // This method is supposed NOT to try to get a PaymentAuthorizationToken
            _dataStoreClient
                .Verify(d => 
                        d.GetAsync(ExpressionCombinationOperator.And, 
                            It.IsAny<Expression<Func<PaymentAuthorizationToken,bool>>>()),
                    Times.Never);
            
            // This method is supposed NOT to create a transaction
            _dataStoreClient
                .Verify(d => d.InsertOneAsync(
                        It.Is<Transaction>(t =>
                            t.Amount == amount &&
                            t.CreatedAt >= DateTime.Today)),
                    Times.Never);
            
            Assert.False(result?.IsSuccess);
            Assert.IsNotNull(result?.Exception);
            Assert.IsInstanceOf<InvalidOperationException>(result?.Exception);
            Assert.IsNull(result?.AuthorizePaymentToken);
        }

        [Test]
        public void AuthorizeKO()
        {
            AuthorizeResult? result = null;

            var service = new CreditCardPaymentService(new Mock<IDataStoreClient>().Object, true);
            
            Assert.DoesNotThrowAsync(async () =>
            {
                result = await service.Authorize(10, TestUtility.CreditCardStore.AMEX1);
            });
            
            Assert.False(result?.IsSuccess);
            Assert.IsNotNull(result?.Exception);
            Assert.IsInstanceOf<InvalidOperationException>(result?.Exception);
            Assert.IsNull(result?.AuthorizePaymentToken);
        }
    }
}