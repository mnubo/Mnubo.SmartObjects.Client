using System;
using Mnubo.SmartObjects.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace Mnubo.SmartObjects.Client.Impl
{
    internal class ObjectClient : IObjectClient
    {
        /// <summary>
        /// Max batch size to process in a single request.
        /// </summary>
        public const int BatchMaxSize = 1000;

        private HttpClient client;

        internal ObjectClient(HttpClient client)
        {
            this.client = client;
        }

        #region Sync Calls
        public void Create(SmartObject smartObject)
        {
            SyncTaskExecutor(CreateAsync(smartObject));
        }

        public void Delete(string deviceId)
        {
            SyncTaskExecutor(DeleteAsync(deviceId));
        }

        public void Update(SmartObject smartObject, string deviceId)
        {
            SyncTaskExecutor(UpdateAsync(smartObject, deviceId));
        }

        public List<Result> CreateUpdate(List<SmartObject> objects)
        {
            try
            {
                Task<List<Result>> task = CreateUpdateAsync(objects);
                task.Wait();

                return task.Result;
            }
            catch (AggregateException aggreEx)
            {
                throw aggreEx.InnerException;
            }
        }
        #endregion

        #region Async Calls
        public Task CreateAsync(SmartObject smartObject)
        {
            return Task.Factory.StartNew(() =>
            {
                try
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

                    client.sendAsyncRequestWithBody(
                        HttpMethod.Post, "objects", 
                        SmartObjectSerializer.SerializeSmartObject(smartObject)).Wait();
                }
                catch (AggregateException aggreEx)
                {
                    throw aggreEx.InnerException;
                }
            });
        }

        public Task UpdateAsync(SmartObject smartObject, string deviceId)
        {
            return Task.Factory.StartNew(() =>
            {
                if (smartObject == null)
                {
                    throw new ArgumentException("SmartObject body cannot be null.");
                }

                if (string.IsNullOrEmpty(deviceId))
                {
                    throw new ArgumentException("x_device_id cannot be blank.");
                }

                try
                {
                    client.sendAsyncRequest(HttpMethod.Put,
                        string.Format("objects/{0}", deviceId), 
                        SmartObjectSerializer.SerializeSmartObject(smartObject)).Wait();
                }
                catch (AggregateException aggreEx)
                {
                    throw aggreEx.InnerException;
                }
            });
        }

        public Task DeleteAsync(string deviceId)
        {
            return Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(deviceId))
                {
                    throw new ArgumentException("x_device_id cannot be blank.");
                }

                try
                {
                    client.sendAsyncRequest(HttpMethod.Delete,
                        string.Format("objects/{0}", deviceId)).Wait();
                }
                catch (AggregateException aggreEx)
                {
                    throw aggreEx.InnerException;
                }
            });
        }

        public Task<List<Result>> CreateUpdateAsync(List<SmartObject> objects)
        {
            return Task<List<Result>>.Factory.StartNew(() =>
            {
                try
                {
                    if (objects == null)
                    {
                        throw new ArgumentException("object body list cannot be null.");
                    }
                    if (objects.Count == 0 || objects.Count > BatchMaxSize)
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

                    var asynResult = client.sendAsyncRequestWithBody(
                        HttpMethod.Put,
                        "objects",
                        SmartObjectSerializer.SerializeSmartObjects(objects));
                    asynResult.Wait();

                    return JsonConvert.DeserializeObject<List<Result>>(asynResult.Result);
                }
                catch (AggregateException aggreEx)
                {
                    throw aggreEx.InnerException;
                }
            });
        }
        #endregion

        private void SyncTaskExecutor(Task task)
        {
            try
            {
                task.Wait();
            }
            catch (AggregateException aggreEx)
            {
                throw aggreEx.InnerException;
            }
        }
    }
}