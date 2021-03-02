using System.Threading.Tasks;
using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Api.Services
{
    public interface ICreditCardPaymentService
    {
        /// <summary>
        /// Checks if the given <see cref="CreditCard"/> can pay the requested Amount
        /// Returns the authorizePaymentToken
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="creditCard"></param>
        /// <returns></returns>
        Task<AuthorizeResult> Authorize(decimal amount, CreditCard creditCard);
        
        /// <summary>
        /// Charges the given amount on the given <see cref="CreditCard"/>  
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="creditCard"></param>
        /// <returns></returns>
        Task<ChargeResult> Charge(decimal amount, CreditCard creditCard);
        
        /// <summary>
        /// Charges the given amount the <see cref="CreditCard"/> linked to the given paymentToken  
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="authorizePaymentToken"></param>
        /// <returns></returns>
        Task<ChargeResult> Charge(decimal amount, string authorizePaymentToken);
        
    }
}
