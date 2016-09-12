using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mnubo.SmartObjects.Client.Models.Search;
using System.Net.Http;
using Newtonsoft.Json;

namespace Mnubo.SmartObjects.Client.Impl
{
    public class RestitutionClient : IRestitutionClient
    {
        private readonly HttpClient client;

        internal RestitutionClient(HttpClient client)
        {
            this.client = client;
        }

        #region Sync
        public IEnumerable<DataSet> GetDataSets()
        {
            return ClientUtils.WaitTask<IEnumerable<DataSet>>(GetDataSetsAsync());
        }

        public ResultSet Search(string query)
        {
            return ClientUtils.WaitTask<ResultSet>(SearchAsync(query));
        }
        #endregion

        #region Async
        public async Task<IEnumerable<DataSet>> GetDataSetsAsync()
        {
            var asynResult = await client.sendAsyncRequestWithResult(
                HttpMethod.Get,
                "search/datasets");

            return JsonConvert.DeserializeObject<IEnumerable<DataSet>>(asynResult);
        }

        public async Task<ResultSet> SearchAsync(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentException("Query cannot be empty or null.");
            }

            var asynResult = await client.sendAsyncRequestWithResult(
                HttpMethod.Post,
                "search/basic",
                query);

            return ResultSetDeserializer.DeserializeResultSet(asynResult);
        }
        #endregion
    }
}
