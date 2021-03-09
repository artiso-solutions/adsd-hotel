using System;
using artiso.AdsdHotel.Yellow.Api.Configuration;
using artiso.AdsdHotel.Yellow.Api.Services;
using artiso.AdsdHotel.Yellow.Contracts;
using artiso.AdsdHotel.Yellow.Contracts.Models;
using Moq;
using NUnit.Framework;

namespace artiso.AdsdHotel.Yellow.Tests.Api.Services
{
    [TestFixture]
    public class OrderServiceTest
    {
        [Test]
        public void AddPaymentMethodFirstTime()
        {
            var dataStoreClient = new Mock<MongoDBClientFactory>(new MongoDbConfig());
            var orderService = new OrderService(dataStoreClient.Object);
            var order = new Order(Guid.NewGuid().ToString(), new Price(10, 100));
            var orderCreditCard = TestUtility.CreditCardStore.AMEX1.GetOrderCreditCard("PAY_TOKEN");
            
            Assert.DoesNotThrow(() =>
            {
                orderService.AddPaymentMethod(order, new StoredPaymentMethod(orderCreditCard));
            });
        }
    }
}
