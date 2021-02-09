using System;
using AspenTech.SmartObjects.Client.Config;
using NUnit.Framework;

namespace AspenTech.SmartObjects.Client.Test.Config
{
    [TestFixture()]
    public class ClientConfigTest
    {
        [Test()]
        public void ConfigBuilderDefaulConfig()
        {
            ClientConfig config = new ClientConfig.Builder()
            {
                Hostname = ClientConfig.Environments.Sandbox,
                ConsumerKey = "CK",
                ConsumerSecret = "CS"
            };

            Assert.AreEqual(config.Hostname, ClientConfig.Environments.Sandbox);
            Assert.AreEqual(config.ConsumerKey, "CK");
            Assert.AreEqual(config.ConsumerSecret, "CS");
            Assert.AreEqual(config.ClientTimeout, ClientConfig.Builder.DefaultTimeout);
            Assert.AreEqual(config.MaxResponseContentBufferSize, ClientConfig.Builder.DefaultMaxResponseContentBufferSize);
            Assert.AreEqual(config.CompressionEnabled, true);
        }

        [Test()]
        public void ConfigBuilderDefaulConfigWithClientParameters()
        {
            ClientConfig config = new ClientConfig.Builder()
            {
                Hostname = ClientConfig.Environments.Sandbox,
                ConsumerKey = "CK",
                ConsumerSecret = "CS",
                ClientTimeout = 50000,
                MaxResponseContentBufferSize = 1000000,
                CompressionEnabled = false
            };

            Assert.AreEqual(config.Hostname, ClientConfig.Environments.Sandbox);
            Assert.AreEqual(config.ConsumerKey, "CK");
            Assert.AreEqual(config.ConsumerSecret, "CS");
            Assert.AreEqual(config.ClientTimeout, 50000);
            Assert.AreEqual(config.MaxResponseContentBufferSize, 1000000);
            Assert.AreEqual(config.CompressionEnabled, false);
        }


        [Test()]
        public void ConfigBuilderAdvanceConfig()
        {
            ClientConfig config = new ClientConfig.Builder()
            {
                Hostname = ClientConfig.Environments.Sandbox,
                ConsumerKey = "CK",
                ConsumerSecret = "CS",
                ClientTimeout = 155,
                MaxResponseContentBufferSize = 166,
                CompressionEnabled = true
            };

            Assert.AreEqual(config.Hostname, ClientConfig.Environments.Sandbox);
            Assert.AreEqual(config.ConsumerKey, "CK");
            Assert.AreEqual(config.ConsumerSecret, "CS");
            Assert.AreEqual(config.ClientTimeout, 155);
            Assert.AreEqual(config.MaxResponseContentBufferSize, 166);
            Assert.AreEqual(config.CompressionEnabled, true);
        }

        [TestCase(ClientConfig.Environments.Sandbox, null, "CS", 155, 166, "securityConsumerKey property cannot be blank.")]
        [TestCase(ClientConfig.Environments.Sandbox, "", "CS", 155, 166, "securityConsumerKey property cannot be blank.")]
        [TestCase(ClientConfig.Environments.Sandbox, "CK", null, 155, 166, "securityConsumerSecret property cannot be blank.")]
        [TestCase(ClientConfig.Environments.Sandbox, "CK", "", 155, 166, "securityConsumerSecret property cannot be blank.")]
        [TestCase(ClientConfig.Environments.Sandbox, "CK", "CS", -9, 555, "clientTimeout must be a positive number.")]
        [TestCase(ClientConfig.Environments.Sandbox, "CK", "CS", 15, -8, "maxResponseContentBufferSize must be a positive number.")]
        public void ConfigBuilderAdvanceConfigWrongValues(
            string environment,
            string securityConsumerKey,
            string securityConsumerSecret,
            int clientTimeout,
            long maxResponseContentBufferSize,
            string errorMsg)
        {
            ClientConfig config;
            Assert.That(() => config = new ClientConfig.Builder()
            {
                Hostname = environment,
                ConsumerKey = securityConsumerKey,
                ConsumerSecret = securityConsumerSecret,
                ClientTimeout = clientTimeout,
                MaxResponseContentBufferSize = maxResponseContentBufferSize
            },
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo(errorMsg));
        }
    }
}