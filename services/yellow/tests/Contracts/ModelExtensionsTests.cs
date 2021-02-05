using System.Linq;
using artiso.AdsdHotel.Yellow.Contracts;
using NUnit.Framework;

namespace artiso.AdsdHotel.Yellow.Tests.Contracts
{
    [TestFixture]
    public class ModelExtensionsTests
    {
        [Test]
        public void GetOrderCreditCardTest()
        {
            const string? authToken = "_AUTH_TOKEN_";
            var cc = TestUtility.CreditCardStore.AMEX1.GetOrderCreditCard(authToken);
            
            Assert.AreEqual(cc.CardNumber.Length, TestUtility.CreditCardStore.AMEX1.CardNumber.Length);
            Assert.AreEqual(cc.CardNumber.Count(c => c != '*'), 4);
            Assert.AreEqual(authToken, cc.ProviderPaymentToken);
            
            TestContext.WriteLine(cc.CardNumber);
        }
    }
}