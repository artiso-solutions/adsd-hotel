using System.Threading.Tasks;
using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Api.Services
{
    public interface IOrderService
    {
        /// <summary>
        /// Find an order by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Order?> FindOneById(string id);
        
        /// <summary>
        /// Adds a Payment Method to an order
        /// 
        /// Collection
        /// Order
        /// PaymentMethods
        /// CreditCard (Register the last 4 digit of PAN)
        /// PaymentToken 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderPaymentMethod"></param>
        Task AddPaymentMethod(Order order, StoredPaymentMethod orderPaymentMethod);

        /// <summary>
        /// Create a new Order
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        Task Create(string orderId, Price price);

        /// <summary>
        /// Stores a new Transaction on the Order
        /// </summary>
        /// <returns></returns>
        Task AddTransaction(Order order, OrderTransaction transaction);
    }
}
