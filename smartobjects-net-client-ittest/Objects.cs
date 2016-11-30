using System;
using NUnit.Framework;
using Mnubo.SmartObjects.Client.Impl;
using Mnubo.SmartObjects.Client.Config;
using Mnubo.SmartObjects.Client.Models;
using Mnubo.SmartObjects.Client.Models.Search;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Mnubo.SmartObjects.Client.ITTest
{
    [TestFixture()]
    public class ObjectsITTests
    {
        private readonly ISmartObjectsClient client;

        public ObjectsITTests()
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
        public void TestObjectsCreate()
        {
            String uuid = Guid.NewGuid().ToString();
            String value1 = "value-" + uuid;
            SmartObject smartObject = new SmartObject.Builder() {
                DeviceId = "deviceId-" + uuid,
                ObjectType = "object_type1",
                Attributes = new Dictionary<string,object>() {
                    {"object_text_attribute", value1}
                }
            };

            client.Objects.Create(smartObject);
            try {
                client.Objects.Create(new SmartObject.Builder() {
                    Username = "username-" + uuid
                });
                Assert.Fail("Invalid smartObject (no password here) creation should fail");
            } catch {
                //expected
            }

            ITTestHelper.EventuallyAssert(() => {
                ResultSet result = client.Restitution.Search(ITTestHelper.searchObjectQuery(smartObject.DeviceId));
                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(value1, result.Rows.ElementAt(0).Values.ElementAt(1));
            });

            SmartObject updated = new SmartObject.Builder() {
                Attributes = new Dictionary<string,object>() {
                    {"object_text_attribute", "newValue"}
                }
            };
            client.Objects.Update(updated, smartObject.DeviceId);
            ITTestHelper.EventuallyAssert(() => {
                ResultSet result = client.Restitution.Search(ITTestHelper.searchObjectQuery(smartObject.DeviceId));
                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual("newValue", result.Rows.ElementAt(0).Values.ElementAt(1));
            });
        }

        [Test()]
        public void TestObjectDelete()
        {
            String uuid = Guid.NewGuid().ToString();
            SmartObject objectToDelete = new SmartObject.Builder() {
                DeviceId = "deviceId-" + uuid,
                ObjectType = "object_type1"
            };
            client.Objects.Create(objectToDelete);
            ITTestHelper.EventuallyAssert(() => {
                ResultSet result = client.Restitution.Search(ITTestHelper.searchObjectQuery(objectToDelete.DeviceId));
                Assert.AreEqual(1, result.Rows.Count);
            });

            client.Objects.Delete(objectToDelete.DeviceId);

            ITTestHelper.EventuallyAssert(() => {
                ResultSet result = client.Restitution.Search(ITTestHelper.searchObjectQuery(objectToDelete.DeviceId));
                Assert.AreEqual(0, result.Rows.Count);
            });
            try {
                client.Objects.Delete(objectToDelete.Username);
                Assert.Fail("Cannot delete on an smart object that does not exists");
            } catch {
                //expected
            }
        }
    }
}
