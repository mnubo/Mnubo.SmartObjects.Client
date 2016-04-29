using Mnubo.SmartObjects.Client.Config;

namespace Mnubo.SmartObjects.Client.Impl
{
    internal class SmartObjectsClient : ISmartObjectsClient
    {
        private HttpClient client;
        public IObjectClient Objects { get; }
        public IOwnerClient Owners { get; }
        public IEventClient Events { get; }
        public IRestitutionClient Restitution { get; }

        internal SmartObjectsClient(ClientConfig config)
        {
            client = new HttpClient(config);

            Objects = new ObjectClient(client);
            Owners = new OwnerClient(client);
            Events = new EventClient(client);
            Restitution = new RestitutionClient(client);
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}