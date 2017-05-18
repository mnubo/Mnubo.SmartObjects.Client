using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mnubo.SmartObjects.Client.Models.Search;
using System.Net.Http;
using Newtonsoft.Json;

namespace Mnubo.SmartObjects.Client.Impl

{
    /// <summary>
    /// Implements <see cref="IRestitutionClient"/>
    /// </summary>
    public class RestitutionClient : IRestitutionClient
    {
        private readonly HttpClient client;

        internal RestitutionClient(HttpClient client)
        {
            this.client = client;
        }

        #region Sync
        
        /// <summary>
        /// Implements <see cref="IRestitutionClient.GetDataSets"/>
        /// </summary>
        public IEnumerable<DataSet> GetDataSets()
        {
            return ClientUtils.WaitTask<IEnumerable<DataSet>>(GetDataSetsAsync());
        }
        
        /// <summary>
        /// Implements <see cref="IRestitutionClient.Search"/>
        /// </summary>
        public ResultSet Search(string query)
        {
            return ClientUtils.WaitTask<ResultSet>(SearchAsync(query));
        }
        #endregion

        #region Async

        
        /// <summary>
        /// Implements <see cref="IRestitutionClient.GetDataSetsAsync"/>
        /// </summary>
        public async Task<IEnumerable<DataSet>> GetDataSetsAsync()
        {
            var asynResult = await client.sendAsyncRequestWithResult(
                HttpMethod.Get,
                "search/datasets");

            return JsonConvert.DeserializeObject<IEnumerable<DataSet>>(asynResult);
        }

        /// <summary>
        /// Implements <see cref="IRestitutionClient.SearchAsync"/>
        /// </summary>
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
