using System;
using NUnit.Framework;
using Nancy.Hosting.Self;
using System.Net.Http;
using AspenTech.SmartObjects.Client.Config;

namespace AspenTech.SmartObjects.Client.Test.Impl
{
    [TestFixture()]
    public class HttpClientTest
    {
        private readonly ClientConfig config;
        private readonly NancyHost nancy;
        private readonly int port;

        public HttpClientTest()
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

        [Test()]
        public void CompressionEnabledDefault()
        {
            var client = new Client.Impl.HttpClient(config, "http", "localhost", port, SucceedAPIsMockModule.BasePath);
            var task = client.SendAsyncRequestWithResult(HttpMethod.Post, "compressed", SucceedAPIsMockModule.TestJsonString);
            task.Wait();

            Assert.AreEqual(SucceedAPIsMockModule.TestJsonString, task.Result);
        }
        [Test()]
        public void InitializationWithToken()
        {
            var otherConfig  =
                new ClientConfig.Builder()
                {
                    Hostname = ClientConfig.Environments.Sandbox,
                    Token = "token"
                };
            var client = new Client.Impl.HttpClient(otherConfig, "http", "localhost", port, SucceedAPIsMockModule.BasePath);
            var task = client.SendAsyncRequestWithResult(HttpMethod.Post, "tokencheck");
            task.Wait();
            Assert.AreEqual("Bearer token", task.Result);
        }

        [Test()]
        public void CompressionEnabledForceTrue()
        {
            var configWithCompression = new ClientConfig.Builder()
            {
                Hostname = ClientConfig.Environments.Sandbox,
                ConsumerKey = "key",
                ConsumerSecret = "secret",
                CompressionEnabled = true
            };

            var client = new Client.Impl.HttpClient(configWithCompression, "http", "localhost", port, SucceedAPIsMockModule.BasePath);
            var task = client.SendAsyncRequestWithResult(HttpMethod.Post, "compressed", SucceedAPIsMockModule.TestJsonString);
            task.Wait();

            Assert.AreEqual(SucceedAPIsMockModule.TestJsonString, task.Result);
        }

        [Test()]
        public void CompressionEnabledForceFalse()
        {
            var configNoCompression = new ClientConfig.Builder()
            {
                Hostname = ClientConfig.Environments.Sandbox,
                ConsumerKey = "key",
                ConsumerSecret = "secret",
                CompressionEnabled = false
            };

            var client = new Client.Impl.HttpClient(configNoCompression, "http", "localhost", port, SucceedAPIsMockModule.BasePath);
            var task = client.SendAsyncRequestWithResult(HttpMethod.Post, "decompressed", SucceedAPIsMockModule.TestJsonString);
            task.Wait();

            Assert.AreEqual(SucceedAPIsMockModule.TestJsonString, task.Result);
        }


        [Test()]
        public void WithExponentialBackoffOn()
        {
            var defaultBackOff = new ClientConfig.Builder()
            {
                Hostname = ClientConfig.Environments.Sandbox,
                ConsumerKey = "key",
                ConsumerSecret = "secret",
                ExponentialBackoffConfig = new ExponentialBackoffConfig.On(5, 500, (res, t) => {
                    Console.WriteLine("RETRYING");
                })
            };
            var client = new Client.Impl.HttpClient(defaultBackOff, "http", "localhost", port, ServiceUnavailableMockModule.BasePath);

            var task1 = client.SendAsyncRequestWithResult(HttpMethod.Post, "first", "");
            task1.Wait();
            Assert.AreEqual("first", task1.Result);

            var task2 = client.SendAsyncRequestWithResult(HttpMethod.Post, "second", "");
            task2.Wait();
            Assert.AreEqual("second", task2.Result);

            var task3 = client.SendAsyncRequestWithResult(HttpMethod.Post, "third", "");
            task3.Wait();
            Assert.AreEqual("third", task3.Result);
        }

        [Test()]
        public void ByDefaultNoExponentialBackoff()
        {
            var defaultBackOff = new ClientConfig.Builder()
            {
                Hostname = ClientConfig.Environments.Sandbox,
                ConsumerKey = "key",
                ConsumerSecret = "secret"
            };
            var client = new Client.Impl.HttpClient(defaultBackOff, "http", "localhost", port, ServiceUnavailableMockModule.BasePath);

            Assert.That(() => client.SendAsyncRequestWithResult(HttpMethod.Post, "first", "").Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<InvalidOperationException>()
                    .With.InnerException.Message.EqualTo("status code: ServiceUnavailable, message service out"));
        }
    }
}