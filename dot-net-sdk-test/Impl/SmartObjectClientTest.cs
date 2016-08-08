using System;
using NUnit.Framework;
using Mnubo.SmartObjects.Client.Impl;
using Mnubo.SmartObjects.Client.Config;
using Mnubo.SmartObjects.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Nancy.Hosting.Self;

namespace Mnubo.SmartObjects.Client.Test.Impl
{
    [TestFixture()]
    public class SmartObjectClientTest
    {
        private readonly ClientConfig config;
        private readonly NancyHost nancy;
        private readonly int port;

        public SmartObjectClientTest()
        {
            var configuration = new HostConfiguration() { UrlReservations = new UrlReservations() { CreateAutomatically = true } };
            port = TestUtils.FreeTcpPort();

            nancy = new NancyHost(configuration, new Uri(string.Format("http://localhost:{0}", port)));
            nancy.Start();
            System.Diagnostics.Debug.WriteLine(string.Format("Nancy has been started on host 'http://localhost:{0}'", port));

            config =
                new ClientConfig.Builder()
                {
                    Environment = ClientConfig.Environments.Sandbox,
                    ConsumerKey = "key",
                    ConsumerSecret = "secret"
                };
        }

        [OneTimeTearDown]
        public void Dispose()
        {
            nancy.Stop();
        }

        #region Sync Create Calls
        [Test()]
        public void ClientSmartObjectSyncCreate()
        {
            withSuccessfulResults(client =>
            {
                client.Create(TestUtils.CreateTestObject());
            });
        }

        [Test()]
        public void ClientSmartObjectSyncCreateBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.Create(TestUtils.CreateTestObject()),
                    Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [Test()]
        public void ClientSmartObjectSyncCreateNullBody()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.Create(null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("SmartObject body cannot be null."));
            });
        }

        [Test()]
        public void ClientSmartObjectSyncCreateNotDeviceID()
        {
            withSuccessfulResults(client =>
            {
                SmartObject smartObject = new SmartObject.Builder().Build();

                Assert.That(() => client.Create(smartObject),
                    Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo("x_device_id cannot be blank."));
            });
        }

        [TestCase("", "x_device_id cannot be blank.", "ObjectType")]
        [TestCase(null, "x_device_id cannot be blank.", "ObjectType")]
        [TestCase("deviceId", "x_object_type cannot be blank.", "")]
        [TestCase("deviceId", "x_object_type cannot be blank.", null)]
        public void ClientSmartObjectSyncCreateEmptyDeviceID(string deviceId, string errorMessage, string objectType)
        {
            withSuccessfulResults(client =>
            {
                SmartObject smartObject = new SmartObject.Builder()
                {
                    DeviceId = deviceId,
                    ObjectType = objectType
                };

                Assert.That(() => client.Create(smartObject),
                    Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo(errorMessage));
            });
        }

        #endregion

        #region Sync Update Calls

        [Test()]
        public void ClientSmartObjectSyncUpdate()
        {
            withSuccessfulResults(client =>
            {
                client.Update(TestUtils.CreateObjectUpdateAttribute(), TestUtils.DeviceId);
            });
        }

        [Test()]
        public void ClientSmartObjectSyncUpdateBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.Update(TestUtils.CreateObjectUpdateAttribute(), TestUtils.DeviceId),
                    Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [Test()]
        public void ClientSmartObjectSyncUpdateNullBody()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.Update(null, "Something"),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("SmartObject body cannot be null."));
            });
        }

        [Test()]
        public void ClientSmartObjectSyncUpdateEmptyDeviceID()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.Update(new SmartObject.Builder().Build(), ""),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("x_device_id cannot be blank."));
            });
        }

        [Test()]
        public void ClientSmartObjectSyncUpdateNullDeviceID()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.Update(new SmartObject.Builder().Build(), null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("x_device_id cannot be blank."));
            });
        }

        #endregion

        #region Sync Delete Calls

        [Test()]
        public void ClientSmartObjectSyncDelete()
        {
            withSuccessfulResults(client =>
            {
                client.Delete(TestUtils.DeviceId);
            });
        }

        [Test()]
        public void ClientSmartObjectSyncDeleteBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.Delete(TestUtils.DeviceId),
                    Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [TestCase(null)]
        [TestCase("")]
        public void ClientSmartObjectSyncDeleteEmptyOrNullDeviceId(string deviceId)
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.Delete(deviceId),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("x_device_id cannot be blank."));
            });
        }

        #endregion

        #region Sync CreateUpdate Calls
        [Test()]
        public void ClientSmartObjectSyncBatchCreate()
        {
            IEnumerable<Result> expected = new List<Result>()
            {
                new Result("deviceid0",Result.ResultStates.Success,null),
                new Result("deviceid1",Result.ResultStates.Success,null),
                new Result("deviceid2",Result.ResultStates.Success,null),
                new Result("deviceid3",Result.ResultStates.Success,null),
                new Result("deviceid4",Result.ResultStates.Success,null)

            };
            withSuccessfulResults(client =>
            {
                IEnumerable<Result> actual = client.CreateUpdate(TestUtils.CreateObjects(5));
                CollectionAssert.AreEqual(expected, actual);
            });
        }

        [Test()]
        public void ClientSmartObjectSyncBatchCreateBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.CreateUpdate(TestUtils.CreateObjects(5)),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [Test()]
        public void ClientSmartObjectSyncBatchNullList()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.CreateUpdate(null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("object body list cannot be null."));
            });
        }

        [Test()]
        public void ClientSmartObjectSyncBatchEmptyList()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.CreateUpdate(new List<SmartObject>()),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("object body list cannot be empty or biger that 1000."));
            });
        }

        [Test()]
        public void ClientSmartObjectSyncBatchMaxSizeList()
        {
            withSuccessfulResults(client =>
            {
                List<SmartObject> objs = new List<SmartObject>();

                for (int i = 1; i <= 1001; i++)
                {
                    objs.Add(new SmartObject.Builder().Build());
                }

                Assert.That(() => client.CreateUpdate(objs),
                    Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo("object body list cannot be empty or biger that 1000."));
            });
        }

        [Test()]
        public void ClientSmartObjectSyncBatchNullUsername()
        {
            withSuccessfulResults(client =>
            {
                List<SmartObject> objs = new List<SmartObject>();

                for (int i = 1; i <= 2; i++)
                {
                    objs.Add(new SmartObject.Builder().Build());
                }

                Assert.That(() => client.CreateUpdate(objs),
                    Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo("x_device_id cannot be blank."));
            });
        }

        #endregion

        #region Sync Exists Calls

        [Test()]
        public void ClientObjectSyncExists()
        {
            withSuccessfulResults(client =>
            {
                Assert.IsTrue(client.ObjectExists(TestUtils.DeviceId));
            });
        }

        [Test()]
        public void ClientObjectSyncExistsObjectNotExists()
        {
            withSuccessfulAndFailedResults(client =>
            {
                Assert.IsTrue(!client.ObjectExists(TestUtils.DeviceId));
            });
        }

        [Test()]
        public void ClientObjectSyncExistsBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.ObjectExists(TestUtils.DeviceId),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [TestCase("")]
        [TestCase(null)]
        public void ClientObjectSyncExistNullOrBlankDeviceId(string deviceId)
        {
            withSuccessfulAndFailedResults(client =>
            {
                Assert.That(() => client.ObjectExists(deviceId),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("deviceId cannot be blank."));
            });
        }

        [Test()]
        public void ClientObjectSyncBatchExists()
        {
            IList<string> input = new List<string>() { "deviceid0", "deviceid1", "deviceid2", "deviceid3" };
            IEnumerable<IDictionary<string, bool>> expectedResults = new List<Dictionary<string, bool>>()
            {
                new Dictionary<string, bool>() { { input[0].ToString(), true } },
                new Dictionary<string, bool>() { { input[1].ToString(), false } },
                new Dictionary<string, bool>() { { input[2].ToString(), true } },
                new Dictionary<string, bool>() { { input[3].ToString(), false } }
            };
            withSuccessfulAndFailedResults(client =>
            {
                CollectionAssert.AreEqual(expectedResults, client.ObjectsExist(input));
            });
        }

        [Test()]
        public void ClientObjectSyncBatchExistBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.ObjectsExist(new List<string>() { "deviceid0", "deviceid1", "deviceid2", "deviceid3" }),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        #endregion

        #region Async Create Calls
        [Test()]
        public void ClientSmartObjectAsyncCreate()
        {
            withSuccessfulResults(client =>
            {
                client.CreateAsync(TestUtils.CreateTestObject()).Wait();
            });
        }

        [Test()]
        public void ClientSmartObjectAsyncCreateBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.CreateAsync(TestUtils.CreateTestObject()).Wait(),
                    Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [Test()]
        public void ClientSmartObjectAsyncCreateNullBody()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.CreateAsync(null).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("SmartObject body cannot be null."));
            });
        }

        [Test()]
        public void ClientSmartObjectAsyncCreateNotDeviceID()
        {
            withSuccessfulResults(client =>
            {
                SmartObject smartObject = new SmartObject.Builder().Build();

                Assert.That(() => client.CreateAsync(smartObject).Wait(),
                    Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("x_device_id cannot be blank."));
            });
        }

        [TestCase("", "x_device_id cannot be blank.", "ObjectType")]
        [TestCase(null, "x_device_id cannot be blank.", "ObjectType")]
        [TestCase("deviceId", "x_object_type cannot be blank.", "")]
        [TestCase("deviceId", "x_object_type cannot be blank.", null)]
        public void ClientSmartObjectAsyncCreateEmptyDeviceID(string deviceId, string errorMessage, string objectType)
        {
            withSuccessfulResults(client =>
            {
                SmartObject smartObject = new SmartObject.Builder()
                {
                    DeviceId = deviceId,
                    ObjectType = objectType
                };

                Assert.That(() => client.CreateAsync(smartObject).Wait(),
                    Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo(errorMessage));
            });
        }

        #endregion

        #region Async Update Calls

        [Test()]
        public void ClientSmartObjectAsyncUpdate()
        {
            withSuccessfulResults(client =>
            {
                client.UpdateAsync(TestUtils.CreateObjectUpdateAttribute(), TestUtils.DeviceId).Wait();
            });
        }

        [Test()]
        public void ClientSmartObjectAsyncUpdateBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.UpdateAsync(TestUtils.CreateObjectUpdateAttribute(), TestUtils.DeviceId).Wait(),
                    Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [Test()]
        public void ClientSmartObjectAsyncUpdateNullBody()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.UpdateAsync(null, "Something").Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("SmartObject body cannot be null."));
            });
        }

        [Test()]
        public void ClientSmartObjectAsyncUpdateEmptyDeviceID()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.UpdateAsync(new SmartObject.Builder().Build(), "").Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("x_device_id cannot be blank."));
            });
        }

        [Test()]
        public void ClientSmartObjectAsyncUpdateNullDeviceID()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.UpdateAsync(new SmartObject.Builder().Build(), null).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("x_device_id cannot be blank."));
            });
        }

        #endregion

        #region Async Delete Calls

        [Test()]
        public void ClientSmartObjectAsyncDelete()
        {
            withSuccessfulResults(client =>
            {
                client.DeleteAsync(TestUtils.DeviceId).Wait();
            });
        }

        [Test()]
        public void ClientSmartObjectAsyncDeleteBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.DeleteAsync(TestUtils.DeviceId).Wait(),
                    Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [TestCase(null)]
        [TestCase("")]
        public void ClientSmartObjectAsyncDeleteEmptyOrNullDeviceId(string deviceId)
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.DeleteAsync(deviceId).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("x_device_id cannot be blank."));
            });
        }

        #endregion

        #region Async CreateUpdate Calls
        [Test()]
        public void ClientSmartObjectAsyncBatchCreate()
        {
            IEnumerable<Result> expected = new List<Result>()
            {
                new Result("deviceid0",Result.ResultStates.Success,null),
                new Result("deviceid1",Result.ResultStates.Success,null),
                new Result("deviceid2",Result.ResultStates.Success,null),
                new Result("deviceid3",Result.ResultStates.Success,null),
                new Result("deviceid4",Result.ResultStates.Success,null)

            };
            withSuccessfulResults(client =>
            {
                Task<IEnumerable<Result>> resultTask = client.CreateUpdateAsync(TestUtils.CreateObjects(5));
                resultTask.Wait();
                CollectionAssert.AreEqual(expected, resultTask.Result);
            });
        }

        [Test()]
        public void ClientSmartObjectAsyncBatchCreateBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.CreateUpdateAsync(TestUtils.CreateObjects(5)).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [Test()]
        public void ClientSmartObjectAsyncBatchNullList()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.CreateUpdateAsync(null).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("object body list cannot be null."));
            });
        }

        [Test()]
        public void ClientSmartObjectAsyncBatchEmptyList()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.CreateUpdateAsync(new List<SmartObject>()).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("object body list cannot be empty or biger that 1000."));
            });
        }

        [Test()]
        public void ClientSmartObjectAsyncBatchMaxSizeList()
        {
            withSuccessfulResults(client =>
            {
                List<SmartObject> objs = new List<SmartObject>();

                for (int i = 1; i <= 1001; i++)
                {
                    objs.Add(new SmartObject.Builder().Build());
                }

                Assert.That(() => client.CreateUpdateAsync(objs).Wait(),
                    Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("object body list cannot be empty or biger that 1000."));
            });
        }

        [Test()]
        public void ClientSmartObjectAsyncBatchNullUsername()
        {
            withSuccessfulResults(client =>
            {
                List<SmartObject> objs = new List<SmartObject>();

                for (int i = 1; i <= 2; i++)
                {
                    objs.Add(new SmartObject.Builder().Build());
                }

                Assert.That(() => client.CreateUpdateAsync(objs).Wait(),
                    Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("x_device_id cannot be blank."));
            });
        }

        #endregion

        #region Async Exists Calls

        [Test()]
        public void ClientObjectAsyncExists()
        {
            withSuccessfulResults(client =>
            {
                Task<bool> resultTask = client.ObjectExistsAsync(TestUtils.DeviceId);
                resultTask.Wait();
                Assert.IsTrue(resultTask.Result);
            });
        }

        [Test()]
        public void ClientObjectAsyncExistsObjectNotExists()
        {
            withSuccessfulAndFailedResults(client =>
            {
                Task<bool> resultTask = client.ObjectExistsAsync(TestUtils.DeviceId);
                resultTask.Wait();
                Assert.IsTrue(!resultTask.Result);
            });
        }

        [Test()]
        public void ClientObjectAsyncExistsBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.ObjectExistsAsync(TestUtils.DeviceId).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [TestCase("")]
        [TestCase(null)]
        public void ClientObjectAsyncExistNullOrBlankDeviceId(string deviceId)
        {
            withSuccessfulAndFailedResults(client =>
            {
                Assert.That(() => client.ObjectExistsAsync(deviceId).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("deviceId cannot be blank."));
            });
        }

        [Test()]
        public void ClientObjectAsyncBatchExists()
        {
            IList<string> input = new List<string>() { "deviceid0", "deviceid1", "deviceid2", "deviceid3" };
            IEnumerable<IDictionary<string, bool>> expectedResults = new List<Dictionary<string, bool>>()
            {
                new Dictionary<string, bool>() { { input[0].ToString(), true } },
                new Dictionary<string, bool>() { { input[1].ToString(), false } },
                new Dictionary<string, bool>() { { input[2].ToString(), true } },
                new Dictionary<string, bool>() { { input[3].ToString(), false } }
            };
            withSuccessfulAndFailedResults(client =>
            {
                Task<IEnumerable<IDictionary<string, bool>>> resultsTask = client.ObjectsExistAsync(input);
                resultsTask.Wait();
                CollectionAssert.AreEqual(expectedResults, resultsTask.Result);
            });
        }

        [Test()]
        public void ClientObjectAsyncBatchExistBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.ObjectsExistAsync(new List<string>() { "deviceid0", "deviceid1", "deviceid2", "deviceid3" }).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        #endregion
        
        #region Utils
        private void AssertIfAreDifferent(SmartObject smartObjectA, SmartObject smartObjectB)
        {
            Assert.AreEqual(smartObjectA.DeviceId, smartObjectB.DeviceId);
            Assert.AreEqual(smartObjectA.ObjectType, smartObjectB.ObjectType);
            Assert.AreEqual(smartObjectA.RegistrationDate.Value.ToString(EventSerializerTest.DatetimeFormat),
                smartObjectB.RegistrationDate.Value.ToString(EventSerializerTest.DatetimeFormat));
            CollectionAssert.AreEqual(smartObjectA.Attributes, smartObjectB.Attributes);
        }

        //Spawn a client which respond successfully to all events
        internal void withSuccessfulResults(Action<IObjectClient> test)
        {
            var httpClient = new HttpClient(config, "http", "localhost", port, SucceedAPIsMockModule.BasePath);
            test(new ObjectClient(httpClient));
        }

        //Spawn a client which fail all request
        internal void withFailedResults(Action<IObjectClient> test)
        {
            var httpClient = new HttpClient(config, "http", "localhost", port, FailedAPIsMockModule.BasePath);
            test(new ObjectClient(httpClient));
        }

        //Spawn a client which fail respond with a mix of failed and successful events
        internal void withSuccessfulAndFailedResults(Action<IObjectClient> test)
        {
            var httpClient = new HttpClient(config, "http", "localhost", port, BatchAPIsMockModule.BasePath);
            test(new ObjectClient(httpClient));
        }
        #endregion
    }

    
}