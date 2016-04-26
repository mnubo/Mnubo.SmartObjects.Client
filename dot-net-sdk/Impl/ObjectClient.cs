using System;
using Mnubo.SmartObjects.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;

namespace Mnubo.SmartObjects.Client.Impl
{
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
        public void Create(SmartObject smartObject)
        {
            ClientUtils.WaitTask(CreateAsync(smartObject));
        }

        public void Delete(string deviceId)
        {
            ClientUtils.WaitTask(DeleteAsync(deviceId));
        }

        public void Update(SmartObject smartObject, string deviceId)
        {
            ClientUtils.WaitTask(UpdateAsync(smartObject, deviceId));
        }

        public IEnumerable<Result> CreateUpdate(IEnumerable<SmartObject> objects)
        {
            return ClientUtils.WaitTask(CreateUpdateAsync(objects));
        }
        #endregion

        #region Async Calls
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

            await client.sendAsyncRequest(
                HttpMethod.Post, 
                "objects",
                ObjectSerializer.SerializeObject(smartObject));
        }

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

            await client.sendAsyncRequest(
                HttpMethod.Put,
                string.Format("objects/{0}", deviceId),
                ObjectSerializer.SerializeObject(smartObject));
        }

        public async Task DeleteAsync(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentException("x_device_id cannot be blank.");
            }

            await client.sendAsyncRequest(
                HttpMethod.Delete,
                string.Format("objects/{0}", deviceId));
        }

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

            var asynResult = await client.sendAsyncRequestWithResult(
                HttpMethod.Put,
                "objects",
                ObjectSerializer.SerializeObjects(objects));

            return JsonConvert.DeserializeObject<IEnumerable<Result>>(asynResult);
        }
        #endregion
    }
}