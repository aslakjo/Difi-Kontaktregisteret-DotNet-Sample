using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KontaktregisteretGateway.Tests
{
    [TestClass]
    public class HentPersonerTests
    {
        [TestMethod]
        public void GetContactInfoFor2Persons_ShouldBeSuccessful()
        {
            var gateway = new Gateway();
            var personalNumber = new string[2];
            personalNumber[0] = "02018090573";
            personalNumber[1] = "02018090301";
            var response = gateway.Execute(personalNumber);

            Assert.AreEqual(personalNumber[0], response.HentPersonerRespons[0].personidentifikator);
            Assert.AreEqual(personalNumber[1], response.HentPersonerRespons[1].personidentifikator);
        }
    }
}
