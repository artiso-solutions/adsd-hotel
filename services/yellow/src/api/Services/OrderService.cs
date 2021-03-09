using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.NoSql;
using artiso.AdsdHotel.Yellow.Api.Configuration;
using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Api.Services
{
    public class OrderService : IOrderService
    {
        private readonly IDataStoreClient _dataStoreClient;

        public OrderService(MongoDBClientFactory mongoDbClientFactory)
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
        public async Task AddPaymentMethod(Order order, OrderPaymentMethod orderPaymentMethod)
        {
            var paymentMethods = order.OrderPaymentMethods;
            
            paymentMethods.Add(orderPaymentMethod);

            await _dataStoreClient.AddOrUpdateAsync(order, entity => entity.Id == order.Id);
        }

        /// <inheritdoc/>
        public async Task Create(string orderId, Price price)
        {
            await _dataStoreClient.InsertOneAsync(new Order(orderId, price));
        }
    }
}
