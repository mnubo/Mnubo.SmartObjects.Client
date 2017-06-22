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
            if (Environment.GetEnvironmentVariable("CONSUMER_KEY") == null ||
                Environment.GetEnvironmentVariable("CONSUMER_SECRET") == null) {
                throw new Exception("Consumer key/secret are unvailable");
            }

            client = ClientFactory.Create(
                new ClientConfig.Builder()
                {
                    Environment = ClientConfig.Environments.Sandbox,
                    ConsumerKey = Environment.GetEnvironmentVariable("CONSUMER_KEY"),
                    ConsumerSecret = Environment.GetEnvironmentVariable("CONSUMER_SECRET")
                }
            );
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
