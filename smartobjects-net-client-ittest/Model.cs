using System;
using NUnit.Framework;
using Mnubo.SmartObjects.Client.Impl;
using Mnubo.SmartObjects.Client.Config;
using Mnubo.SmartObjects.Client.Models.DataModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Mnubo.SmartObjects.Client.ITTest
{
    [TestFixture()]
    public class ModelITTests
    {
        private readonly ISmartObjectsClient client;

        public ModelITTests()
        {
            client = ITTestHelper.newClient();
        }

        [Test()]
        public void TestExport()
        {
            Model model = client.Model.Export();
            Assert.GreaterOrEqual(model.EventTypes.Count, 2, "event types count");
            Assert.GreaterOrEqual(model.Timeseries.Count, 2, "timeseries count");
            Assert.GreaterOrEqual(model.ObjectTypes.Count, 1, "object types count");
            Assert.GreaterOrEqual(model.ObjectAttributes.Count, 1, "object attributes count");
            Assert.GreaterOrEqual(model.OwnerAttributes.Count, 1, "owner attributes count");
            Assert.GreaterOrEqual(model.Sessionizers.Count, 1, "sessionizers count");

        }
    }
}
