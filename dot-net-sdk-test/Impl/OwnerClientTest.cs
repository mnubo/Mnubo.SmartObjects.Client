using System;
using NUnit.Framework;
using Mnubo.SmartObjects.Client.Impl;
using Mnubo.SmartObjects.Client.Config;
using Mnubo.SmartObjects.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Mnubo.SmartObjects.Client.Test.Impl
{
    [TestFixture()]
//    [Ignore("Ignore a fixture")]
    public class OwnerClientTest
    {
        private const string UsernameBase = "dot.net.sdk.test";
        private const string Password = "pppp";
        private const string AttributeName = "attribute1";
        private const string ObjectTypeName = "wind_direction";
        private Random randomGenerator = new Random();

        private ClientConfig config =
                new ClientConfig.Builder()
                {
                    Environment = ClientConfig.Environments.Sandbox,
                    ConsumerKey = Config.ConsumerKey,
                    ConsumerSecret = Config.ConsumerSecret
                };
        private ISmartObjectsClient client;
        private IOwnerClient ownerClient;

        public OwnerClientTest()
        {
            client = ClientFactory.Create(config);
            ownerClient = client.Owners;
        }

        #region Async Calls
        [Test()]
        public void ClientOwnerSyncCreateFullOwnerUpdateDelete()
        {
            string username = GetRandomUsername();

            Owner owner = new Owner.Builder()
            {
                Username = username,
                Password = Password,
                Attributes = new Dictionary<string, object>()
                {
                    { AttributeName, "test" }
                },
                RegistrationDate = DateTime.UtcNow,
                EventId = Guid.NewGuid()
            };

            ownerClient.Create(owner);

            ownerClient.Update(
                new Owner.Builder()
                {
                    Attributes = new Dictionary<string, object>()
                    {
                        { AttributeName, "modified" }
                    }
                }, username);

            ownerClient.Delete(username);
        }

        [Test()]
        public void ClientOwnerSyncCreateBasicDelete()
        {
            string username = GetRandomUsername();

            Owner owner = new Owner.Builder()
            {
                Username = username,
                Password = Password
            };

            ownerClient.Create(owner);

            ownerClient.Delete(username);
        }

        [Test()]
        public void ClientOwnerSyncCreateClaimDelete()
        {
            string username = GetRandomUsername();
            string deviceId = username;

            SmartObject obj = new SmartObject.Builder()
            {
                DeviceId = deviceId,
                ObjectType = ObjectTypeName
            };

            Owner owner = new Owner.Builder()
            {
                Username = username,
                Password = Password
            };

            ownerClient.Create(owner);
            client.Objects.Create(obj);

            ownerClient.Claim(username, deviceId);

            ownerClient.Delete(username);
            client.Objects.Delete(deviceId);
        }

        [Test()]
        public void ClientOwnerSyncUpdatePasswordDelete()
        {
            string username = GetRandomUsername();

            Owner owner = new Owner.Builder()
            {
                Username = username,
                Password = Password
            };

            ownerClient.Create(owner);

            ownerClient.UpdatePassword(username, "newPassword");

            ownerClient.Delete(username);
        }

        [Test()]
        public void ClientOwnerSyncBatchCreateDelete()
        {
            List<Owner> owners = new List<Owner>();

            for (int i = 1; i <= 200; i++)
            {
                owners.Add(new Owner.Builder()
                {
                    Username = GetRandomUsername(),
                    Password = Password
                });
            }

            List<Result> ownerResult = ownerClient.CreateUpdate(owners);

            Assert.AreEqual(ownerResult.Count, 200);

            foreach (Owner owner in owners)
            {
                ownerClient.Delete(owner.Username);
            }
        }

        [Test()]
        public void ClientOwnerSyncCreateBadRequest()
        {
            string username = GetRandomUsername();

            Owner owner = new Owner.Builder()
            {
                Username = username,
                Password = Password,
                Attributes = new Dictionary<string, object>()
                {
                    { "Unknow", "5" }
                }
            };

            Assert.That(() => ownerClient.Create(owner),
                Throws.TypeOf<InvalidOperationException>()
                .With.Message.Contains("status code: BadRequest, message {\"path\":\"/owners\",\"errorCode\":400,\"requestId\":")
                .With.Message.Contains("\"message\":\"Unknown field 'unknow'\"}"));
        }

        [Test()]
        public void ClientOwnerSyncUpdateBadRequest()
        {
            Assert.That(() => ownerClient.Update(new Owner.Builder().Build(), "Unknown"),
                Throws.TypeOf<InvalidOperationException>()
                .With.Message.Contains("status code: BadRequest, message {\"path\":\"/owners/Unknown\",\"errorCode\":400,\"requestId\":")
                .With.Message.Contains("\"message\":\"Owner's attributes cannot be null or empty.\"}"));
        }

        [Test()]
        public void ClientOwnerSyncDeleteBadRequest()
        {
            Assert.That(() => ownerClient.Delete("Unknown"),
                Throws.TypeOf<InvalidOperationException>()
                .With.Message.Contains("status code: BadRequest, message {\"path\":\"/owners/Unknown\",\"errorCode\":400,\"requestId\":")
                .With.Message.Contains("\"message\":\"Owner 'Unknown' not found.\"}"));
        }

        [Test()]
        public void ClientOwnerSyncClaimBadRequest()
        {
            Assert.That(() => ownerClient.Claim("Unknown", "Unknown"),
                Throws.TypeOf<InvalidOperationException>()
                .With.Message.Contains("status code: BadRequest, message {\"path\":\"/owners/Unknown/objects/Unknown/claim\",\"errorCode\":400,\"requestId\":")
                .With.Message.Contains("\"message\":\"Owner Unknown does not exist.\"}"));
        }

        [Test()]
        public void ClientOwnerSyncUpdatePasswordBadRequest()
        {
            Assert.That(() => ownerClient.UpdatePassword("Unknown", "Unknown"),
                Throws.TypeOf<InvalidOperationException>()
                .With.Message.Contains("status code: BadRequest, message {\"path\":\"/owners/Unknown/password\",\"errorCode\":400,\"requestId\":")
                .With.Message.Contains("\"message\":\"The username : 'Unknown' not found.\"}"));
        }

        [Test()]
        public void ClientOwnerSyncClaimNullUsername()
        {
            Assert.That(() => ownerClient.Claim(null, "deviceid"),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("username cannot be blank."));
        }

        [Test()]
        public void ClientOwnerSyncClaimEmptyUsername()
        {
            Assert.That(() => ownerClient.Claim("", "deviceid"),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("username cannot be blank."));
        }

        [Test()]
        public void ClientOwnerSyncClaimNullDeviceId()
        {
            Assert.That(() => ownerClient.Claim("username", null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("device_Id cannot be blank."));
        }

        [Test()]
        public void ClientOwnerSyncClaimEmptyDeviceId()
        {
            Assert.That(() => ownerClient.Claim("username", ""),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("device_Id cannot be blank."));
        }

        [Test()]
        public void ClientOwnerSyncCreateNullBody()
        {
            Assert.That(() => ownerClient.Create(null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("owner body cannot be null."));
        }

        [Test()]
        public void ClientOwnerSyncCreateNotUsername()
        {
            Owner owner = new Owner.Builder().Build();

            Assert.That(() => ownerClient.Create(owner),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("username cannot be blank."));
        }

        [TestCase("", "username cannot be blank.", "password")]
        [TestCase(null, "username cannot be blank.", "password")]
        [TestCase("username", "password cannot be blank.", null)]
        [TestCase("username", "password cannot be blank.", "")]
        public void ClientOwnerSyncCreateUsernamePasswordValidator(string username, string errorMessage, string password)
        {
            Owner owner = new Owner.Builder()
            {
                Username = username,
                Password = password
            };

            Assert.That(() => ownerClient.Create(owner),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo(errorMessage));
        }

        [Test()]
        public void ClientOwnerSyncDeleteEmptyUsername()
        {
            Assert.That(() => ownerClient.Delete(""),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("username cannot be blank."));
        }

        [Test()]
        public void ClientOwnerSyncDeleteNullUsername()
        {
            Assert.That(() => ownerClient.Delete(null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("username cannot be blank."));
        }

        [Test()]
        public void ClientOwnerSyncUpdateNullBody()
        {
            Assert.That(() => ownerClient.Update(null, "Something"),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("owner body cannot be null."));
        }

        [Test()]
        public void ClientOwnerSyncUpdateEmptyUsername()
        {
            Assert.That(() => ownerClient.Update(new Owner.Builder().Build(), ""),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("username cannot be blank."));
        }

        [Test()]
        public void ClientOwnerSyncUpdateNullUsername()
        {
            Assert.That(() => ownerClient.Update(new Owner.Builder().Build(), null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("username cannot be blank."));
        }

        [Test()]
        public void ClientOwnerSyncUpdatePasswordEmptyUsername()
        {
            Assert.That(() => ownerClient.UpdatePassword("", "newPassword"),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("username cannot be blank."));
        }

        [Test()]
        public void ClientOwnerSyncUpdatePasswordNullUsername()
        {
            Assert.That(() => ownerClient.UpdatePassword(null, "newPassword"),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("username cannot be blank."));
        }

        [Test()]
        public void ClientOwnerSyncUpdatePasswordEmptyPassword()
        {
            Assert.That(() => ownerClient.UpdatePassword("username", ""),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("password cannot be blank."));
        }

        [Test()]
        public void ClientOwnerSyncUpdatePasswordNullPassworde()
        {
            Assert.That(() => ownerClient.UpdatePassword("username", null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("password cannot be blank."));
        }

        [Test()]
        public void ClientOwnerSyncBatchNullList()
        {
            Assert.That(() => ownerClient.CreateUpdate(null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("owner body list cannot be null."));
        }

        [Test()]
        public void ClientOwnerSyncBatchEmptyList()
        {
            Assert.That(() => ownerClient.CreateUpdate(new List<Owner>()),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Owner body list cannot be empty or biger that 1000."));
        }

        [Test()]
        public void ClientOwnerSyncBatchMaxSizeList()
        {
            List<Owner> owners = new List<Owner>();

            for (int i = 1; i <= 1001; i++)
            {
                owners.Add(new Owner.Builder().Build());
            }

            Assert.That(() => ownerClient.CreateUpdate(owners),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Owner body list cannot be empty or biger that 1000."));
        }

        [Test()]
        public void ClientOwnerSyncBatchNullUsername()
        {
            List<Owner> owners = new List<Owner>();

            for (int i = 1; i <= 2; i++)
            {
                owners.Add(new Owner.Builder().Build());
            }

            Assert.That(() => ownerClient.CreateUpdate(owners),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("username cannot be blank."));
        }
        #endregion

        #region Async Calls
        [Test()]
        public void ClientOwnerAsyncCreateOwnerUpdateClaimUpdatePasswordDelete()
        {
            List<Owner> owners = new List<Owner>();
            List<Task> createTasks = new List<Task>();
            for (int i = 1; i <= 5; i++)
            {
                owners.Add(new Owner.Builder()
                {
                    Username = GetRandomUsername(),
                    Password = Password,
                    RegistrationDate = DateTime.UtcNow,
                    Attributes = new Dictionary<string, object>()
                    {
                        { AttributeName, "test" }
                    }
                });
            }

            string deviceId = GetRandomUsername();
            SmartObject obj = new SmartObject.Builder()
            {
                DeviceId = deviceId,
                ObjectType = ObjectTypeName
            };
            client.Objects.Create(obj);

            owners.ForEach((owner) => createTasks.Add(ownerClient.CreateAsync(owner)));
            Task.WaitAll(createTasks.ToArray());

            List<Task> updateTask = new List<Task>();
            owners.ForEach((owner) => updateTask.Add(
                ownerClient.UpdateAsync(
                    new Owner.Builder()
                    {
                        Attributes = new Dictionary<string, object>()
                        {
                            { AttributeName, "modified" }
                        }
                    }, owner.Username)));
            Task.WaitAll(updateTask.ToArray());

            List<Task> claimTask = new List<Task>();
            owners.ForEach((owner) => claimTask.Add(
                ownerClient.ClaimAsync(owner.Username, deviceId)));
            Task.WaitAll(claimTask.ToArray());

            List<Task> updatePasswdTask = new List<Task>();
            owners.ForEach((owner) => updatePasswdTask.Add(
                ownerClient.UpdatePasswordAsync(owner.Username, "newPasswd")));
            Task.WaitAll(updatePasswdTask.ToArray());

            List<Task> deleteTask = new List<Task>();
            owners.ForEach((owner) => deleteTask.Add(
                ownerClient.DeleteAsync(owner.Username)));
            Task.WaitAll(deleteTask.ToArray());

            client.Objects.Delete(deviceId);
        }

        [Test()]
        public void ClientOwnerAsyncBatchDelete()
        {
            List<Owner> owners = new List<Owner>();
            Dictionary<Task<Owner>, Owner> createTasks = new Dictionary<Task<Owner>, Owner>();
            for (int i = 1; i <= 5; i++)
            {
                owners.Add(new Owner.Builder()
                {
                    Username = GetRandomUsername(),
                    Password = Password,
                    RegistrationDate = DateTime.UtcNow,
                    Attributes = new Dictionary<string, object>()
                    {
                        { AttributeName, "test" }
                    }
                });
            }

            Task<List<Result>> resultListTask = ownerClient.CreateUpdateAsync(owners);
            resultListTask.Wait();

            List<Task> deleteTask = new List<Task>();
            owners.ForEach((owner) => deleteTask.Add(
                ownerClient.DeleteAsync(owner.Username)));
            Task.WaitAll(deleteTask.ToArray());
        }

        [Test()]
        public void ClientOwnerAsyncClaimNullUsername()
        {
            Assert.That(() => ownerClient.ClaimAsync(null, "deviceid").Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("username cannot be blank."));
        }

        [Test()]
        public void ClientOwnerAsyncClaimEmptyUsername()
        {
            Assert.That(() => ownerClient.ClaimAsync("", "deviceid").Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("username cannot be blank."));
        }

        [Test()]
        public void ClientOwnerAsyncClaimNullDeviceId()
        {
            Assert.That(() => ownerClient.ClaimAsync("username", null).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("device_Id cannot be blank."));
        }

        [Test()]
        public void ClientOwnerAsyncClaimEmptyDeviceId()
        {
            Assert.That(() => ownerClient.ClaimAsync("username", "").Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("device_Id cannot be blank."));
        }

        [Test()]
        public void ClientOwnerAsyncCreateNullBody()
        {
            Assert.That(() => ownerClient.CreateAsync(null).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("owner body cannot be null."));
        }

        [Test()]
        public void ClientOwnerAsyncCreateNotUsername()
        {
            Owner owner = new Owner.Builder().Build();

            Assert.That(() => ownerClient.CreateAsync(owner).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("username cannot be blank."));
        }

        [TestCase("", "username cannot be blank.", "password")]
        [TestCase(null, "username cannot be blank.", "password")]
        [TestCase("username", "password cannot be blank.", null)]
        [TestCase("username", "password cannot be blank.", "")]
        public void ClientOwnerAsyncCreateEmptyUsername(string username, string errorMsg, string password)
        {
            Owner owner = new Owner.Builder()
            {
                Username = username,
                Password = password
            };

            Assert.That(() => ownerClient.CreateAsync(owner).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo(errorMsg));
        }

        [Test()]
        public void ClientOwnerAsyncDeleteEmptyUsername()
        {
            Assert.That(() => ownerClient.DeleteAsync("").Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("username cannot be blank."));
        }

        [Test()]
        public void ClientOwnerAsyncDeleteNullUsername()
        {
            Assert.That(() => ownerClient.DeleteAsync(null).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("username cannot be blank."));
        }

        [Test()]
        public void ClientOwnerAsyncUpdateNullBody()
        {
            Assert.That(() => ownerClient.UpdateAsync(null, "Something").Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("owner body cannot be null."));
        }

        [Test()]
        public void ClientOwnerAsyncUpdateEmptyUsername()
        {
            Assert.That(() => ownerClient.UpdateAsync(new Owner.Builder().Build(), "").Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("username cannot be blank."));
        }

        [Test()]
        public void ClientOwnerAsyncUpdateNullUsername()
        {
            Assert.That(() => ownerClient.UpdateAsync(new Owner.Builder().Build(), null).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("username cannot be blank."));
        }

        [Test()]
        public void ClientOwnerAsyncUpdatePasswordEmptyUsername()
        {
            Assert.That(() => ownerClient.UpdatePasswordAsync("", "newPassword").Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("username cannot be blank."));
        }

        [Test()]
        public void ClientOwnerAsyncUpdatePasswordNullUsername()
        {
            Assert.That(() => ownerClient.UpdatePasswordAsync(null, "newPassword").Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("username cannot be blank."));
        }

        [Test()]
        public void ClientOwnerAsyncUpdatePasswordEmptyPassword()
        {
            Assert.That(() => ownerClient.UpdatePasswordAsync("username", "").Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("password cannot be blank."));
        }

        [Test()]
        public void ClientOwnerAsyncUpdatePasswordNullPassworde()
        {
            Assert.That(() => ownerClient.UpdatePasswordAsync("username", null).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("password cannot be blank."));
        }

        [Test()]
        public void ClientOwnerAsyncBatchNullList()
        {
            Assert.That(() => ownerClient.CreateUpdateAsync(null).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("owner body list cannot be null."));
        }

        [Test()]
        public void ClientOwnerAsyncBatchEmptyList()
        {
            Assert.That(() => ownerClient.CreateUpdateAsync(new List<Owner>()).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("Owner body list cannot be empty or biger that 1000."));
        }

        [Test()]
        public void ClientOwnerAsyncBatchMaxSizeList()
        {
            List<Owner> owners = new List<Owner>();

            for (int i = 1; i <= 1001; i++)
            {
                owners.Add(new Owner.Builder().Build());
            }

            Assert.That(() => ownerClient.CreateUpdateAsync(owners).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("Owner body list cannot be empty or biger that 1000."));
        }

        [Test()]
        public void ClientOwnerAsyncBatchNullUsername()
        {
            List<Owner> owners = new List<Owner>();

            for (int i = 1; i <= 2; i++)
            {
                owners.Add(new Owner.Builder().Build());
            }

            Assert.That(() => ownerClient.CreateUpdateAsync(owners).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("username cannot be blank."));
        }
        #endregion

        private string GetRandomUsername()
        {
            return UsernameBase + randomGenerator.Next(1000000).ToString() + randomGenerator.Next(1000000).ToString();
        }

        private void AssertIfAreDifferent(Owner ownerA, Owner ownerB)
        {
            Assert.AreEqual(ownerA.Username, ownerB.Username);
            Assert.AreEqual(ownerA.RegistrationDate.Value.ToString(EventSerializerTest.DatetimeFormat),
                ownerB.RegistrationDate.Value.ToString(EventSerializerTest.DatetimeFormat));
            CollectionAssert.AreEqual(ownerA.Attributes, ownerB.Attributes);
        }
    }
}