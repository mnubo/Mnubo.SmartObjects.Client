using NUnit.Framework;
using AspenTech.SmartObjects.Client.Models.Search;

namespace AspenTech.SmartObjects.Client.ITTest
{
    [TestFixture()]
    public class SearchITTest
    {
        private readonly ISmartObjectsClient client;

        public SearchITTest()
        {
            client = ITTestHelper.newClient();
        }

        [Test()]
        public void TestExport()
        {
            ResultSet results = client.Restitution.Search(@"{""from"":""event"",""select"":[{""count"":""*""}]}}");
            Assert.AreEqual(1, results.Columns.Count);
            Assert.AreEqual(1, results.Rows.Count);

        }
    }
}
