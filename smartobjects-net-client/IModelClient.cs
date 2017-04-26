using Mnubo.SmartObjects.Client.Models.DataModel;
using System.Threading.Tasks;

namespace Mnubo.SmartObjects.Client
{
    /// <summary>
    /// Model API client interface
    /// </summary>
    public interface IModelClient
    {
        /// <summary>
        /// Export model of the current zone
        /// </summary>
        /// <returns>Model</returns>
        Model Export();

        /// <summary>
        /// Export model of the current zone asynchronously
        /// </summary>
        /// <returns>Model</returns>
        Task<Model> ExportAsync();
    }
}
