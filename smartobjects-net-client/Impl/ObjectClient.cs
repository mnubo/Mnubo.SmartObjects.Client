using System;
using Mnubo.SmartObjects.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;

namespace Mnubo.SmartObjects.Client.Impl
{
    /// <summary>
    /// HTTP implementation of <see cref="Mnubo.SmartObjects.Client.IObjectClient" />
    /// </summary>
    internal class ObjectClient : IObjectClient
    {
        /// <summary>
        /// Max batch size to process in a single request.
        /// </summary>
        private const int BatchMaxSize = 1000;

        private readonly HttpClient client;

        internal ObjectClient(HttpClient client)
        {
            this.client = client;
        }

        #region Sync Calls

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IObjectClient.Create" />
        /// </summary>
        public void Create(SmartObject smartObject)
        {
            ClientUtils.WaitTask(CreateAsync(smartObject));
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IObjectClient.Delete" />
        /// </summary>
        public void Delete(string deviceId)
        {
            ClientUtils.WaitTask(DeleteAsync(deviceId));
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IObjectClient.Update" />
        /// </summary>
        public void Update(SmartObject smartObject, string deviceId)
        {
            ClientUtils.WaitTask(UpdateAsync(smartObject, deviceId));
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IObjectClient.CreateUpdate" />
        /// </summary>
        public IEnumerable<Result> CreateUpdate(IEnumerable<SmartObject> objects)
        {
            return ClientUtils.WaitTask<IEnumerable<Result>>(CreateUpdateAsync(objects));
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IObjectClient.ObjectExists" />
        /// </summary>
        public bool ObjectExists(string deviceId)
        {
            return ClientUtils.WaitTask<bool>(ObjectExistsAsync(deviceId));
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IObjectClient.ObjectsExist" />
        /// </summary>
        public IDictionary<string, bool> ObjectsExist(IList<string> deviceIds)
        {
            return ClientUtils.WaitTask<IDictionary<string, bool>>(ObjectsExistAsync(deviceIds));
        }
        #endregion

        #region Async Calls
        
        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IObjectClient.CreateAsync" />
        /// </summary>
        public async Task CreateAsync(SmartObject smartObject)
        {
            if (smartObject == null)
            {
                throw new ArgumentException("SmartObject body cannot be null.");
            }

            if (string.IsNullOrEmpty(smartObject.DeviceId))
            {
                throw new ArgumentException("x_device_id cannot be blank.");
            }

            if (string.IsNullOrEmpty(smartObject.ObjectType))
            {
                throw new ArgumentException("x_object_type cannot be blank.");
            }

            await client.SendAsyncRequest(
                HttpMethod.Post, 
                "/api/v3/objects",
                ObjectSerializer.SerializeObject(smartObject));
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IObjectClient.UpdateAsync" />
        /// </summary>
        public async Task UpdateAsync(SmartObject smartObject, string deviceId)
        {
            if (smartObject == null)
            {
                throw new ArgumentException("SmartObject body cannot be null.");
            }

            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentException("x_device_id cannot be blank.");
            }

            await client.SendAsyncRequest(
                HttpMethod.Put,
                string.Format("/api/v3/objects/{0}", deviceId),
                ObjectSerializer.SerializeObject(smartObject));
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IObjectClient.DeleteAsync" />
        /// </summary>
        public async Task DeleteAsync(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentException("x_device_id cannot be blank.");
            }

            await client.SendAsyncRequest(
                HttpMethod.Delete,
                string.Format("/api/v3/objects/{0}", deviceId));
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IObjectClient.CreateUpdateAsync" />
        /// </summary>
        public async Task<IEnumerable<Result>> CreateUpdateAsync(IEnumerable<SmartObject> objects)
        {
            if (objects == null)
            {
                throw new ArgumentException("object body list cannot be null.");
            }
            if (objects.Count() == 0 || objects.Count() > BatchMaxSize)
            {
                throw new ArgumentException(string.Format("object body list cannot be empty or biger that {0}.", BatchMaxSize));
            }

            foreach (SmartObject smartObject in objects)
            {
                if (string.IsNullOrEmpty(smartObject.DeviceId))
                {
                    throw new ArgumentException("x_device_id cannot be blank.");
                }
            }

            var asynResult = await client.SendAsyncRequestWithResult(
                HttpMethod.Put,
                "/api/v3/objects",
                ObjectSerializer.SerializeObjects(objects));

            return JsonConvert.DeserializeObject<IEnumerable<Result>>(asynResult);
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IObjectClient.ObjectExistsAsync" />
        /// </summary>
        public async Task<bool> ObjectExistsAsync(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentException("deviceId cannot be blank.");
            }

            var asynResultAsStr = await client.SendAsyncRequestWithResult(
                HttpMethod.Get,
                string.Format("/api/v3/objects/exists/{0}", deviceId),
                null);

            var asynResult = JsonConvert.DeserializeObject<IDictionary<string, bool>>(asynResultAsStr);

            return asynResult != null && asynResult.Count == 1 && asynResult.ContainsKey(deviceId) && asynResult[deviceId];
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IObjectClient.ObjectsExistAsync" />
        /// </summary>
        public async Task<IDictionary<string, bool>> ObjectsExistAsync(IList<string> deviceIds)
        {
            if (deviceIds == null)
            {
                throw new ArgumentException("List of deviceIds cannot be null.");
            }

            var asynResultAsStr = await client.SendAsyncRequestWithResult(
                HttpMethod.Post,
                "/api/v3/objects/exists",
                JsonConvert.SerializeObject(deviceIds));

            return ExistResultsDeserializer.DeserializeExistResults(asynResultAsStr);
        }
        #endregion
    }
}