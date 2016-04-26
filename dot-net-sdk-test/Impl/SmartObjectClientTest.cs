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
    public class SmartObjectClientTest
    {
        private const string DeviceIdBase = "dot.net.sdk.test";
        private const string ObjectType = "wind_direction";
        private const string Password = "pppp";
        private const string AttributeName = "color";

        private readonly ClientConfig config;
        private readonly ISmartObjectsClient client;
        private readonly IObjectClient objectClient;
        private readonly Random RandomGenerator = new Random();

        public SmartObjectClientTest()
        {
            config =
                new ClientConfig.Builder()
                {
                    Environment = ClientConfig.Environments.Sandbox,
                    ConsumerKey = Config.ConsumerKey,
                    ConsumerSecret = Config.ConsumerSecret
                };
            client = ClientFactory.Create(config);
            objectClient = client.Objects;
        }

        #region Sync calls
        [Test()]
        public void ClientSmartObjectSyncCreateFullObjectUpdateDelete()
        {
            string deviceId = GetRandomDeviceId();
            string username = deviceId;

            Owner owner = new Owner.Builder()
            {
                Username = username,
                Password = Password
            };
            SmartObject smartObject = new SmartObject.Builder()
            {
                DeviceId = deviceId,
                ObjectType = ObjectType,
                Attributes = new Dictionary<string, object>()
                {
                    { AttributeName, "test" }
                },
                RegistrationDate = DateTime.UtcNow,
                EventId = Guid.NewGuid(),
                Username = username
            };

            client.Owners.Create(owner);
            objectClient.Create(smartObject);

            objectClient.Update(new SmartObject.Builder()
            {
                Attributes = new Dictionary<string, object>()
                {
                    { AttributeName, "modified" }
                }
            }, deviceId);

            client.Owners.Delete(username);
            objectClient.Delete(deviceId);
        }

        [Test()]
        public void ClientSmartObjectSyncCreateBasicDelete()
        {
            string deviceId = GetRandomDeviceId();

            SmartObject smartObject = new SmartObject.Builder()
            {
                DeviceId = deviceId,
                ObjectType = ObjectType
            };

            objectClient.Create(smartObject);

            objectClient.Delete(deviceId);
        }

        [Test()]
        public void ClientSmartObjectSyncBatchCreateDelete()
        {
            List<SmartObject> objs = new List<SmartObject>();

            for (int i = 1; i <= 200; i++)
            {
                objs.Add(new SmartObject.Builder()
                {
                    DeviceId = GetRandomDeviceId(),
                    ObjectType = ObjectType
                });
            }

            IEnumerable<Result> objectResult = objectClient.CreateUpdate(objs);

            Assert.AreEqual(objectResult.Count(), 200);

            foreach (SmartObject obj in objs)
            {
                objectClient.Delete(obj.DeviceId);
            }
        }

        [Test()]
        public void ClientSmartObjectSyncCreateBadRequest()
        {
            string deviceId = GetRandomDeviceId();

            SmartObject smartObject = new SmartObject.Builder()
            {
                DeviceId = deviceId,
                ObjectType = ObjectType,
                Attributes = new Dictionary<string, object>()
                {
                    { "Unknow", "5" }
                }
            };

            Assert.That(() => objectClient.Create(smartObject),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Unknown field 'unknow'"));
        }

        [Test()]
        public void ClientSmartObjectSyncUpdateBadRequest()
        {
            Assert.That(() => objectClient.Update(new SmartObject.Builder().Build(), "Unknown"),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Object with x_device_id 'Unknown' not found."));
        }

        [Test()]
        public void ClientSmartObjectSyncDeleteBadRequest()
        {
            Assert.That(() => objectClient.Delete("Unknown"),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Object with x_device_id 'Unknown' not found."));
        }

        [Test()]
        public void ClientSmartObjectSyncCreateNullBody()
        {
            Assert.That(() => objectClient.Create(null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("SmartObject body cannot be null."));
        }

        [Test()]
        public void ClientSmartObjectSyncCreateNotDeviceID()
        {
            SmartObject smartObject = new SmartObject.Builder().Build();

            Assert.That(() => objectClient.Create(smartObject),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("x_device_id cannot be blank."));
        }

        [TestCase("", "x_device_id cannot be blank.", "ObjectType")]
        [TestCase(null, "x_device_id cannot be blank.", "ObjectType")]
        [TestCase("deviceId", "x_object_type cannot be blank.", "")]
        [TestCase("deviceId", "x_object_type cannot be blank.", null)]
        public void ClientSmartObjectSyncCreateEmptyDeviceID(string deviceId, string errorMessage, string objectType)
        {
            SmartObject smartObject = new SmartObject.Builder()
            {
                DeviceId = deviceId,
                ObjectType = objectType
            };

            Assert.That(() => objectClient.Create(smartObject),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo(errorMessage));
        }
        [Test()]
        public void ClientSmartObjectSyncDeleteEmptyDeviceId()
        {
            Assert.That(() => objectClient.Delete(""),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("x_device_id cannot be blank."));
        }

        [Test()]
        public void ClientSmartObjectSyncDeleteNullDeviceId()
        {
            Assert.That(() => objectClient.Delete(null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("x_device_id cannot be blank."));
        }

        [Test()]
        public void ClientSmartObjectSyncUpdateNullBody()
        {
            Assert.That(() => objectClient.Update(null, "Something"),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("SmartObject body cannot be null."));
        }

        [Test()]
        public void ClientSmartObjectSyncUpdateEmptyDeviceID()
        {
            Assert.That(() => objectClient.Update(new SmartObject.Builder().Build(), ""),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("x_device_id cannot be blank."));
        }

        [Test()]
        public void ClientSmartObjectSyncUpdateNullDeviceID()
        {
            Assert.That(() => objectClient.Update(new SmartObject.Builder().Build(), null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("x_device_id cannot be blank."));
        }

        [Test()]
        public void ClientSmartObjectSyncBatchNullList()
        {
            Assert.That(() => objectClient.CreateUpdate(null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("object body list cannot be null."));
        }

        [Test()]
        public void ClientSmartObjectSyncBatchEmptyList()
        {
            Assert.That(() => objectClient.CreateUpdate(new List<SmartObject>()),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("object body list cannot be empty or biger that 1000."));
        }

        [Test()]
        public void ClientSmartObjectSyncBatchMaxSizeList()
        {
            List<SmartObject> objs = new List<SmartObject>();

            for (int i = 1; i <= 1001; i++)
            {
                objs.Add(new SmartObject.Builder().Build());
            }

            Assert.That(() => objectClient.CreateUpdate(objs),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("object body list cannot be empty or biger that 1000."));
        }

        [Test()]
        public void ClientSmartObjectSyncBatchNullUsername()
        {
            List<SmartObject> objs = new List<SmartObject>();

            for (int i = 1; i <= 2; i++)
            {
                objs.Add(new SmartObject.Builder().Build());
            }

            Assert.That(() => objectClient.CreateUpdate(objs),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("x_device_id cannot be blank."));
        }
        #endregion

        #region Async calls
        [Test()]
        public void ClientSmartObjectAsyncCreateUpdateDelete()
        {
            List<SmartObject> objs = new List<SmartObject>();
            List<Task> createTasks = new List<Task>();
            for (int i = 1; i <= 5; i++)
            {
                objs.Add(new SmartObject.Builder()
                {
                    DeviceId = GetRandomDeviceId(),
                    ObjectType = ObjectType,
                    Attributes = new Dictionary<string, object>()
                    {
                        { AttributeName, "test" }
                    },
                    RegistrationDate = DateTime.UtcNow
                });
            }

            objs.ForEach((obj) => createTasks.Add(objectClient.CreateAsync(obj)));
            Task.WaitAll(createTasks.ToArray());

            List<Task> updateTask = new List<Task>();
            objs.ForEach((obj) => updateTask.Add(
                objectClient.UpdateAsync(
                    new SmartObject.Builder()
                    {
                        Attributes = new Dictionary<string, object>()
                        {
                            { AttributeName, "modified" }
                        }
                    }, obj.DeviceId)));
            Task.WaitAll(updateTask.ToArray());

            List<Task> deleteTask = new List<Task>();
            objs.ForEach((obj) => deleteTask.Add(
                objectClient.DeleteAsync(obj.DeviceId)));
            Task.WaitAll(deleteTask.ToArray());
        }

        [Test()]
        public void ClientSmartObjectAsyncBatchDelete()
        {
            List<SmartObject> objs = new List<SmartObject>();
            Dictionary<Task<SmartObject>, SmartObject> createTasks = new Dictionary<Task<SmartObject>, SmartObject>();
            for (int i = 1; i <= 5; i++)
            {
                objs.Add(new SmartObject.Builder()
                {
                    DeviceId = GetRandomDeviceId(),
                    ObjectType = ObjectType,
                    Attributes = new Dictionary<string, object>()
                    {
                        { AttributeName, "test" }
                    },
                    RegistrationDate = DateTime.UtcNow
                });
            }

            Task<IEnumerable<Result>> resultListTask = objectClient.CreateUpdateAsync(objs);
            resultListTask.Wait();

            List<Task> deleteTask = new List<Task>();
            objs.ForEach((obj) => deleteTask.Add(
                objectClient.DeleteAsync(obj.DeviceId)));
            Task.WaitAll(deleteTask.ToArray());
        }

        [Test()]
        public void ClientSmartObjectAsyncCreateNullBody()
        {
            Assert.That(() => objectClient.CreateAsync(null).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("SmartObject body cannot be null."));
        }

        [Test()]
        public void ClientSmartObjectAsyncCreateNotDeviceID()
        {
            SmartObject smartObject = new SmartObject.Builder().Build();

            Assert.That(() => objectClient.CreateAsync(smartObject).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("x_device_id cannot be blank."));
        }

        [TestCase("", "x_device_id cannot be blank.", "ObjectType")]
        [TestCase(null, "x_device_id cannot be blank.", "ObjectType")]
        [TestCase("deviceId", "x_object_type cannot be blank.", "")]
        [TestCase("deviceId", "x_object_type cannot be blank.", null)]
        public void ClientSmartObjectAsyncCreateEmptyDeviceID(string deviceId, string errorMessage, string objectType)
        {
            SmartObject smartObject = new SmartObject.Builder()
            {
                DeviceId = deviceId,
                ObjectType = objectType
            };

            Assert.That(() => objectClient.CreateAsync(smartObject).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo(errorMessage));
        }

        [Test()]
        public void ClientSmartObjectAsyncDeleteEmptyDeviceId()
        {
            Assert.That(() => objectClient.DeleteAsync("").Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("x_device_id cannot be blank."));
        }

        [Test()]
        public void ClientSmartObjectAsyncDeleteNullDeviceId()
        {
            Assert.That(() => objectClient.DeleteAsync(null).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("x_device_id cannot be blank."));
        }

        [Test()]
        public void ClientSmartObjectAsyncUpdateNullBody()
        {
            Assert.That(() => objectClient.UpdateAsync(null, "Something").Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("SmartObject body cannot be null."));
        }

        [Test()]
        public void ClientSmartObjectAsyncUpdateEmptyDeviceID()
        {
            Assert.That(() => objectClient.UpdateAsync(new SmartObject.Builder().Build(), "").Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("x_device_id cannot be blank."));
        }

        [Test()]
        public void ClientSmartObjectAsyncUpdateNullDeviceID()
        {
            Assert.That(() => objectClient.UpdateAsync(new SmartObject.Builder().Build(), null).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("x_device_id cannot be blank."));
        }

        [Test()]
        public void ClientSmartObjectAsyncBatchNullList()
        {
            Assert.That(() => objectClient.CreateUpdateAsync(null).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("object body list cannot be null."));
        }

        [Test()]
        public void ClientSmartObjectAsyncBatchEmptyList()
        {
            Assert.That(() => objectClient.CreateUpdateAsync(new List<SmartObject>()).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("object body list cannot be empty or biger that 1000."));
        }

        [Test()]
        public void ClientSmartObjectAsyncBatchMaxSizeList()
        {
            List<SmartObject> objs = new List<SmartObject>();

            for (int i = 1; i <= 1001; i++)
            {
                objs.Add(new SmartObject.Builder().Build());
            }

            Assert.That(() => objectClient.CreateUpdateAsync(objs).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("object body list cannot be empty or biger that 1000."));
        }

        [Test()]
        public void ClientSmartObjectAsyncBatchNullUsername()
        {
            List<SmartObject> objs = new List<SmartObject>();

            for (int i = 1; i <= 2; i++)
            {
                objs.Add(new SmartObject.Builder().Build());
            }

            Assert.That(() => objectClient.CreateUpdateAsync(objs).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("x_device_id cannot be blank."));
        }
        #endregion

        private String GetRandomDeviceId()
        {
            return DeviceIdBase + RandomGenerator.Next(1000000).ToString() + RandomGenerator.Next(1000000).ToString();
        }

        private void AssertIfAreDifferent(SmartObject smartObjectA, SmartObject smartObjectB)
        {
            Assert.AreEqual(smartObjectA.DeviceId, smartObjectB.DeviceId);
            Assert.AreEqual(smartObjectA.ObjectType, smartObjectB.ObjectType);
            Assert.AreEqual(smartObjectA.RegistrationDate.Value.ToString(EventSerializerTest.DatetimeFormat),
                smartObjectB.RegistrationDate.Value.ToString(EventSerializerTest.DatetimeFormat));
            CollectionAssert.AreEqual(smartObjectA.Attributes, smartObjectB.Attributes);
        }
    }
}