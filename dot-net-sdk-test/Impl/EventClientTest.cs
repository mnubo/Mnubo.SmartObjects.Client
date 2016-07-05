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
    public class EventClientTest
    {
        private readonly SmartObject smartObject;
        private readonly ClientConfig config;
        private readonly ISmartObjectsClient client;
        private readonly IEventClient eventClient;

        public EventClientTest()
        {
            config =
                new ClientConfig.Builder()
                {
                    Environment = ClientConfig.Environments.Sandbox,
                    ConsumerKey = Config.ConsumerKey,
                    ConsumerSecret = Config.ConsumerSecret
                };

            client = ClientFactory.Create(config);
            eventClient = client.Events;

            smartObject = TestUtils.CreateBasicObject();

            client.Objects.Create(smartObject);
        }

        ~EventClientTest()
        {
            if (client != null)
            {
                client.Objects.Delete(smartObject.DeviceId);
            }
        }

        #region Sync Calls
        [Test()]
        public void ClientEventSyncPostFullEvent()
        {
            eventClient.Post(TestUtils.CreateEvents(smartObject.DeviceId, 2));
        }

        [Test()]
        public void ClientEventSyncPostBasicEvent()
        {
            eventClient.Post( 
                new List<Event>()
                {
                    TestUtils.CreateBasicEvent(smartObject.DeviceId),
                    TestUtils.CreateBasicEvent(smartObject.DeviceId)
                });
        }

        [Test()]
        public void ClientEventSyncPostBadRequest()
        {
            
            Assert.That(() => eventClient.Post(
                new List<Event>()
                {
                    TestUtils.CreateBasicEvent(smartObject.DeviceId),
                    TestUtils.CreateEventWrongTimeserie(smartObject.DeviceId)
                }),
               Throws.TypeOf<ArgumentException>()
               .With.Message.EqualTo("Unknown field 'unknown'"));
        }

        [Test()]
        public void ClientEventSyncPostEventExists()
        {
            Event eve = TestUtils.CreateFullEvent(smartObject.DeviceId);

            eventClient.Post(new List<Event>() { eve });

            Assert.AreEqual(true, eventClient.IsEventExists(eve.EventId.Value));
        }

        [Test()]
        public void ClientEventSyncExistsEventNotExists()
        {
            Assert.AreEqual(false, eventClient.IsEventExists(Guid.NewGuid()));
        }

        [Test()]
        public void ClientEventSyncBatchCreateExistsDelete()
        {
            Event eve = TestUtils.CreateFullEvent(smartObject.DeviceId);

            eventClient.Post(new List<Event>() { eve });

            IEnumerable<Dictionary<string, bool>> expected = new List<Dictionary<string, bool>>()
            {
                new Dictionary<string, bool>()
                {
                    {eve.EventId.Value.ToString(), true}
                },
                new Dictionary<string, bool>()
                {
                    {"unknown", false}
                }
            };

            IList<Guid> request = new List<Guid>()
            {
                eve.EventId.Value,
                Guid.NewGuid()
            };

            Assert.AreEqual(expected, eventClient.EventsExist(request));
        }

        public void ClientEventSyncBatchExistBadRequest()
        {
            Assert.That(() => eventClient.EventsExist(null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("List of eventIds cannot be null."));
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
            eventClient.PostAsync(TestUtils.CreateEvents(smartObject.DeviceId, 2)).Wait();
        }

        [Test()]
        public void ClientEventAsyncPostEventExists()
        {
            Event eve = TestUtils.CreateFullEvent(smartObject.DeviceId);

            eventClient.Post(new List<Event>() { eve });

            Task<bool> existTask = eventClient.IsEventExistsAsync(eve.EventId.Value);
            existTask.Wait();

            Assert.AreEqual(true, existTask.Result);
        }

        [Test()]
        public void ClientEventAsyncExistsEventNotExists()
        {
            Task<bool> existTask = eventClient.IsEventExistsAsync(Guid.NewGuid());
            existTask.Wait();

            Assert.AreEqual(false, existTask);
        }

        [Test()]
        public void ClientEventAsyncBatchCreateExistsDelete()
        {
            Event eve = TestUtils.CreateFullEvent(smartObject.DeviceId);

            eventClient.Post(new List<Event>() { eve });

            IEnumerable<Dictionary<string, bool>> expected = new List<Dictionary<string, bool>>()
            {
                new Dictionary<string, bool>()
                {
                    {eve.EventId.Value.ToString(), true}
                },
                new Dictionary<string, bool>()
                {
                    {"unknown", false}
                }
            };

            IList<Guid> request = new List<Guid>()
            {
                eve.EventId.Value,
                Guid.NewGuid()
            };

            Task<IEnumerable<IDictionary<string, bool>>> existTask = eventClient.EventsExistAsync(request);
            existTask.Wait();

            Assert.AreEqual(expected, existTask.Result);
        }

        public void ClientEventAsyncBatchExistBadRequest()
        {
            Assert.That(() => eventClient.EventsExistAsync(null).Wait(),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("List of eventIds cannot be null."));
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

        [Test()]
        public void ClientEventAsyncPostBadRequest()
        {
            Assert.That(() => eventClient.PostAsync(
                new List<Event>()
                {
                    TestUtils.CreateBasicEvent(smartObject.DeviceId),
                    TestUtils.CreateEventWrongTimeserie(smartObject.DeviceId)
                }).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("Unknown field 'unknown'"));
        }
        #endregion
    }
}