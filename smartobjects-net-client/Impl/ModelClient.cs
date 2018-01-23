using System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Mnubo.SmartObjects.Client.Models.DataModel;
using System.Collections.Generic;

namespace Mnubo.SmartObjects.Client.Impl
{
    /// <summary>
    /// HTTP implementation of <see cref="Mnubo.SmartObjects.Client.IModelClient" />
    /// </summary>
    public class ModelClient : IModelClient
    {
        private readonly HttpClient client;
        public SandboxOnlyOps SandboxOps { get; }

        internal ModelClient(HttpClient client)
        {
            this.client = client;
            this.SandboxOps = new SandboxOnlyOpsImpl(client);
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IModelClient.Export" />
        /// </summary>
        public Model Export()
        {
            return ClientUtils.WaitTask(ExportAsync());
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IModelClient.ExportAsync" />
        /// </summary>
        public async Task<Model> ExportAsync()
        {
            var asyncResult = await client.sendAsyncRequestWithResult(
                HttpMethod.Get,
                "model/export"
            );
            return ModelDeserializer.DeserializeModel(asyncResult);
        }

        public List<EventType> GetEventTypes()
        {
            return ClientUtils.WaitTask(GetEventTypesAsync());
        }

        public async Task<List<EventType>> GetEventTypesAsync()
        {
            var asyncResult = await client.sendAsyncRequestWithResult(
                HttpMethod.Get,
                "model/eventTypes"
            );
            return new EventTypeSerializer().Deserialize(asyncResult);
        }

        public List<ObjectAttribute> GetObjectAttributes()
        {
            return ClientUtils.WaitTask(GetObjectAttributesAsync());
        }

        public async Task<List<ObjectAttribute>> GetObjectAttributesAsync()
        {
            var asyncResult = await client.sendAsyncRequestWithResult(
                HttpMethod.Get,
                "model/objectAttributes"
            );
            return new ObjectAttributeSerializer().Deserialize(asyncResult);
        }

        public List<ObjectType> GetObjectTypes()
        {
            return ClientUtils.WaitTask(GetObjectTypesAsync());
        }

        public async Task<List<ObjectType>> GetObjectTypesAsync()
        {
            var asyncResult = await client.sendAsyncRequestWithResult(
                HttpMethod.Get,
                "model/objectTypes"
            );
            return new ObjectTypeSerializer().Deserialize(asyncResult);
        }

        public List<OwnerAttribute> GetOwnerAttributes()
        {
            return ClientUtils.WaitTask(GetOwnerAttributesAsync());
        }

        public async Task<List<OwnerAttribute>> GetOwnerAttributesAsync()
        {
            var asyncResult = await client.sendAsyncRequestWithResult(
                HttpMethod.Get,
                "model/ownerAttributes"
            );
            return new OwnerAttributeSerializer().Deserialize(asyncResult);
        }

        public List<Timeseries> GetTimeseries()
        {
            return ClientUtils.WaitTask(GetTimeseriesAsync());
        }

        public async Task<List<Timeseries>> GetTimeseriesAsync()
        {
            var asyncResult = await client.sendAsyncRequestWithResult(
                HttpMethod.Get,
                "model/timeseries"
            );
            return new TimeseriesSerializer().Deserialize(asyncResult);
        }
        class SandboxOnlyOpsImpl : SandboxOnlyOps
        {
            public SandboxTypeOps<EventType> EventTypesOps { get; }
            public SandboxTypeOps<ObjectType> ObjectTypesOps { get; }
            public SandboxEntityOps<ObjectAttribute> ObjectAttributesOps { get; }
            public SandboxEntityOps<OwnerAttribute> OwnerAttributesOps { get; }
            public SandboxEntityOps<Timeseries> TimeseriesOps { get; }
            public ResetOps ResetOps { get; }
            internal SandboxOnlyOpsImpl(HttpClient client) {
                this.EventTypesOps = new SandboxTypeOpsImpl<EventType>(client, "/eventTypes", new EventTypeSerializer());
                this.ObjectTypesOps = new SandboxTypeOpsImpl<ObjectType>(client, "/objectTypes", new ObjectTypeSerializer());
                this.ObjectAttributesOps = new SandboxEntityOpsImpl<ObjectAttribute>(client, "/objectAttributes", new ObjectAttributeSerializer());
                this.OwnerAttributesOps = new SandboxEntityOpsImpl<OwnerAttribute>(client, "/ownerAttributes", new OwnerAttributeSerializer());
                this.TimeseriesOps = new SandboxEntityOpsImpl<Timeseries>(client, "/timeseries", new TimeseriesSerializer());
                this.ResetOps = new ResetOpsImpl(client);
            }
        };

        class SandboxTypeOpsImpl<A> : SandboxTypeOps<A>
        {
            HttpClient client;
            string path;
            Serializer<A> serializer;

            internal SandboxTypeOpsImpl(HttpClient client, string path, Serializer<A> serializer) {
                this.client = client;
                this.path = path;
                this.serializer = serializer;
            }
            public void Create(List<A> value)
            {
                ClientUtils.WaitTask(CreateAsync(value));
            }

            public Task CreateAsync(List<A> value)
            {
                return this.client.sendAsyncRequest(
                    HttpMethod.Post,
                    "model" + this.path,
                    this.serializer.Serialize(value)
                );
            }

            public void CreateOne(A value)
            {
                Create(new List<A>() { value });
            }

            public Task CreateOneAsync(A value)
            {
                return CreateAsync(new List<A>() { value });
            }

            public void Delete(string key)
            {
                ClientUtils.WaitTask(DeleteAsync(key));
            }

            public Task DeleteAsync(string key)
            {
                return this.client.sendAsyncRequest(
                    HttpMethod.Delete,
                    "model" + this.path + "/" + key
                );
            }

            public void Update(string key, A update)
            {
                ClientUtils.WaitTask(UpdateAsync(key, update));
            }

            public Task UpdateAsync(string key, A update)
            {
                return this.client.sendAsyncRequest(
                    HttpMethod.Put,
                    "model" + this.path + "/" + key,
                    this.serializer.Serialize(update)
                );
            }
        }

        class SandboxEntityOpsImpl<A> : SandboxEntityOps<A> {
            private HttpClient client;
            private string path;
            private Serializer<A> serializer;
            private UpdateEntitySerializer ueSerializer = new UpdateEntitySerializer();
            
            internal SandboxEntityOpsImpl(HttpClient client, string path, Serializer<A> serializer) {
                this.client = client;
                this.path = path;
                this.serializer = serializer;
            }

            public void Create(List<A> value)
            {
                ClientUtils.WaitTask(CreateAsync(value));
            }

            public Task CreateAsync(List<A> value)
            {
                return this.client.sendAsyncRequest(
                    HttpMethod.Post,
                    "model" + this.path,
                    this.serializer.Serialize(value)
                );
            }

            public void CreateOne(A value)
            {
                Create(new List<A>() { value });
            }

            public Task CreateOneAsync(A value)
            {
                return CreateAsync(new List<A>() { value });
            }

            public string GenerateDeployCode(string key)
            {
                return ClientUtils.WaitTask(GenerateDeployCodeAsync(key));
            }

            public async Task<string> GenerateDeployCodeAsync(string key)
            {
                var json = await client.sendAsyncRequestWithResult(
                    HttpMethod.Post,
                    "model" + this.path + "/" + key + "/deploy"
                );
                var res = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                if (!res.ContainsKey("code"))
                    throw new InvalidOperationException("Expecting 'code' in the response body.");
                return res["code"];
            }

            public void ApplyDeployCode(string key, string code)
            {
                ClientUtils.WaitTask(ApplyDeployCodeAsync(key, code));
            }

            public async Task ApplyDeployCodeAsync(string key, string code)
            {
                await client.sendAsyncRequest(
                    HttpMethod.Post,
                    "model" + this.path + "/" + key + "/deploy/" + code
                );
            }

            public void Deploy(string key)
            {
                ClientUtils.WaitTask(DeployAsync(key));
            }

            public async Task DeployAsync(string key)
            {
                var code = await GenerateDeployCodeAsync(key);
                await ApplyDeployCodeAsync(key, code);
            }

            public void Update(string key, UpdateEntity update)
            {
                ClientUtils.WaitTask(UpdateAsync(key, update));
            }

            public Task UpdateAsync(string key, UpdateEntity update)
            {
                return this.client.sendAsyncRequest(
                    HttpMethod.Put,
                    "model" + this.path + "/" + key,
                    this.ueSerializer.Serialize(update)
                );
            }
        }

        class ResetOpsImpl : ResetOps {
            private HttpClient client;
            internal ResetOpsImpl(HttpClient client){
                this.client = client;
            }
            public string GenerateResetCode() {
                return ClientUtils.WaitTask(GenerateResetCodeAsync());
            }

            public async Task<string> GenerateResetCodeAsync(){
                var json = await client.sendAsyncRequestWithResult(
                    HttpMethod.Post,
                    "model/reset"
                );
                var res = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                if (!res.ContainsKey("code"))
                    throw new InvalidOperationException("Expecting 'code' in the response body.");
                return res["code"];
            }

            public void ApplyResetCode(string code) {
                ClientUtils.WaitTask(ApplyResetCodeAsync(code));
            }

            public async Task ApplyResetCodeAsync(string code) {

                await client.sendAsyncRequest(
                    HttpMethod.Post,
                    "model/reset/" + code
                );
            }

            public void Reset() {
                ClientUtils.WaitTask(ResetAsync());
            }

            public async Task ResetAsync() {
                var code = await GenerateResetCodeAsync();
                await ApplyResetCodeAsync(code);
            }

        }
    }
}
