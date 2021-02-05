using System;
using System.Linq;
using artiso.AdsdHotel.Yellow.Contracts.Models;

namespace artiso.AdsdHotel.Yellow.Api.Validation
{
    public static class ValidateExtensions
    {
        public static ValidationModelResult<T> Validate<T>(this T model)
        {
            return new(model);
        }

        public static ValidationModelResult<T> That<T>(this ValidationModelResult<T> v, Func<T, bool> rule, string errorMessage = "")
        {
            if (v.Errors.Any())
                return v;

            if (!rule(v.Instance)) 
                v.SetError(errorMessage);

            return v;
        }
        
        public static ValidationModelResult<T> HasData<T>(this ValidationModelResult<T> v, Func<T, string> rule, string errorMessage)
        {
            var provValue = rule(v.Instance);

            return v.That(_ => !string.IsNullOrWhiteSpace(provValue), errorMessage);
        }
        
        public static ValidationModelResult<T> NotNull<T>(this ValidationModelResult<T> v, Func<T, object?> rule, string errorMessage)
        {
            var provValue = rule(v.Instance);

            return v.That(_ => provValue is not null, errorMessage);
        }
    }

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
                .That(c => c.CardNumber.All(n => int.TryParse(n.ToString(), out _)), $"{nameof(CreditCard.CardNumber)} must contain only digits") // only INTs in PAN
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
