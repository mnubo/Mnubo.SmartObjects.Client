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

            SmartObject updated = new SmartObject.Builder() {
                Attributes = new Dictionary<string,object>() {
                    {"object_text_attribute", "newValue"}
                }
            };
            client.Objects.Update(updated, smartObject.DeviceId);
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

            client.Objects.Delete(objectToDelete.DeviceId);
            try {
                client.Objects.Delete(objectToDelete.Username);
                Assert.Fail("Cannot delete on an smart object that does not exists");
            } catch {
                //expected
            }
        }
    }
}
