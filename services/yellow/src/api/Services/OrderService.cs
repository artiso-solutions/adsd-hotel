using System.Collections.Generic;
using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.NoSql;
using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Api.Services
{
    public class OrderService : IOrderService
    {
        private readonly IDataStoreClient _dataStoreClient;

        public OrderService(MongoDbClientFactory mongoDbClientFactory)
        {
            _dataStoreClient = mongoDbClientFactory.GetClient(typeof(Order));
        }

        /// <inheritdoc/>
        public async Task<Order?> FindOneById(string id)
        {
            var order = await _dataStoreClient.GetAsync<Order>(ExpressionCombinationOperator.And,
                o => o.Id == id);

            return order;
        }

        /// <inheritdoc/>
        public async Task AddPaymentMethod(Order order, StoredPaymentMethod orderPaymentMethod)
        {
            var paymentMethods = order.PaymentMethods ?? new List<StoredPaymentMethod>();

            paymentMethods.Add(orderPaymentMethod);

            order.PaymentMethods = paymentMethods;

            await _dataStoreClient.AddOrUpdateAsync(order, entity => entity.Id == order.Id);
        }

        /// <inheritdoc/>
        public async Task Create(string orderId, Price price)
        {
            await _dataStoreClient.InsertOneAsync(new Order(orderId, price));
        }

        /// <inheritdoc/>
        public async Task AddTransaction(Order order, OrderTransaction transaction)
        {
            var transactions = order.Transactions ?? new List<OrderTransaction>();

            transactions.Add(transaction);

            order.Transactions = transactions;

            await _dataStoreClient.AddOrUpdateAsync(order, entity => entity.Id == order.Id);
        }
    }
}
