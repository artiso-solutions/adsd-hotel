using System;
using artiso.AdsdHotel.ITOps.NoSql;
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
            var dataStoreClient = new Mock<MongoDbClientFactory>(new MongoDbConfig());
            var orderService = new OrderService(dataStoreClient.Object);
            var order = new Order(Guid.NewGuid().ToString(), new Price(10, 100));
            var orderCreditCard = TestUtility.CreditCardStore.AMEX1.GetOrderCreditCard("PAY_TOKEN");

            Assert.DoesNotThrowAsync(async () =>
            {
                await orderService.AddPaymentMethod(order, new StoredPaymentMethod(orderCreditCard));
            });
        }
    }
}
