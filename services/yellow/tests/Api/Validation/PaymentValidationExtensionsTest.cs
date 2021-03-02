using System;
using artiso.AdsdHotel.Yellow.Api.Validation;
using artiso.AdsdHotel.Yellow.Contracts.Models;
using NUnit.Framework;

namespace artiso.AdsdHotel.Yellow.Tests.Api.Validation
{
    [TestFixture]
    public class PaymentValidationExtensionsTest
    {
        [Test]
        public void PaymentMethodMustHaveCreditCard()
        {
            var paymentMethod = new PaymentMethod(null);
            
            var v = paymentMethod.Validate();
            
            v.PaymentMethodIsValid(_ => paymentMethod);
            
            Assert.False(v.IsValid());
            
            TestContext.Out.WriteLine($"Error message : {v.GetErrors()}");
        }
        
        [Test]
        public void CreditCardMustHaveCardHolder()
        {
            var paymentMethod = new PaymentMethod(new CreditCard(IssuingNetwork.AmericanExpress, string.Empty, "0", "0", DateTime.MaxValue));
            
            var v = paymentMethod.Validate();
            
            v.PaymentMethodIsValid(_ => paymentMethod);
            
            Assert.False(v.IsValid());
            
            TestContext.Out.WriteLine($"Error message : {v.GetErrors()}");
        }
        
        [Test]
        public void CreditCardMustValidExpirationDate()
        {
            var creditCard = new CreditCard(IssuingNetwork.AmericanExpress, "John Doe", "349621197422556", "123", DateTime.MinValue);
            
            var paymentMethod = new PaymentMethod(creditCard);
            
            var v = paymentMethod.Validate();
            
            v.PaymentMethodIsValid(_ => paymentMethod);
            
            Assert.False(v.IsValid());
            
            TestContext.Out.WriteLine($"Error message : {v.GetErrors()}");
        }
        
        [Test]
        public void CreditCardPanMustBeOnlyDigits()
        {
            var creditCard = new CreditCard(IssuingNetwork.AmericanExpress, "John Doe", "AAAA21197422556", "123", DateTime.MaxValue);
            
            var paymentMethod = new PaymentMethod(creditCard);
            
            var v = paymentMethod.Validate();
            
            v.PaymentMethodIsValid(_ => paymentMethod);
            
            Assert.False(v.IsValid());
            
            TestContext.Out.WriteLine($"Error message : {v.GetErrors()}");
        }
        
        [Test]
        public void CreditCardCvvMustBeOnlyDigits()
        {
            var creditCard = new CreditCard(IssuingNetwork.AmericanExpress, "John Doe", "349621197422556", "AA", DateTime.MaxValue);
            
            var paymentMethod = new PaymentMethod(creditCard);
            
            var v = paymentMethod.Validate();
            
            v.PaymentMethodIsValid(_ => paymentMethod);
            
            Assert.False(v.IsValid());
            
            TestContext.Out.WriteLine($"Error message : {v.GetErrors()}");
        }
        
        [Test]
        [TestCase(IssuingNetwork.AmericanExpress, "John Doe", "0", "1234")]
        [TestCase(IssuingNetwork.MasterCard, "John Doe", "0", "333")]
        public void CreditCardNumberMustHaveValidLength(IssuingNetwork network, string cardHolder, string pan, string cvv)
        {
            var creditCard = new CreditCard(network, cardHolder, pan, cvv, DateTime.MaxValue);
            
            var paymentMethod = new PaymentMethod(creditCard);
            
            var v = paymentMethod.Validate();
            
            v.PaymentMethodIsValid(_ => paymentMethod);
            
            Assert.False(v.IsValid());
            
            TestContext.Out.WriteLine($"Error message : {v.GetErrors()}");
        }
        
        [Test]
        [TestCase(IssuingNetwork.AmericanExpress, "John Doe", "349621197422556", "0")]
        [TestCase(IssuingNetwork.MasterCard, "John Doe", "5215265659358633", "0")]
        public void CreditCvvMustHaveValidLength(IssuingNetwork network, string cardHolder, string pan, string cvv)
        {
            var creditCard = new CreditCard(network, cardHolder, pan, cvv, DateTime.MaxValue);
            
            var paymentMethod = new PaymentMethod(creditCard);
            
            var v = paymentMethod.Validate();
            
            v.PaymentMethodIsValid(_ => paymentMethod);
            
            Assert.False(v.IsValid());
            
            TestContext.Out.WriteLine($"Error message : {v.GetErrors()}");
        }
    }
}
