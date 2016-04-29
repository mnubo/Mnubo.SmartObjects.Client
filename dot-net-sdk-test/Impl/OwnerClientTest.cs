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
    public class OwnerClientTest
    {
        private readonly ClientConfig config;
        private readonly ISmartObjectsClient client;
        private readonly IOwnerClient ownerClient;

        public OwnerClientTest()
        {
            config =
                new ClientConfig.Builder()
                {
                    Environment = ClientConfig.Environments.Sandbox,
                    ConsumerKey = Config.ConsumerKey,
                    ConsumerSecret = Config.ConsumerSecret
                };
            client = ClientFactory.Create(config);
            ownerClient = client.Owners;
        }

        #region Sync Calls
        [Test()]
        public void ClientOwnerSyncCreateFullOwnerUpdateDelete()
        {
            Owner owner = TestUtils.CreateFullOwner();

            ownerClient.Create(owner);

            ownerClient.Update(TestUtils.CreateOwnerUpdateAttribute(), owner.Username);

            ownerClient.Delete(owner.Username);
        }

        [Test()]
        public void ClientOwnerSyncCreateBasicDelete()
        {
            Owner owner = TestUtils.CreateBasicOwner();

            ownerClient.Create(owner);

            ownerClient.Delete(owner.Username);
        }

        [Test()]
        public void ClientOwnerSyncCreateClaimDelete()
        {
            SmartObject obj = TestUtils.CreateBasicObject();

            Owner owner = TestUtils.CreateBasicOwner();

            ownerClient.Create(owner);
            client.Objects.Create(obj);

            ownerClient.Claim(owner.Username, obj.DeviceId);

            ownerClient.Delete(owner.Username);
            client.Objects.Delete(obj.DeviceId);
        }

        [Test()]
        public void ClientOwnerSyncUpdatePasswordDelete()
        {
            Owner owner = TestUtils.CreateBasicOwner();

            ownerClient.Create(owner);

            ownerClient.UpdatePassword(owner.Username, "newPassword");

            ownerClient.Delete(owner.Username);
        }

        [Test()]
        public void ClientOwnerSyncBatchCreateDelete()
        {
            List<Owner> owners = TestUtils.CreateOwners(200);

            IEnumerable<Result> ownerResult = ownerClient.CreateUpdate(owners);

            Assert.AreEqual(ownerResult.Count(), 200);

            foreach (Owner owner in owners)
            {
                ownerClient.Delete(owner.Username);
            }
        }

        [Test()]
        public void ClientOwnerSyncCreateBadRequest()
        {
            Owner owner = TestUtils.CreateOwnerWrongAttribute();

            Assert.That(() => ownerClient.Create(owner),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Unknown field 'unknow'"));
        }

        [Test()]
        public void ClientOwnerSyncUpdateBadRequest()
        {
            Assert.That(() => ownerClient.Update(new Owner.Builder().Build(), "Unknown"),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Owner's attributes cannot be null or empty."));
        }

        [Test()]
        public void ClientOwnerSyncDeleteBadRequest()
        {
            Assert.That(() => ownerClient.Delete("Unknown"),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Owner 'Unknown' not found."));
        }

        [Test()]
        public void ClientOwnerSyncClaimBadRequest()
        {
            Assert.That(() => ownerClient.Claim("Unknown", "Unknown"),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Owner Unknown does not exist."));
        }

        [Test()]
        public void ClientOwnerSyncUpdatePasswordBadRequest()
        {
            Assert.That(() => ownerClient.UpdatePassword("Unknown", "Unknown"),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("The username : 'Unknown' not found."));
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
            Owner owner = TestUtils.CreateOwner();
            SmartObject obj = TestUtils.CreateBasicObject();

            client.Objects.CreateAsync(obj).Wait();
            ownerClient.CreateAsync(owner).Wait();

            ownerClient.UpdateAsync(TestUtils.CreateOwnerUpdateAttribute(),owner.Username).Wait();

            ownerClient.ClaimAsync(owner.Username, obj.DeviceId).Wait();

            ownerClient.UpdatePasswordAsync(owner.Username, "newPasswd").Wait();

            ownerClient.DeleteAsync(owner.Username).Wait();
            client.Objects.Delete(obj.DeviceId);
        }

        [Test()]
        public void ClientOwnerAsyncCreateOwnerUpdateDeleteList()
        {
            List<Owner> owners = TestUtils.CreateOwners(5);

            List<Task> createTasks = new List<Task>();
            owners.ForEach((owner) => createTasks.Add(ownerClient.CreateAsync(owner)));
            Task.WaitAll(createTasks.ToArray());

            List<Task> updateTask = new List<Task>();
            owners.ForEach((owner) => updateTask.Add(
                ownerClient.UpdateAsync(TestUtils.CreateOwnerUpdateAttribute(), owner.Username)));
            Task.WaitAll(updateTask.ToArray());

            List<Task> deleteTask = new List<Task>();
            owners.ForEach((owner) => deleteTask.Add(
                ownerClient.DeleteAsync(owner.Username)));
            Task.WaitAll(deleteTask.ToArray());
        }

        [Test()]
        public void ClientOwnerAsyncBatchDelete()
        {
            List<Owner> owners = TestUtils.CreateOwners(5);

            Task<IEnumerable<Result>> resultListTask = ownerClient.CreateUpdateAsync(owners);
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

        private void AssertIfAreDifferent(Owner ownerA, Owner ownerB)
        {
            Assert.AreEqual(ownerA.Username, ownerB.Username);
            Assert.AreEqual(ownerA.RegistrationDate.Value.ToString(EventSerializerTest.DatetimeFormat),
                ownerB.RegistrationDate.Value.ToString(EventSerializerTest.DatetimeFormat));
            CollectionAssert.AreEqual(ownerA.Attributes, ownerB.Attributes);
        }
    }
}