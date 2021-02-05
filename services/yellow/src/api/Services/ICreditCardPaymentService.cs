using System.Threading.Tasks;
using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Api.Services
{
    public interface ICreditCardPaymentService
    {
        // Transactions
        // OrderId
        // FopId
        Task<AuthorizeResult> Authorize(decimal amount, CreditCard creditCard);
    }
}