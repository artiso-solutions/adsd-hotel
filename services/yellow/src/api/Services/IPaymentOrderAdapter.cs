// unset

using System.Threading.Tasks;
using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Api.Services
{
    public interface IPaymentOrderAdapter
    {
        Task<ChargeResult> ChargeOrder(Order order, decimal amount);
        Task<ChargeResult> ChargeOrder(Order order, decimal amount, PaymentMethod alternativePaymentMethod);
        Task AddPaymentMethodToOrder(Order order, CreditCard? creditCard, string? authorizePaymentToken);
    }
}
