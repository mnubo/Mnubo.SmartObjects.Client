using System;
using NUnit.Framework;
using Mnubo.SmartObjects.Client.Impl;
using Mnubo.SmartObjects.Client.Config;
using Mnubo.SmartObjects.Client.Models.DataModel;
using Mnubo.SmartObjects.Client.Models.Search;
using System.Threading.Tasks;
using System.Linq;

namespace Mnubo.SmartObjects.Client.ITTest
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
