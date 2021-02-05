using System;
using artiso.AdsdHotel.Yellow.Contracts.Models;
using NUnit.Framework;

namespace artiso.AdsdHotel.Yellow.Tests
{
    internal static class TestUtility
    {
        internal static TestCaseData GetCaseData(string caseName, params object[] payload)
        {
            var testCaseData = new TestCaseData(payload).SetName(caseName);

            return testCaseData;
        }

        internal static class CreditCardStore
        {
            public static CreditCard AMEX1 => new(
                IssuingNetwork.AmericanExpress,
                "John Doe",
                "349621197422556",
                "0000",
                DateTime.Now.AddYears(2));
            
            public static CreditCard MASTERCARD1 => new(
                IssuingNetwork.MasterCard,
                "John Doe",
                "5215181434935456",
                "000",
                DateTime.Now.AddYears(2));
            
            // TODO : Other card types to be added..

        }
    }
}
