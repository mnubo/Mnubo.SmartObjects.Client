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

        [Test()]
        public void TestGets()
        {
            Assert.GreaterOrEqual(client.Model.GetEventTypes().Count, 2, "event types count");
            Assert.GreaterOrEqual(client.Model.GetObjectTypes().Count, 1, "object types count");
            Assert.GreaterOrEqual(client.Model.GetObjectAttributes().Count, 1, "object attributes count");
            Assert.GreaterOrEqual(client.Model.GetTimeseries().Count, 2, "timeseries count");
            Assert.GreaterOrEqual(client.Model.GetOwnerAttributes().Count, 1, "owner attributes count");

        }

        [Test()]
        public void TestSandboxOps()
        {
            //we can't reset in it test, all sdk integration tests use the same namespace
            //client.Model.SandboxOps.ResetOps.Reset();

            string key = Guid.NewGuid().ToString().Replace("-", "");
            client.Model.SandboxOps.EventTypesOps.CreateOne(new EventType(
                key, "", "scheduled", new List<string>()
            ));
            client.Model.SandboxOps.EventTypesOps.Update(key, new EventType(
                key, "new desc", "unscheduled", new List<string>()
            ));
            client.Model.SandboxOps.EventTypesOps.AddRelation("event_type1", "ts_text_attribute");
            client.Model.SandboxOps.EventTypesOps.RemoveRelation("event_type1", "ts_text_attribute");

            client.Model.SandboxOps.ObjectTypesOps.CreateOne(new ObjectType(
                key, "", new List<string>()
            ));
            client.Model.SandboxOps.ObjectTypesOps.Update(key, new ObjectType(
                key, "new desc", new List<string>()
            ));
            client.Model.SandboxOps.ObjectTypesOps.AddRelation("cat_detector", "object_text_attribute");
            client.Model.SandboxOps.ObjectTypesOps.RemoveRelation("cat_detector", "object_text_attribute");


            string tsKey = key + "-ts";
            client.Model.SandboxOps.TimeseriesOps.CreateOne(new Timeseries(
                tsKey, "", "", "TEXT", new List<string>() { key }
            ));
            client.Model.SandboxOps.TimeseriesOps.Update(tsKey, new UpdateEntity("new display name", "new desc"));
            client.Model.SandboxOps.TimeseriesOps.Deploy(tsKey);

            string objKey = key + "-obj";
            client.Model.SandboxOps.ObjectAttributesOps.CreateOne(new ObjectAttribute(
                objKey, "", "", "FLOAT", "none", new List<string>() { key }
            ));
            client.Model.SandboxOps.ObjectAttributesOps.Update(objKey, new UpdateEntity("new display name", "new desc"));
            client.Model.SandboxOps.ObjectAttributesOps.Deploy(objKey);

            string ownerKey = key + "-own";
            client.Model.SandboxOps.OwnerAttributesOps.CreateOne(new OwnerAttribute(
                ownerKey, "", "", "DOUBLE", "none"
            ));
            client.Model.SandboxOps.OwnerAttributesOps.Update(ownerKey, new UpdateEntity("new display name", "new desc"));
            client.Model.SandboxOps.OwnerAttributesOps.Deploy(ownerKey);

            client.Model.SandboxOps.EventTypesOps.Delete(key);
            client.Model.SandboxOps.ObjectTypesOps.Delete(key);
        }
    }
}
