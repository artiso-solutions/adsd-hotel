using System;
using System.Linq;
using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Api.Validation
{
    public static class PaymentMethodValidateExtensions
    {
        public static ValidationModelResult<T> PaymentMethodIsValid<T>(this ValidationModelResult<T> v, Func<T, PaymentMethod> rule)
        {
            var paymentMethod = rule(v.Instance);

            var paymentMethodCreditCard = paymentMethod.CreditCard;

            if (paymentMethodCreditCard is null)
            {
                v.SetError($"{nameof(PaymentMethod.CreditCard)} must not be null");
                return v;
            }

            var creditCardValidateResult = paymentMethodCreditCard.Validate()
                .HasData(c => c.CardHolder, $"{nameof(CreditCard.CardHolder)} must not be empty")
                .That(c => c.ExpirationDate > DateTime.Today, $"Invalid {nameof(CreditCard.ExpirationDate)}")
                .That(c => c.CardNumber.All(n => int.TryParse((string?) n.ToString(), out _)), $"{nameof(CreditCard.CardNumber)} must contain only digits") // only INTs in PAN
                .That(c => c.Cvv.All(n => int.TryParse(n.ToString(), out _)), $"{nameof(CreditCard.Cvv)} must contain only digits")
                .That(c =>
                {
                    switch (paymentMethodCreditCard.IssuingNetwork)
                    {
                        case IssuingNetwork.AmericanExpress:
                            if (paymentMethodCreditCard.CardNumber.Length != 15)
                            {
                                return false;
                            }
                            break;
                        case IssuingNetwork.MasterCard:
                        case IssuingNetwork.Visa:
                        default:
                            if (paymentMethodCreditCard.CardNumber.Length != 16)
                            {
                                return false;
                            }
                            break;
                    }

                    return true;
                }, $"{nameof(CreditCard.CardNumber)} is invalid, length is invalid")
                .That(c =>
                {
                    switch (paymentMethodCreditCard.IssuingNetwork)
                    {
                        case IssuingNetwork.AmericanExpress:
                            if (paymentMethodCreditCard.Cvv.Length != 4)
                            {
                                return false;
                            }
                            break;
                        case IssuingNetwork.MasterCard:
                        case IssuingNetwork.Visa:
                        default:
                            if (paymentMethodCreditCard.Cvv.Length != 3)
                            {
                                return false;
                            }
                            break;
                    }

                    return true;
                }, $"{nameof(CreditCard.Cvv)} is invalid, length is invalid");
            
            if (!creditCardValidateResult.IsValid()) 
                v.SetError(creditCardValidateResult.GetErrors());

            return v;
        }
        
    }
}
