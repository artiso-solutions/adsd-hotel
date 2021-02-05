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
        Task<Order> FindOneById(string id);
        
        // Collection
        // Order
        // PaymentMethods
        // CreditCard (Register the last 4 digit of PAN)
        // PaymentToken
        /// <summary>
        /// Adds a Payment Method to an order 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderPaymentMethod"></param>
        void AddPaymentMethod(Order order, OrderPaymentMethod orderPaymentMethod);
    }
}