using System;
using Mnubo.SmartObjects.Client.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace Mnubo.SmartObjects.Client.Impl
{
    /// <summary>
    /// HTTP implementation of <see cref="Mnubo.SmartObjects.Client.IOwnerClient" />
    /// </summary>
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
        
        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IOwnerClient.Claim" />
        /// </summary>
        public void Claim(string username, string deviceId, Dictionary<string, object> body)
        {
            ClientUtils.WaitTask(ClaimAsync(username, deviceId, body));
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IOwnerClient.Unclaim" />
        /// </summary>
        public void Unclaim(string username, string deviceId, Dictionary<string, object> body)
        {
            ClientUtils.WaitTask(UnclaimAsync(username, deviceId, body));
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IOwnerClient.BatchClaim" />
        /// </summary>
        public IEnumerable<Result> BatchClaim(IEnumerable<ClaimOrUnclaim> claims)
        {
            return ClientUtils.WaitTask(BatchClaimAsync(claims));
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IOwnerClient.BatchUnclaim" />
        /// </summary>
        public IEnumerable<Result> BatchUnclaim(IEnumerable<ClaimOrUnclaim> unclaims)
        {
            return ClientUtils.WaitTask(BatchUnclaimAsync(unclaims));
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IOwnerClient.Create" />
        /// </summary>
        public void Create(Owner owner)
        {
            ClientUtils.WaitTask(CreateAsync(owner));
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IOwnerClient.Delete" />
        /// </summary>
        public void Delete(string username)
        {
            ClientUtils.WaitTask(DeleteAsync(username));
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IOwnerClient.Update" />
        /// </summary>
        public void Update(Owner owner, string username)
        {
            ClientUtils.WaitTask(UpdateAsync(owner, username));
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IOwnerClient.UpdatePassword" />
        /// </summary>
        public void UpdatePassword(string username, string password)
        {
            ClientUtils.WaitTask(UpdatePasswordAsync(username, password));
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IOwnerClient.CreateUpdate" />
        /// </summary>
        public IEnumerable<Result> CreateUpdate(IEnumerable<Owner> owners)
        {
            return ClientUtils.WaitTask<IEnumerable<Result>>(CreateUpdateAsync(owners));
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IOwnerClient.OwnerExists" />
        /// </summary>
        public bool OwnerExists(string username)
        {
            return ClientUtils.WaitTask<bool>(OwnerExistsAsync(username));
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IOwnerClient.OwnersExist" />
        /// </summary>
        public IDictionary<string, bool> OwnersExist(IList<string> usernames)
        {
            return ClientUtils.WaitTask<IDictionary<string, bool>>(OwnersExistAsync(usernames));
        }
        #endregion

        #region Async calls
        
        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IOwnerClient.CreateAsync" />
        /// </summary>
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

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IOwnerClient.ClaimAsync" />
        /// </summary>
        public async Task ClaimAsync(string username, string deviceId, Dictionary<string, object> body)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("username cannot be blank.");
            }

            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentException("device_Id cannot be blank.");
            }

            if (body == null || body.Count == 0) {
                await client.sendAsyncRequest(
                    HttpMethod.Post,
                    string.Format("owners/{0}/objects/{1}/claim", username, deviceId)
                );
            } else {
                await client.sendAsyncRequest(
                    HttpMethod.Post,
                    string.Format("owners/{0}/objects/{1}/claim", username, deviceId),
                    JsonConvert.SerializeObject(
                        body,
                        new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            DateFormatString = EventSerializer.DatetimeFormat
                        })
                );
            }
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IOwnerClient.UnclaimAsync" />
        /// </summary>
        public async Task UnclaimAsync(string username, string deviceId, Dictionary<string, object> body)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("username cannot be blank.");
            }

            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentException("device_Id cannot be blank.");
            }

            if (body == null || body.Count == 0) {
                await client.sendAsyncRequest(
                    HttpMethod.Post,
                    string.Format("owners/{0}/objects/{1}/unclaim", username, deviceId)
                );
            } else {
                await client.sendAsyncRequest(
                    HttpMethod.Post,
                    string.Format("owners/{0}/objects/{1}/unclaim", username, deviceId),
                    JsonConvert.SerializeObject(
                        body,
                        new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            DateFormatString = EventSerializer.DatetimeFormat
                        })
                );
            }
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IOwnerClient.BatchClaimAsync" />
        /// </summary>
        public async Task<IEnumerable<Result>> BatchClaimAsync(IEnumerable<ClaimOrUnclaim> claims)
        {
            if (claims == null)
            {
                throw new ArgumentException("claims list cannot be null.");
            }

            if (claims.Count() > BatchMaxSize)
            {
                throw new ArgumentException(string.Format("Claims list cannot be greater than {0}.", BatchMaxSize));
            }

            var asynResult = await client.sendAsyncRequestWithResult(
                HttpMethod.Post,
                "owners/claim",
                ClaimOrUnclaimSerializer.SerializeClaimOrUnclaims(claims)
            );

            return JsonConvert.DeserializeObject<IEnumerable<Result>>(asynResult);
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IOwnerClient.BatchUnclaimAsync" />
        /// </summary>
        public async Task<IEnumerable<Result>> BatchUnclaimAsync(IEnumerable<ClaimOrUnclaim> unclaims)
        {
            if (unclaims == null)
            {
                throw new ArgumentException("unclaims list cannot be null.");
            }

            if (unclaims.Count() > BatchMaxSize)
            {
                throw new ArgumentException(string.Format("Claims list cannot be greater than {0}.", BatchMaxSize));
            }

            var asynResult = await client.sendAsyncRequestWithResult(
                HttpMethod.Post,
                "owners/unclaim",
                ClaimOrUnclaimSerializer.SerializeClaimOrUnclaims(unclaims)
            );

            return JsonConvert.DeserializeObject<IEnumerable<Result>>(asynResult);
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IOwnerClient.UpdateAsync" />
        /// </summary>
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

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IOwnerClient.DeleteAsync" />
        /// </summary>
        public async Task DeleteAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("username cannot be blank.");
            }

            await client.sendAsyncRequest(HttpMethod.Delete,
                string.Format("owners/{0}", username));
        }

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IOwnerClient.UpdatePasswordAsync" />
        /// </summary>
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

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IOwnerClient.CreateUpdateAsync" />
        /// </summary>
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

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IOwnerClient.OwnerExistsAsync" />
        /// </summary>
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

        /// <summary>
        /// Implements <see cref="Mnubo.SmartObjects.Client.IOwnerClient.OwnersExistAsync" />
        /// </summary>
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
