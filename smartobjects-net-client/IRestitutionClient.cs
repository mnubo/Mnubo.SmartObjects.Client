using Mnubo.SmartObjects.Client.Models.Search;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mnubo.SmartObjects.Client
{
    /// <summary>
    /// search API client interface, it supports Dataset and basic search.
    /// </summary>
    public interface IRestitutionClient
    {
        /// <summary>
        /// Give support to basic search on Sync mode
        /// </summary>
        /// <param name="query">Query to search</param>
        /// <returns>Query result</returns>
        ResultSet Search(string query);

        /// <summary>
        /// Give support of search datasets on Sync mode
        /// </summary>
        /// <returns>Dataset list</returns>
        IEnumerable<DataSet> GetDataSets();

        /// <summary>
        /// Give support to basic search on Async mode
        /// </summary>
        /// <param name="query">Query to search</param>
        /// <returns>Query result</returns>
        Task<ResultSet> SearchAsync(string query);

        /// <summary>
        /// Give support of search datasets on Async mode
        /// </summary>
        /// <returns>Dataset list</returns>
        Task<IEnumerable<DataSet>> GetDataSetsAsync();
    }
}
