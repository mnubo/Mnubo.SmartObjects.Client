using Mnubo.SmartObjects.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mnubo.SmartObjects.Client
{
    /// <summary>
    /// Object client interface
    /// </summary>
    public interface IObjectClient
    {
        /// <summary>
        /// Allows create a new object in sync mode.
        /// </summary>
        /// <param name="smartObject">object to be created</param>
        void Create(SmartObject smartObject);

        /// <summary>
        /// Allows update an existing object in sync mode.
        /// </summary>
        /// <param name="smartObject">new values of the object to be updated</param>
        /// <param name="deviceId">deviceId of the object to be updated</param>
        void Update(SmartObject smartObject, string deviceId);

        /// <summary>
        /// Allows delete an existing Object in sync mode.
        /// </summary>
        /// <param name="deviceId">deviceId of the object to delete</param>
        void Delete(string deviceId);

        /// <summary>
        /// Allow create, or update in case of the SmartObject(s) already exist, a batch of SmartObjects in async mode.
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        List<Result> CreateUpdate(List<SmartObject> objs);

        /// <summary>
        /// Allows create a new object in async mode.
        /// </summary>
        /// <param name="smartObject">object to be created</param>
        /// <returns>A async task.</returns>
        Task CreateAsync(SmartObject smartObject);

        /// <summary>
        /// Allows update an existing object in async mode.
        /// </summary>
        /// <param name="smartObject">new values of the object to be updated</param>
        /// <param name="deviceId">deviceId of the object to be updated</param>
        /// <returns>A async task.</returns>
        Task UpdateAsync(SmartObject smartObject, string deviceId);

        /// <summary>
        /// Allows delete an existing Object in async mode.
        /// </summary>
        /// <param name="deviceId">deviceId of the object to delete</param>
        /// <returns>A async task.</returns>
        Task DeleteAsync(string deviceId);

        /// <summary>
        /// Allow create, or update in case of the SmartObject(s) already exist, a batch of SmartObjects in async mode.
        /// </summary>
        /// <param name="objs">List of the objects</param>
        /// <returns>A async task.</returns>
        Task<List<Result>> CreateUpdateAsync(List<SmartObject> objs);
    }
}