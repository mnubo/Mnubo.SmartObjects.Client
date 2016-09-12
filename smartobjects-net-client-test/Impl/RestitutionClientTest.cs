using Mnubo.SmartObjects.Client.Config;
using Mnubo.SmartObjects.Client.Impl;
using Mnubo.SmartObjects.Client.Models;
using Mnubo.SmartObjects.Client.Models.Search;
using Nancy.Hosting.Self;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mnubo.SmartObjects.Client.Test.Impl
{
    [TestFixture()]
    public class RestitutionClientTest
    {
        private readonly ClientConfig config;
        private readonly NancyHost nancy;
        private readonly int port;

        public RestitutionClientTest()
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

        #region Sync
        [Test()]
        public void ClientRestitutionSyncGetDatasets()
        {
            withSuccessfulResults(client =>
            {
                IEnumerable<DataSet> actual = client.GetDataSets();
                IEnumerable<DataSet> expected = TestUtils.CreateDatasets();
                Assert.AreEqual(actual.Count(), expected.Count());
                for (int i = 0; i < actual.Count(); i++)
                {
                    TestUtils.AssertDataSetEquals(actual.ElementAt<DataSet>(i), expected.ElementAt<DataSet>(i));
                }
            });
        }

        [Test()]
        public void ClientRestitutionSyncGetDatasetsBadRequest()
        {
            withFailedResults(client =>
            {
                 Assert.That(() => client.GetDataSets(),
                    Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [Test()]
        public void ClientRestitutionSyncSearch()
        {
            withSuccessfulResults(client =>
            {
                string expected = TestUtils.CreateExpectedSerializedResultSet();
                string actual = JsonConvert.SerializeObject(client.Search(TestUtils.CreateQuery()));
                Assert.AreEqual(expected, actual);
            });
        }

        [Test()]
        public void ClientRestitutionSyncSearchBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.Search(TestUtils.CreateQuery()),
                   Throws.TypeOf<ArgumentException>()
                   .With.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [TestCase("")]
        [TestCase(null)]
        public void ClientRestitutionSyncSearchGetEventsQueryValidator(string query)
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.Search(query),
                Throws.TypeOf<ArgumentException>()
                   .With.Message.EqualTo("Query cannot be empty or null."));
            });
        }


        #endregion

        #region Async
        [Test()]
        public void ClientRestitutionAsyncGetDatasets()
        {
            withSuccessfulResults(client =>
            {
                IEnumerable<DataSet> expected = TestUtils.CreateDatasets();
                Task<IEnumerable<DataSet>> resultTask = client.GetDataSetsAsync();
                resultTask.Wait();
                IEnumerable<DataSet> actual = resultTask.Result;
                Assert.AreEqual(actual.Count(), expected.Count());
                for (int i = 0; i < actual.Count(); i++)
                {
                    TestUtils.AssertDataSetEquals(actual.ElementAt<DataSet>(i), expected.ElementAt<DataSet>(i));
                }
            });
        }

        [Test()]
        public void ClientRestitutionAsyncGetDatasetsBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.GetDataSetsAsync().Wait(),
                   Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [Test()]
        public void ClientRestitutionAsyncSearch()
        {
            withSuccessfulResults(client =>
            {
                String expected = TestUtils.CreateExpectedSerializedResultSet();
                Task<ResultSet> resultTask = client.SearchAsync(TestUtils.CreateQuery());
                resultTask.Wait();
                String actual = JsonConvert.SerializeObject(resultTask.Result);
                Assert.AreEqual(expected, actual);
            });
        }

        [Test()]
        public void ClientRestitutionAsyncSearchBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.SearchAsync(TestUtils.CreateQuery()).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [TestCase("")]
        [TestCase(null)]
        public void ClientRestitutionAsyncSearchGetEventsQueryValidator(string query)
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.SearchAsync(query).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("Query cannot be empty or null."));
            });
        }
        #endregion

        #region Utils
        //Spawn a client which respond successfully to all events
        internal void withSuccessfulResults(Action<IRestitutionClient> test)
        {
            var httpClient = new HttpClient(config, "http", "localhost", port, SucceedAPIsMockModule.BasePath);
            test(new RestitutionClient(httpClient));
        }

        //Spawn a client which fail all request
        internal void withFailedResults(Action<IRestitutionClient> test)
        {
            var httpClient = new HttpClient(config, "http", "localhost", port, FailedAPIsMockModule.BasePath);
            test(new RestitutionClient(httpClient));
        }

        //Spawn a client which fail respond with a mix of failed and successful events
        internal void withSuccessfulAndFailedResults(Action<IRestitutionClient> test)
        {
            var httpClient = new HttpClient(config, "http", "localhost", port, BatchAPIsMockModule.BasePath);
            test(new RestitutionClient(httpClient));
        }

        #endregion
    }
}
