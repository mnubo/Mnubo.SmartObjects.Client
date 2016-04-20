using System;
using System.Collections.Generic;
using NUnit.Framework;
using Mnubo.SmartObjects.Client.Impl;
using Mnubo.SmartObjects.Client.Config;
using Mnubo.SmartObjects.Client.Models;
using System.Threading.Tasks;

namespace Mnubo.SmartObjects.Client.Test.Impl
{
    [TestFixture()]
//    [Ignore("Ignore a fixture")]
    public class EventClientTest
    {
        private const string Password = "pppp";
        private const string ObjectType = "wind_direction";
        private const string DeviceIdBase = "dot.net.sdk.test";
        private const string EventType = "wind_direction";
        private const string AttributeName = "wind_direction";
        private SmartObject smartObject;
        private string deviceId;
        private ClientConfig config =
                new ClientConfig.Builder()
                {
                    Environment = ClientConfig.Environments.Sandbox,
                    ConsumerKey = Config.ConsumerKey,
                    ConsumerSecret = Config.ConsumerSecret
                };
        private ISmartObjectsClient client;
        private IEventClient eventClient;

        public EventClientTest()
        {
            client = ClientFactory.Create(config);
            eventClient = client.Events;

            Random rnd = new Random();
            deviceId = DeviceIdBase + rnd.Next(1000).ToString();

            smartObject = new SmartObject.Builder()
            {
                DeviceId = deviceId,
                ObjectType = ObjectType
            };

            client.Objects.Create(smartObject);
        }

        ~EventClientTest()
        {
            client.Objects.Delete(smartObject.DeviceId);
        }

        #region Sync Calls
        [Test()]
        public void ClientEventSyncPostFullEvent()
        {
            Event eve1 = new Event.Builder()
            {
                EventType = EventType,
                DeviceId = deviceId,
                Timeseries = new Dictionary<string, object>()
                {
                    { AttributeName, "testA" }
                },
                Timestamp = DateTime.UtcNow,
                EventId = Guid.NewGuid()
            };

            Event eve2 = new Event.Builder()
            {
                EventType = EventType,
                DeviceId = deviceId,
                Timeseries = new Dictionary<string, object>()
                {
                    { AttributeName, "testB" }
                },
                Timestamp = DateTime.UtcNow,
                EventId = Guid.NewGuid()
            };

            eventClient.Post(new List<Event>() { eve1, eve2 });
        }

        [Test()]
        public void ClientEventSyncPostBasicEvent()
        {
            Event eve1 = new Event.Builder()
            {
                EventType = EventType,
                DeviceId = deviceId,
                Timeseries = new Dictionary<string, object>()
                {
                    { AttributeName, "testA" }
                }
            };

            Event eve2 = new Event.Builder()
            {
                EventType = EventType,
                DeviceId = deviceId,
                Timeseries = new Dictionary<string, object>()
                {
                    { AttributeName, "testB" }
                }
            };

            eventClient.Post(new List<Event>() { eve1, eve2 });
        }

        [Test()]
        public void ClientEventSyncPostBadRequest()
        {
            Event eve1 = new Event.Builder()
            {
                EventType = EventType,
                DeviceId = deviceId,
                Timeseries = new Dictionary<string, object>()
                {
                    { AttributeName, "testA" }
                }
            };

            Event eve2 = new Event.Builder()
            {
                EventType = EventType,
                DeviceId = deviceId,
                Timeseries = new Dictionary<string, object>()
                {
                    { "Unknown", "testB" }
                }
            };

            Assert.That(() => eventClient.Post(new List<Event>() { eve1, eve2 }),
               Throws.TypeOf<InvalidOperationException>()
               .With.Message.Contains("status code: BadRequest, message {\"path\":\"/events\",\"errorCode\":400,\"requestId\":")
               .With.Message.Contains("\"message\":\"Unknown field 'unknown'\"}"));
        }

        [Test()]
        public void ClientEventSyncNullEventList()
        {
            Assert.That(() => eventClient.Post(null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Event list cannot be empty or null."));
        }

        [Test()]
        public void ClientEventSyncEmptyEventList()
        {
            Assert.That(() => eventClient.Post(new List<Event>()),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Event list cannot be empty or null."));
        }
        #endregion

        #region Async Calls
        [Test()]
        public void ClientEventAsyncPostFullEvent()
        {
            Event eve1 = new Event.Builder()
            {
                EventType = EventType,
                DeviceId = deviceId,
                Timeseries = new Dictionary<string, object>()
                {
                    { AttributeName, "testA" }
                },
                Timestamp = DateTime.UtcNow,
                EventId = Guid.NewGuid()
            };

            Event eve2 = new Event.Builder()
            {
                EventType = EventType,
                DeviceId = deviceId,
                Timeseries = new Dictionary<string, object>()
                {
                    { AttributeName, "testB" }
                },
                Timestamp = DateTime.UtcNow,
                EventId = Guid.NewGuid()
            };

            Task task = eventClient.PostAsync(new List<Event>() { eve1, eve2 });
            task.Wait();
        }

        [Test()]
        public void ClientEventAsyncNullEventList()
        {
            Assert.That(() => eventClient.PostAsync(null).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("Event list cannot be empty or null."));
        }

        [Test()]
        public void ClientEventAsyncEmptyEventList()
        {
            Assert.That(() => eventClient.PostAsync(new List<Event>()).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("Event list cannot be empty or null."));
        }
        #endregion
    }
}