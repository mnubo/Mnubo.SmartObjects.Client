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