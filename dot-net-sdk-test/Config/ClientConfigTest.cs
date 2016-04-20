using System;
using NUnit.Framework;
using Mnubo.SmartObjects.Client.Config;

namespace Mnubo.SmartObjects.Client.Test.Config
{
    [TestFixture()]
    public class ClientConfigTest
    {
        [Test()]
        public void ConfigBuilderDefaulConfig()
        {
            ClientConfig config = new ClientConfig.Builder()
            {
                Environment = ClientConfig.Environments.Sandbox,
                ConsumerKey = "CK",
                ConsumerSecret = "CS"
            };

            Assert.AreEqual(config.Environment, ClientConfig.Environments.Sandbox);
            Assert.AreEqual(config.ConsumerKey, "CK");
            Assert.AreEqual(config.ConsumerSecret, "CS");
            Assert.AreEqual(config.ClientTimeout, ClientConfig.Builder.DefaultTimeout);
            Assert.AreEqual(config.MaxResponseContentBufferSize, ClientConfig.Builder.DefaultMaxResponseCcontentBufferSize);
        }

        [Test()]
        public void ConfigBuilderDefaulConfigWithClientParameters()
        {
            ClientConfig config = new ClientConfig.Builder()
            {
                Environment = ClientConfig.Environments.Sandbox,
                ConsumerKey = "CK",
                ConsumerSecret = "CS",
                ClientTimeout = 50000,
                MaxResponseContentBufferSize = 1000000
            };

            Assert.AreEqual(config.Environment, ClientConfig.Environments.Sandbox);
            Assert.AreEqual(config.ConsumerKey, "CK");
            Assert.AreEqual(config.ConsumerSecret, "CS");
            Assert.AreEqual(config.ClientTimeout, 50000);
            Assert.AreEqual(config.MaxResponseContentBufferSize, 1000000);
        }


        [Test()]
        public void ConfigBuilderAdvanceConfig()
        {
            ClientConfig config = new ClientConfig.Builder()
            {
                Environment = ClientConfig.Environments.Sandbox,
                ConsumerKey = "CK",
                ConsumerSecret = "CS",
                ClientTimeout = 155,
                MaxResponseContentBufferSize = 166
            };

            Assert.AreEqual(config.Environment, ClientConfig.Environments.Sandbox);
            Assert.AreEqual(config.ConsumerKey, "CK");
            Assert.AreEqual(config.ConsumerSecret, "CS");
            Assert.AreEqual(config.ClientTimeout, 155);
            Assert.AreEqual(config.MaxResponseContentBufferSize, 166);
        }

        [TestCase(ClientConfig.Environments.Sandbox, null, "CS", 155, 166, "securityConsumerKey property cannot be blank.")]
        [TestCase(ClientConfig.Environments.Sandbox, "", "CS", 155, 166, "securityConsumerKey property cannot be blank.")]
        [TestCase(ClientConfig.Environments.Sandbox, "CK", null, 155, 166, "securityConsumerSecret property cannot be blank.")]
        [TestCase(ClientConfig.Environments.Sandbox, "CK", "", 155, 166, "securityConsumerSecret property cannot be blank.")]
        [TestCase(ClientConfig.Environments.Sandbox, "CK", "CS", -9, 555, "clientTimeout must be a positive number.")]
        [TestCase(ClientConfig.Environments.Sandbox, "CK", "CS", 15, -8, "maxResponseContentBufferSize must be a positive number.")]
        public void ConfigBuilderAdvanceConfigWrongValues(
            ClientConfig.Environments environment,
            string securityConsumerKey,
            string securityConsumerSecret,
            int clientTimeout,
            long maxResponseContentBufferSize,
            string errorMsg)
        {
            ClientConfig config;
            Assert.That(() => config = new ClientConfig.Builder()
            {
                Environment = environment,
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