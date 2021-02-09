using System;
using System.Collections.Generic;
using NUnit.Framework;
using Mnubo.SmartObjects.Client.Impl;
using Mnubo.SmartObjects.Client.Config;
using Mnubo.SmartObjects.Client.Models;
using System.Threading.Tasks;
using Nancy.Hosting.Self;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Nancy.Diagnostics;
using Newtonsoft.Json;
using Nancy.IO;
using System.Net.Sockets;
using System.Net;
using System.Linq;

namespace Mnubo.SmartObjects.Client.Test.Impl
{
    [TestFixture()]
    public class EventClientTest
    {
        private readonly ClientConfig config;
        private readonly NancyHost nancy;
        private readonly int port;

        public EventClientTest()
        {
            var configuration = new HostConfiguration() { UrlReservations = new UrlReservations() { CreateAutomatically = true } };
            port = TestUtils.FreeTcpPort();

            nancy = new NancyHost(configuration, new Uri(string.Format("http://localhost:{0}", port)));
            nancy.Start();
            System.Diagnostics.Debug.WriteLine(string.Format("Nancy has been started on host 'http://localhost:{0}'", port));

            config =
                new ClientConfig.Builder()
                {
                    Hostname = ClientConfig.Environments.Sandbox,
                    ConsumerKey = "key",
                    ConsumerSecret = "secret"
                };          
        }

        [OneTimeTearDown]
        public void Dispose()
        {
            nancy.Stop();
        }

        #region Sync Calls
        [Test()]
        public void ClientEventSyncSendFullEvent()
        {
            withSuccessfulResults(client => 
            {
                IEnumerable<EventResult> expected;
                var actual = client.Send(TestUtils.CreateEventsAndSuccessfulResults("deviceId", 2, out expected));
                CollectionAssert.AreEqual(expected, actual);
            });
        }

        [Test()]
        public void ClientEventSyncSendBasicEvent()
        {
            withSuccessfulResults(client =>
            {
                IEnumerable<EventResult> expected = new List<EventResult>()
                {
                    new EventResult(Guid.Empty, true, EventResult.ResultStates.success, null),
                    new EventResult(Guid.Empty, true, EventResult.ResultStates.success, null),
                };

                var actual = client.Send(
                    new List<Event>()
                    {
                        TestUtils.CreateBasicEvent("deviceId"),
                        TestUtils.CreateBasicEvent("deviceId")
                    });

                CollectionAssert.AreEqual(expected, actual, new EventResultWithIgnoredIdComparer());
            });  
        }

        [Test()]
        public void ClientEventSyncSendWithBadRequest()
        {
            withFailedResults(client =>
            {
                IEnumerable<EventResult> expected = new List<EventResult>()
                {
                    new EventResult(Guid.Empty, true, EventResult.ResultStates.success, null),
                    new EventResult(Guid.Empty, true, EventResult.ResultStates.success, null),
                };

                Assert.That(() => client.Send(
                    new List<Event>()
                    {
                        TestUtils.CreateBasicEvent("deviceId"),
                        TestUtils.CreateBasicEvent("deviceId")
                    }),
                    Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [Test()]
        public void ClientEventSyncSendWithMultiResults()
        {
            withSuccessfulAndFailedResults(client =>
            {
                Guid eventId = Guid.NewGuid();
                IEnumerable<EventResult> expected = new List<EventResult>()
                {
                    new EventResult(eventId, true, EventResult.ResultStates.conflict, null),
                    new EventResult(Guid.Empty, true, EventResult.ResultStates.success, null),
                    new EventResult(Guid.Empty, false, EventResult.ResultStates.error, "Event Failed"),
                    new EventResult(Guid.Empty, false, EventResult.ResultStates.success, null)
                };

                var actual = client.Send(
                    new List<Event>()
                    {
                        TestUtils.CreateEventWithEventId("deviceId", eventId),
                        TestUtils.CreateBasicEvent("deviceId"),
                        TestUtils.CreateBasicEvent("deviceId"),
                        TestUtils.CreateBasicEvent("deviceId")
                    });

                Assert.AreEqual(4, actual.Count<EventResult>());
                Assert.AreEqual(new EventResult(eventId, true, EventResult.ResultStates.conflict, null), actual.ElementAt<EventResult>(0));
                Assert.IsTrue(EventResultWithIgnoredIdComparer.EqualsIgnoreEventId(new EventResult(Guid.Empty, true, EventResult.ResultStates.success, null), actual.ElementAt<EventResult>(1)));
                Assert.IsTrue(EventResultWithIgnoredIdComparer.EqualsIgnoreEventId(new EventResult(Guid.Empty, false, EventResult.ResultStates.error, "Event Failed"), actual.ElementAt<EventResult>(2)));
                Assert.IsTrue(EventResultWithIgnoredIdComparer.EqualsIgnoreEventId(new EventResult(Guid.Empty, false, EventResult.ResultStates.success, null), actual.ElementAt<EventResult>(3)));
            });
        }

        [Test()]
        public void ClientEventSyncNullEventList()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.Send(null),
                    Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo("Event list cannot be empty or null."));
            });
        }

        [Test()]
        public void ClientEventSyncEmptyEventList()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.Send(new List<Event>()),
                    Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo("Event list cannot be empty or null."));
            });
        }

        [Test()]
        public void ClientEventSyncSendEventExistsWhenIdExists()
        {
            withSuccessfulResults(client =>
            {
                Assert.AreEqual(true, client.EventExists(Guid.NewGuid()));
            });
        }

        [Test()]
        public void ClientEventSyncSendEventExistsWhenIdNotExists()
        {
            withSuccessfulAndFailedResults(client =>
            {
                Assert.AreEqual(false, client.EventExists(Guid.NewGuid()));
            });
        }

        [Test()]
        public void ClientEventSyncSendEventBatchExist()
        {
            IList<Guid> input = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            IDictionary<string, bool> expectedResults = new Dictionary<string, bool>()
            {
                { input[0].ToString(), true },
                { input[1].ToString(), false },
                { input[2].ToString(), true },
                { input[3].ToString(), false }
            };
            withSuccessfulAndFailedResults(client =>
            {
                CollectionAssert.AreEqual(expectedResults, client.EventsExist(input));
            });
        }

        public void ClientEventSyncBatchExistBadRequest()
        {
            withSuccessfulAndFailedResults(client =>
            {
                Assert.That(() => client.EventsExist(null),
                    Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo("List of eventIds cannot be null."));
            });
        }
        #endregion

        #region Async Calls
        [Test()]
        public void ClientEventAsyncSendFullEvent()
        {
            withSuccessfulResults(client =>
            {
                IEnumerable<EventResult> expected;
                var actual = client.SendAsync(TestUtils.CreateEventsAndSuccessfulResults("deviceId", 2, out expected));
                actual.Wait();
                CollectionAssert.AreEqual(expected, actual.Result);
            });
        }

        [Test()]
        public void ClientEventAsyncSendBasicEvent()
        {
            withSuccessfulResults(client =>
            {
                IEnumerable<EventResult> expected = new List<EventResult>()
                {
                    new EventResult(Guid.Empty, true, EventResult.ResultStates.success, null),
                    new EventResult(Guid.Empty, true, EventResult.ResultStates.success, null),
                };

                var actual = client.SendAsync(
                    new List<Event>()
                    {
                        TestUtils.CreateBasicEvent("deviceId"),
                        TestUtils.CreateBasicEvent("deviceId")
                    });
                actual.Wait();

                CollectionAssert.AreEqual(expected, actual.Result, new EventResultWithIgnoredIdComparer());
            });
        }

        [Test()]
        public void ClientEventAsyncSendWithBadRequest()
        {
            withFailedResults(client =>
            {
                IEnumerable<EventResult> expected = new List<EventResult>()
                {
                    new EventResult(Guid.Empty, true, EventResult.ResultStates.success, null),
                    new EventResult(Guid.Empty, true, EventResult.ResultStates.success, null),
                };

                Assert.That(() => client.SendAsync(
                    new List<Event>()
                    {
                        TestUtils.CreateBasicEvent("deviceId"),
                        TestUtils.CreateBasicEvent("deviceId")
                    }).Wait(),
                    Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [Test()]
        public void ClientEventAsyncSendWithMultiResults()
        {
            withSuccessfulAndFailedResults(client =>
            {
                Guid eventId = Guid.NewGuid();
                IEnumerable<EventResult> expected = new List<EventResult>()
                {
                    new EventResult(eventId, true, EventResult.ResultStates.conflict, null),
                    new EventResult(Guid.Empty, true, EventResult.ResultStates.success, null),
                    new EventResult(Guid.Empty, false, EventResult.ResultStates.error, "Event Failed"),
                    new EventResult(Guid.Empty, false, EventResult.ResultStates.success, null)
                };

                var actual = client.SendAsync(
                    new List<Event>()
                    {
                        TestUtils.CreateEventWithEventId("deviceId", eventId),
                        TestUtils.CreateBasicEvent("deviceId"),
                        TestUtils.CreateBasicEvent("deviceId"),
                        TestUtils.CreateBasicEvent("deviceId")
                    });

                actual.Wait();
                Assert.AreEqual(4, actual.Result.Count<EventResult>());
                Assert.AreEqual(new EventResult(eventId, true, EventResult.ResultStates.conflict, null), actual.Result.ElementAt<EventResult>(0));
                Assert.IsTrue(EventResultWithIgnoredIdComparer.EqualsIgnoreEventId(new EventResult(Guid.Empty, true, EventResult.ResultStates.success, null), actual.Result.ElementAt<EventResult>(1)));
                Assert.IsTrue(EventResultWithIgnoredIdComparer.EqualsIgnoreEventId(new EventResult(Guid.Empty, false, EventResult.ResultStates.error, "Event Failed"), actual.Result.ElementAt<EventResult>(2)));
                Assert.IsTrue(EventResultWithIgnoredIdComparer.EqualsIgnoreEventId(new EventResult(Guid.Empty, false, EventResult.ResultStates.success, null), actual.Result.ElementAt<EventResult>(3)));
            });
        }

        [Test()]
        public void ClientEventAsyncNullEventList()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.SendAsync(null).Wait(),
                    Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("Event list cannot be empty or null."));
            });
        }

        [Test()]
        public void ClientEventAsyncEmptyEventList()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.SendAsync(new List<Event>()).Wait(),
                    Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("Event list cannot be empty or null."));
            });
        }

        [Test()]
        public void ClientEventAsyncSendEventExistsWhenIdExists()
        {
            withSuccessfulResults(client =>
            {
                var result = client.EventExistsAsync(Guid.NewGuid());
                result.Wait();
                Assert.AreEqual(true, result.Result);
            });
        }

        [Test()]
        public void ClientEventAsyncSendEventExistsWhenIdNotExists()
        {
            withSuccessfulAndFailedResults(client =>
            {
                var result = client.EventExistsAsync(Guid.NewGuid());
                result.Wait();
                Assert.AreEqual(false,result.Result);
            });
        }

        [Test()]
        public void ClientEventAsyncSendEventBatchExist()
        {
            IList<Guid> input = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            IDictionary<string, bool> expectedResults = new Dictionary<string, bool>()
            {
                { input[0].ToString(), true },
                { input[1].ToString(), false },
                { input[2].ToString(), true },
                { input[3].ToString(), false }
            };
            withSuccessfulAndFailedResults(client =>
            {
                var result = client.EventsExistAsync(input);
                result.Wait();
                CollectionAssert.AreEqual(expectedResults, result.Result);
            });
        }

        public void ClientEventAsyncBatchExistBadRequest()
        {
            withSuccessfulAndFailedResults(client =>
            {
                Assert.That(() => client.EventsExistAsync(null).Wait(),
                    Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("List of eventIds cannot be null."));
            });
        }
        #endregion


        //Spawn a client which respond successfully to all events
        internal void withSuccessfulResults(Action<EventClient> test)
        {
            var httpClient = new HttpClient(config, "http", "localhost", port, "/succeed");
            test(new EventClient(httpClient));
        }

        //Spawn a client which fail all request
        internal void withFailedResults(Action<EventClient> test)
        {
            var httpClient = new HttpClient(config, "http", "localhost", port, "/failed");
            test(new EventClient(httpClient));
        }

        //Spawn a client which fail respond with a mix of failed and successful events
        internal void withSuccessfulAndFailedResults(Action<EventClient> test)
        {
            var httpClient = new HttpClient(config, "http", "localhost", port, "/batch");
            test(new EventClient(httpClient));
        }
    }
}