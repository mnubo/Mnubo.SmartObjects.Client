using System;
using Mnubo.SmartObjects.Client.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace Mnubo.SmartObjects.Client.Impl
{
    internal class OwnerClient : IOwnerClient
    {
        /// <summary>
        /// Max batch size to process in a single request.
        /// </summary>
        private const int BatchMaxSize = 1000;

        private readonly HttpClient client;

        internal OwnerClient(HttpClient client)
        {
            this.client = client;
        }

        #region Sync calls
        public void Claim(string username, string deviceId)
        {
            ClientUtils.WaitTask(ClaimAsync(username, deviceId));
        }

        public void Create(Owner owner)
        {
            ClientUtils.WaitTask(CreateAsync(owner));
        }

        public void Delete(string username)
        {
            ClientUtils.WaitTask(DeleteAsync(username));
        }

        public void Update(Owner owner, string username)
        {
            ClientUtils.WaitTask(UpdateAsync(owner, username));
        }

        public void UpdatePassword(string username, string password)
        {
            ClientUtils.WaitTask(UpdatePasswordAsync(username, password));
        }

        public IEnumerable<Result> CreateUpdate(IEnumerable<Owner> owners)
        {
            return ClientUtils.WaitTask<IEnumerable<Result>>(CreateUpdateAsync(owners));
        }

        public bool OwnerExists(string username)
        {
            return ClientUtils.WaitTask<bool>(OwnerExistsAsync(username));
        }

        public IDictionary<string, bool> OwnersExist(IList<string> usernames)
        {
            return ClientUtils.WaitTask<IDictionary<string, bool>>(OwnersExistAsync(usernames));
        }
        #endregion

        #region Async calls
        public async Task CreateAsync(Owner owner)
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

            await client.sendAsyncRequest(
                HttpMethod.Post,
                "owners",
                OwnerSerializer.SerializeOwner(owner));
        }

        public async Task ClaimAsync(string username, string deviceId)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("username cannot be blank.");
            }

            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentException("device_Id cannot be blank.");
            }

            await client.sendAsyncRequest(HttpMethod.Post,
                    string.Format("owners/{0}/objects/{1}/claim", username, deviceId));
        }

        public async Task UpdateAsync(Owner owner, string username)
        {
            if (owner == null)
            {
                throw new ArgumentException("owner body cannot be null.");
            }

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("username cannot be blank.");
            }

            await client.sendAsyncRequest(HttpMethod.Put,
                string.Format("owners/{0}", username),
                OwnerSerializer.SerializeOwner(owner));
        }

        public async Task DeleteAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("username cannot be blank.");
            }

            await client.sendAsyncRequest(HttpMethod.Delete,
                string.Format("owners/{0}", username));
        }

        public async Task UpdatePasswordAsync(string username, string password)
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

            await client.sendAsyncRequest(HttpMethod.Put,
                string.Format("owners/{0}/password", username),
                OwnerSerializer.SerializeOwner(owner));
        }

        public async Task<IEnumerable<Result>> CreateUpdateAsync(IEnumerable<Owner> owners)
        {
            if (owners == null)
            {
                throw new ArgumentException("owner body list cannot be null.");
            }

            if (owners.Count() == 0 || owners.Count() > BatchMaxSize)
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

            var asynResult = await client.sendAsyncRequestWithResult(
                HttpMethod.Put,
                "owners",
                OwnerSerializer.SerializeOwners(owners));

            return JsonConvert.DeserializeObject<IEnumerable<Result>>(asynResult);
        }

        public async Task<bool> OwnerExistsAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("username cannot be blank.");
            }

            var asynResultAsStr = await client.sendAsyncRequestWithResult(
                HttpMethod.Get,
                string.Format("owners/exists/{0}", username),
                null);

            var asynResult = JsonConvert.DeserializeObject<IDictionary<string, bool>>(asynResultAsStr);

            return asynResult != null && asynResult.Count == 1 && asynResult.ContainsKey(username) && asynResult[username];
        }

        public async Task<IDictionary<string, bool>> OwnersExistAsync(IList<string> usernames)
        {
            if (usernames == null)
            {
                throw new ArgumentException("List of usernames cannot be null.");
            }

            var asynResultAsStr = await client.sendAsyncRequestWithResult(
                HttpMethod.Post,
                "owners/exists",
                JsonConvert.SerializeObject(usernames));

            return ExistResultsDeserializer.DeserializeExistResults(asynResultAsStr);
        }
        #endregion
    }
}