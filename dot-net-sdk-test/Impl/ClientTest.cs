using System;
using NUnit.Framework;
using Mnubo.SmartObjects.Client.Impl;
using Mnubo.SmartObjects.Client.Config;

namespace Mnubo.SmartObjects.Client.Test.Impl
{

    [TestFixture()]
//    [Ignore("Ignore a fixture")]
    public class ClientTest
    {
        private const ClientConfig.Environments Environment = ClientConfig.Environments.Sandbox;
        private const string ConsumerKeyValue = Config.ConsumerKey;
        private const string ConsumerSecretValue = Config.ConsumerSecret;

        [Test()]
        public void ClientCredentialHandlerFetchingToken()
        {
            ClientConfig config = new ClientConfig.Builder()
            {
                Environment = Environment,
                ConsumerKey = ConsumerKeyValue,
                ConsumerSecret = ConsumerSecretValue
            };

            ClientFactory.Create(config);
        }

        [TestCase("Unknown", ConsumerSecretValue)]
        [TestCase(ConsumerKeyValue, "Unknown")]
        public void ClientCredentialBadRequestToken(
            string consumerKey, 
            string consumerSecret)
        {
            ClientConfig config = new ClientConfig.Builder()
            {
                Environment = ClientConfig.Environments.Sandbox,
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret
            };

            Assert.That(() => ClientFactory.Create(config),
               Throws.TypeOf<InvalidOperationException>()
               .With.Message.EqualTo("Error fetching token..."));
        }
    }
}