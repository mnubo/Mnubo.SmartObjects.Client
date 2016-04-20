using System;
using Mnubo.SmartObjects.Client.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Mnubo.SmartObjects.Client.Impl
{
    internal class OwnerClient : IOwnerClient
    {
        /// <summary>
        /// Max batch size to process in a single request.
        /// </summary>
        public const int BatchMaxSize = 1000;

        private HttpClient client;

        internal OwnerClient(HttpClient client)
        {
            this.client = client;
        }

        #region Sync calls
        public void Claim(string username, string deviceId)
        {
            SyncTaskExecutor(ClaimAsync(username, deviceId));
        }

        public void Create(Owner owner)
        {
            SyncTaskExecutor(CreateAsync(owner));
        }

        public void Delete(string username)
        {
            SyncTaskExecutor(DeleteAsync(username));
        }

        public void Update(Owner owner, string username)
        {
            SyncTaskExecutor(UpdateAsync(owner, username));
        }

        public void UpdatePassword(string username, string password)
        {
            SyncTaskExecutor(UpdatePasswordAsync(username, password));
        }

        public List<Result> CreateUpdate(List<Owner> owners)
        {
            try
            {
                Task<List<Result>> task = CreateUpdateAsync(owners);
                task.Wait();

                return task.Result;
            }
            catch (AggregateException aggreEx)
            {
                throw aggreEx.InnerException;
            }
        }
        #endregion

        #region Async calls
        public Task CreateAsync(Owner owner)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    if (owner == null)
                    {
                        throw new ArgumentException("owner body cannot be null.");
                    }

                    if (string.IsNullOrEmpty(owner.Username))
                    {
                        throw new ArgumentException("username cannot be blank.");
                    }

                    if (string.IsNullOrEmpty(owner.Password))
                    {
                        throw new ArgumentException("password cannot be blank.");
                    }

                    client.sendAsyncRequestWithBody(
                        HttpMethod.Post, 
                        "owners",
                        OwnerSerializer.SerializeOwner(owner)).Wait();
                }
                catch (AggregateException aggreEx)
                {
                    throw aggreEx.InnerException;
                }
            });
        }

        public Task ClaimAsync(string username, string deviceId)
        {
            return Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(username))
                {
                    throw new ArgumentException("username cannot be blank.");
                }

                if (string.IsNullOrEmpty(deviceId))
                {
                    throw new ArgumentException("device_Id cannot be blank.");
                }

                try
                {
                    client.sendAsyncRequest(HttpMethod.Post,
                        string.Format("owners/{0}/objects/{1}/claim", username, deviceId)).Wait();
                }
                catch (AggregateException aggreEx)
                {
                    throw aggreEx.InnerException;
                }
            });
        }

        public Task UpdateAsync(Owner owner, string username)
        {
            return Task.Factory.StartNew(() =>
            {
            if (owner == null)
            {
                throw new ArgumentException("owner body cannot be null.");
            }

                if (string.IsNullOrEmpty(username))
                {
                    throw new ArgumentException("username cannot be blank.");
                }

                try
                {
                    client.sendAsyncRequest(HttpMethod.Put,
                        string.Format("owners/{0}", username), 
                        OwnerSerializer.SerializeOwner(owner)).Wait();
                }
                catch (AggregateException aggreEx)
                {
                    throw aggreEx.InnerException;
                }
            });
        }

        public Task DeleteAsync(string username)
        {
            return Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(username))
                {
                    throw new ArgumentException("username cannot be blank.");
                }

                try
                {
                    client.sendAsyncRequest(HttpMethod.Delete,
                        string.Format("owners/{0}", username)).Wait();
                }
                catch (AggregateException aggreEx)
                {
                    throw aggreEx.InnerException;
                }
            });
        }

        public Task UpdatePasswordAsync(string username, string password)
        {
            return Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(username))
                {
                    throw new ArgumentException("username cannot be blank.");
                }

                if (string.IsNullOrEmpty(password))
                {

                    throw new ArgumentException("password cannot be blank.");
                }

                Owner owner = new Owner.Builder()
                {
                    Username = username,
                    Password = password
                };

                try
                {
                    client.sendAsyncRequest(HttpMethod.Put,
                        string.Format("owners/{0}/password", username), 
                        OwnerSerializer.SerializeOwner(owner)).Wait();
                }
                catch (AggregateException aggreEx)
                {
                    throw aggreEx.InnerException;
                }
            });
        }

        public Task<List<Result>> CreateUpdateAsync(List<Owner> owners)
        {
            return Task<List<Result>>.Factory.StartNew(() =>
            {
                try
                {
                    if (owners == null)
                    {
                        throw new ArgumentException("owner body list cannot be null.");
                    }

                    if (owners.Count == 0 || owners.Count > BatchMaxSize)
                    {
                        throw new ArgumentException(string.Format("Owner body list cannot be empty or biger that {0}.", BatchMaxSize));
                    }

                    foreach (Owner owner in owners)
                    {
                        if (string.IsNullOrEmpty(owner.Username))
                        {
                            throw new ArgumentException("username cannot be blank.");
                        }
                    }

                    var asynResult = client.sendAsyncRequestWithBody(
                        HttpMethod.Put,
                        "owners",
                        OwnerSerializer.SerializeOwners(owners));
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