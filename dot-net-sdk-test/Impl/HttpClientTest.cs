using System;
using NUnit.Framework;
using Mnubo.SmartObjects.Client.Impl;
using Mnubo.SmartObjects.Client.Config;
using Mnubo.SmartObjects.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Nancy.Hosting.Self;
using System.Net.Http;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Mnubo.SmartObjects.Client.Test.Impl
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

        [Test()]
        public void CompressionEnabledDefault()
        {
            var client = new Client.Impl.HttpClient(config, "http", "localhost", port, SucceedAPIsMockModule.BasePath);
            var task = client.sendAsyncRequestWithResult(HttpMethod.Post, "compressed", SucceedAPIsMockModule.TestJsonString);
            task.Wait();

            Assert.AreEqual(SucceedAPIsMockModule.TestJsonString, task.Result);
        }

        [Test()]
        public void CompressionEnabledForceTrue()
        {
            var configWithCompression = new ClientConfig.Builder()
            {
                Environment = ClientConfig.Environments.Sandbox,
                ConsumerKey = "key",
                ConsumerSecret = "secret",
                CompressionEnabled = true
            };

            var client = new Client.Impl.HttpClient(configWithCompression, "http", "localhost", port, SucceedAPIsMockModule.BasePath);
            var task = client.sendAsyncRequestWithResult(HttpMethod.Post, "compressed", SucceedAPIsMockModule.TestJsonString);
            task.Wait();

            Assert.AreEqual(SucceedAPIsMockModule.TestJsonString, task.Result);
        }

        [Test()]
        public void CompressionEnabledForceFalse()
        {
            var configNoCompression = new ClientConfig.Builder()
            {
                Environment = ClientConfig.Environments.Sandbox,
                ConsumerKey = "key",
                ConsumerSecret = "secret",
                CompressionEnabled = false
            };

            var client = new Client.Impl.HttpClient(configNoCompression, "http", "localhost", port, SucceedAPIsMockModule.BasePath);
            var task = client.sendAsyncRequestWithResult(HttpMethod.Post, "decompressed", SucceedAPIsMockModule.TestJsonString);
            task.Wait();

            Assert.AreEqual(SucceedAPIsMockModule.TestJsonString, task.Result);
        }
    }
}