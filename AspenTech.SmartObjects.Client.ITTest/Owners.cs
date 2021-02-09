using System;
using NUnit.Framework;
using System.Collections.Generic;
using AspenTech.SmartObjects.Client.Models;

namespace AspenTech.SmartObjects.Client.ITTest
{
    [TestFixture()]
    public class OwnersITTests
    {
        private readonly ISmartObjectsClient client;

        public OwnersITTests()
        {
            client = ITTestHelper.newClient();
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

            client.Owners.Claim(owner.Username, smartObject.DeviceId);

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

            try {
                client.Owners.Unclaim(owner.Username, smartObject.DeviceId);
                Assert.Fail("Unclaim on an already unclaimed object should fail");
            } catch {
                //expected
            }

        }


        [Test()]
        public void TestClaimUnclaimWithBody()
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
            Dictionary<string,object> body = new Dictionary<string,object>() {
                {"x_timestamp", "2017-04-24T16:13:11+00:00"}
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
                client.Owners.Claim(uuid, smartObject.DeviceId, body);
                Assert.Fail("Claim on a non existent owner should fail");
            } catch {
                //expected
            }

            try {
                client.Owners.Claim(owner.Username, uuid, body);
                Assert.Fail("Claim on a non existent owner should fail");
            } catch {
                //expected
            }

            client.Owners.Claim(owner.Username, smartObject.DeviceId, body);

            try {
                client.Owners.Unclaim(uuid, smartObject.DeviceId, body);
                Assert.Fail("Unclaim on an non-existing owner should fail");
            } catch {
                //expected
            }
            try {
                client.Owners.Unclaim(owner.Username, uuid, body);
                Assert.Fail("Unclaim on an non-existing object should fail");
            } catch {
                //expected
            }

            client.Owners.Unclaim(owner.Username, smartObject.DeviceId, body);

            try {
                client.Owners.Unclaim(owner.Username, smartObject.DeviceId);
                Assert.Fail("Unclaim on an already unclaimed object should fail");
            } catch {
                //expected
            }

        }

        [Test()]
        public void TestBatchClaimUnclaim()
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


            Dictionary<string,object> body = new Dictionary<string,object>() {
                {"x_timestamp", "2017-04-24T16:13:11+00:00"}
            };
            IEnumerable<ClaimOrUnclaim>  valid = new List<ClaimOrUnclaim>() {
                new ClaimOrUnclaim(owner.Username, smartObject.DeviceId, body)
            };
            IEnumerable<ClaimOrUnclaim>  unknownUsername = new List<ClaimOrUnclaim>() {
                new ClaimOrUnclaim(uuid, smartObject.DeviceId, body)
            };
            IEnumerable<ClaimOrUnclaim>  unknownDeviceId = new List<ClaimOrUnclaim>() {
                new ClaimOrUnclaim(owner.Username, uuid, body)
            };

            client.Owners.Create(owner);
            client.Objects.Create(smartObject);

            ITTestHelper.AllFailed(client.Owners.BatchClaim(unknownUsername));
            ITTestHelper.AllFailed(client.Owners.BatchClaim(unknownDeviceId));

            ITTestHelper.AllSuccess(client.Owners.BatchClaim(valid));

            ITTestHelper.AllFailed(client.Owners.BatchUnclaim(unknownUsername));
            ITTestHelper.AllFailed(client.Owners.BatchUnclaim(unknownDeviceId));

            ITTestHelper.AllSuccess(client.Owners.BatchUnclaim(valid));

            //Already unclaimed
            ITTestHelper.AllFailed(client.Owners.BatchUnclaim(valid));
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

            Owner updated = new Owner.Builder() {
                Attributes = new Dictionary<string,object>() {
                    {"owner_text_attribute", "newValue"}
                }
            };
            client.Owners.Update(updated, owner.Username);
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
            client.Owners.Delete(ownerToDelete.Username);

            try {
                client.Owners.Delete(ownerToDelete.Username);
                Assert.Fail("Cannot delete on an owner that does not exists");
            } catch {
                //expected
            }
        }
    }
}
