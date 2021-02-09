using AspenTech.SmartObjects.Client.Config;
using AspenTech.SmartObjects.Client.Datalake;

namespace AspenTech.SmartObjects.Client.Impl
{
    internal class SmartObjectsClient : ISmartObjectsClient
    {
        private readonly HttpClient _client;
        
        public IObjectClient Objects { get; }
        
        public IOwnerClient Owners { get; }
        
        public IEventClient Events { get; }
        
        public IRestitutionClient Restitution { get; }
        
        public IModelClient Model { get; }
        
        public IDatalakeClient Datalake { get; }

        internal SmartObjectsClient(ClientConfig config)
        {
            this._client = new HttpClient(config);

            Objects = new ObjectClient(this._client);
            Owners = new OwnerClient(this._client);
            Events = new EventClient(this._client);
            Restitution = new RestitutionClient(this._client);
            Model = new ModelClient(this._client);
            Datalake = new DatalakeClient(this._client, new DatasetValidator());
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}