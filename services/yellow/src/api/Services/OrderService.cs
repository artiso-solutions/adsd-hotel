using System.Threading.Tasks;
using artiso.AdsdHotel.ITOps.NoSql;
using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Api.Services
{
    public class OrderService : IOrderService
    {
        private readonly IDataStoreClient _dataStoreClient;

        public OrderService(IDataStoreClient dataStoreClient)
        {
            _dataStoreClient = dataStoreClient;
        }

        /// <inheritdoc/>
        public async Task<Order?> FindOneById(string id)
        {
            var order = await _dataStoreClient.GetAsync<Order>(ExpressionCombinationOperator.And, o => o.Id == id);

            return order;
        }

        /// <inheritdoc/>
        public Task AddPaymentMethod(Order order, OrderPaymentMethod orderPaymentMethod) => throw new System.NotImplementedException();
    }
}
