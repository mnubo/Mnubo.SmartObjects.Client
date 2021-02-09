using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using AspenTech.SmartObjects.Client.Models.Search;
using Newtonsoft.Json;

namespace AspenTech.SmartObjects.Client.Impl

{
    /// <summary>
    /// HTTP implementation of <see cref="Mnubo.SmartObjects.Client.IRestitutionClient" />
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
            var asynResult = await client.SendAsyncRequestWithResult(
                HttpMethod.Get,
                "/api/v3/search/datasets");

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

            var asynResult = await client.SendAsyncRequestWithResult(
                HttpMethod.Post,
                "/api/v3/search/basic",
                query);

            return ResultSetDeserializer.DeserializeResultSet(asynResult);
        }
        #endregion
    }
}
