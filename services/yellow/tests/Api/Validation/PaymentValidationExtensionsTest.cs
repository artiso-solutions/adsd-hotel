using System;
using artiso.AdsdHotel.Yellow.Api.Validation;
using artiso.AdsdHotel.Yellow.Contracts.Models;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace artiso.AdsdHotel.Yellow.Tests.Api.Validation
{
    [TestFixture]
    public class PaymentValidationExtensionsTest
    {
        // [Test]
        // public void PaymentValidationTest()
        // {
        //     var paymentMethod = new PaymentMethod(new CreditCard(IssuingNetwork.AmericanExpress, string.Empty, 0, 0, DateTime.MaxValue));
        //
        //     var v = paymentMethod.Validate();
        //     
        //     v.PaymentMethodIsValid(_ => paymentMethod);
        //     
        //     Assert.True(v.IsValid());
        // }

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
        public void CreditCardNumberMustHaveValidLength()
        {
            var creditCard = new CreditCard(IssuingNetwork.AmericanExpress, "John Doe", "0", "1243", DateTime.MaxValue);
            
            var paymentMethod = new PaymentMethod(creditCard);
            
            var v = paymentMethod.Validate();
            
            v.PaymentMethodIsValid(_ => paymentMethod);
            
            Assert.False(v.IsValid());
            
            TestContext.Out.WriteLine($"Error message : {v.GetErrors()}");
        }
        
        [Test]
        public void CreditCvvMustHaveValidLength()
        {
            var creditCard = new CreditCard(IssuingNetwork.AmericanExpress, "John Doe", "349621197422556", "0", DateTime.MaxValue);
            
            var paymentMethod = new PaymentMethod(creditCard);
            
            var v = paymentMethod.Validate();
            
            v.PaymentMethodIsValid(_ => paymentMethod);
            
            Assert.False(v.IsValid());
            
            TestContext.Out.WriteLine($"Error message : {v.GetErrors()}");
        }
    }
}