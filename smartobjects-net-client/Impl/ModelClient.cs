using System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Mnubo.SmartObjects.Client.Models.DataModel;

namespace Mnubo.SmartObjects.Client.Impl
{
    /// <summary>
    /// HTTP implementation of <see cref="Mnubo.SmartObjects.Client.IModelClient" />
    /// </summary>
    public class ModelClient : IModelClient
    {
        private readonly HttpClient client;

        internal ModelClient(HttpClient client)
        {
            this.client = client;
        }

        #region Sync
        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IModelClient.Export" />
        /// </summary>
        public Model Export()
        {
            return ClientUtils.WaitTask(ExportAsync());
        }

        #endregion

        #region Async
        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IModelClient.ExportAsync" />
        /// </summary>
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
