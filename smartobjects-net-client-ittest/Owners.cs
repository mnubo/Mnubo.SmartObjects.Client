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
    public class OwnersITTests
    {
        private readonly ISmartObjectsClient client;

        public OwnersITTests()
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
        public void TestClaimUnclaim()
        {
            String uuid = Guid.NewGuid().ToString();
            String value1 = "value-" + uuid;
            Owner owner = new Owner.Builder() {
                Username = "username-" + uuid,
                Password = "password-" + uuid,
                Attributes = new Dictionary<string,object>() {
                    {"owner_text_attribute", value1}
                }
            };

            SmartObject smartObject = new SmartObject.Builder() {
                DeviceId = "deviceId-" + uuid,
                ObjectType = "object_type1",
                Attributes = new Dictionary<string,object>() {
                    {"object_text_attribute", value1}
                }
            };

            client.Owners.Create(owner);
            client.Objects.Create(smartObject);

            try {
                client.Owners.Claim(uuid, smartObject.DeviceId);
                Assert.Fail("Claim on a non existent owner should fail");
            } catch {
                //expected
            }

            try {
                client.Owners.Claim(owner.Username, uuid);
                Assert.Fail("Claim on a non existent owner should fail");
            } catch {
                //expected
            }

            ITTestHelper.EventuallyAssert(() => {
                ResultSet resultOwn = client.Restitution.Search(ITTestHelper.searchOwnerQuery(owner.Username));
                Assert.AreEqual(1, resultOwn.Rows.Count);

                ResultSet resultObj = client.Restitution.Search(ITTestHelper.searchObjectQuery(smartObject.DeviceId));
                Assert.AreEqual(1, resultObj.Rows.Count);
            });

            client.Owners.Claim(owner.Username, smartObject.DeviceId);

            ITTestHelper.EventuallyAssert(() => {
                ResultSet resultOwn = client.Restitution.Search(ITTestHelper.searchObjectByOwnerQuery(owner.Username));
                Assert.AreEqual(1, resultOwn.Rows.Count);
            });

            try {
                client.Owners.Unclaim(uuid, smartObject.DeviceId);
                Assert.Fail("Unclaim on an non-existing owner should fail");
            } catch {
                //expected
            }
            try {
                client.Owners.Unclaim(owner.Username, uuid);
                Assert.Fail("Unclaim on an non-existing object should fail");
            } catch {
                //expected
            }

            client.Owners.Unclaim(owner.Username, smartObject.DeviceId);
            ITTestHelper.EventuallyAssert(() => {
                ResultSet resultOwn = client.Restitution.Search(ITTestHelper.searchObjectByOwnerQuery(owner.Username));
                Assert.AreEqual(0, resultOwn.Rows.Count);
            });

            try {
                client.Owners.Unclaim(owner.Username, smartObject.DeviceId);
                Assert.Fail("Unclaim on an already unclaimed object should fail");
            } catch {
                //expected
            }

        }

        [Test()]
        public void TestCreateAndUpdate()
        {
            String uuid = Guid.NewGuid().ToString();
            String value1 = "value-" + uuid;
            Owner owner = new Owner.Builder() {
                Username = "username-" + uuid,
                Password = "password-" + uuid,
                Attributes = new Dictionary<string,object>() {
                    {"owner_text_attribute", value1}
                }
            };

            client.Owners.Create(owner);
            try {
                client.Owners.Create(new Owner.Builder() {
                    Username = "username-" + uuid
                });
                Assert.Fail("Invalid owner (no password here) creation should fail");
            } catch {
                //expected
            }

            ITTestHelper.EventuallyAssert(() => {
                ResultSet result = client.Restitution.Search(ITTestHelper.searchOwnerQuery(owner.Username));
                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(value1, result.Rows.ElementAt(0).Values.ElementAt(1));
            });

            Owner updated = new Owner.Builder() {
                Attributes = new Dictionary<string,object>() {
                    {"owner_text_attribute", "newValue"}
                }
            };
            client.Owners.Update(updated, owner.Username);
            ITTestHelper.EventuallyAssert(() => {
                ResultSet result = client.Restitution.Search(ITTestHelper.searchOwnerQuery(owner.Username));
                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual("newValue", result.Rows.ElementAt(0).Values.ElementAt(1));
            });
        }
        [Test()]
        public void TestDelete()
        {
            String uuid = Guid.NewGuid().ToString();
            Owner ownerToDelete = new Owner.Builder() {
                Username = "username-" + uuid,
                Password = "password-" + uuid
            };
            client.Owners.Create(ownerToDelete);
            ITTestHelper.EventuallyAssert(() => {
                ResultSet result = client.Restitution.Search(ITTestHelper.searchOwnerQuery(ownerToDelete.Username));
                Assert.AreEqual(1, result.Rows.Count);
            });

            client.Owners.Delete(ownerToDelete.Username);

            ITTestHelper.EventuallyAssert(() => {
                ResultSet result = client.Restitution.Search(ITTestHelper.searchOwnerQuery(ownerToDelete.Username));
                Assert.AreEqual(0, result.Rows.Count);
            });
            try {
                client.Owners.Delete(ownerToDelete.Username);
                Assert.Fail("Cannot delete on an owner that does not exists");
            } catch {
                //expected
            }
        }
    }
}
