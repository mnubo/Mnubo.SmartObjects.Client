using System;
using System.Threading.Tasks;
using Mnubo.SmartObjects.Client.Models.Search;
using System.Net.Http;
using Newtonsoft.Json;
using Mnubo.SmartObjects.Client.Models.DataModel;

namespace Mnubo.SmartObjects.Client.Impl
{
    public class ModelClient : IModelClient
    {
        private readonly HttpClient client;

        internal ModelClient(HttpClient client)
        {
            this.client = client;
        }

        #region Sync
        public Model Export()
        {
            return ClientUtils.WaitTask(ExportAsync());
        }

        #endregion

        #region Async
        public async Task<Model> ExportAsync()
        {
            var asynResult = await client.sendAsyncRequestWithResult(
                HttpMethod.Get,
                "model/export"
            );
            return ModelDeserializer.DeserializeModel(asynResult);
        }
        #endregion
    }
}
