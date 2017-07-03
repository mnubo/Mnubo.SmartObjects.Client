using System;
using Mnubo.SmartObjects.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mnubo.SmartObjects.Client
{
    /// <summary>
    /// Owner client interface
    /// </summary>
    public interface IOwnerClient
    {
        /// <summary>
        /// Create a new owner 
        /// </summary>
        /// <param name="owner">owner to be created</param>
        void Create(Owner owner);

        /// <summary>
        /// Claim an object to an owner
        /// </summary>
        /// <param name="username">owner who claims the object.</param>
        /// <param name="deviceId">object claimed.</param>
        /// <param name="body">body to include with the claim.</param>
        void Claim(string username, string deviceId, Dictionary<string, object> body = null);

        /// <summary>
        /// Unclaim an object to an owner 
        /// </summary>
        /// <param name="username">owner how claim the object.</param>
        /// <param name="deviceId">object to unclaim.</param>
        /// <param name="body">body to include with the unclaim.</param>
        void Unclaim(string username, string deviceId, Dictionary<string, object> body = null);

        /// <summary>
        /// Batch claim objects to owners
        /// </summary>
        /// <param name="claims">list of ClaimOrUnclaim.</param>
        /// <returns>A list of Result</returns>
        IEnumerable<Result> BatchClaim(IEnumerable<ClaimOrUnclaim> claims);

        /// <summary>
        /// Batch unclaim objects to owners
        /// </summary>
        /// <param name="unclaims">list of ClaimOrUnclaim.</param>
        /// <returns>A list of Result</returns>
        IEnumerable<Result> BatchUnclaim(IEnumerable<ClaimOrUnclaim> unclaims);

        /// <summary>
        /// Update an existing owner 
        /// </summary>
        /// <param name="owner">new values of the owner to be updated.</param>
        /// <param name="username">Owner to be updated.</param>
        void Update(Owner owner, string username);

        /// <summary>
        /// Delete an existing Owner
        /// </summary>
        /// <param name="username">Owner to delete.</param>
        void Delete(String username);

        /// <summary>
        /// Update the password of an existing Owner 
        /// </summary>
        /// <param name="username">username to update the password.</param>
        /// <param name="password">new password.</param>
        void UpdatePassword(string username, string password);

        /// <summary>
        /// Create, or update in case of the owner(s) already exist, a batch of owners
        /// </summary>
        /// <param name="owners">list of onwers to add or update.</param>
        /// <returns>the list of result.</returns>
        IEnumerable<Result> CreateUpdate(IEnumerable<Owner> owners);

        /// <summary>
        /// Validate if an owner exists.
        /// </summary>
        /// <param name="username">Owner's username to validate.</param>
        /// <returns>true if the owner exists or false if not.</returns>
        bool OwnerExists(string username);

        /// <summary>
        /// Validate if a list of owners exist.
        /// </summary>
        /// <param name="usernames">list of usernames to validate. ["userA", "userB" ]</param>
        /// <returns>the dictionary of usernames with an existing boolean, true if it exists or false if not. {"userA":true},{"userB":false}</returns>
        IDictionary<string, bool> OwnersExist(IList<string> usernames);

        /// <summary>
        /// Create a new owner in async mode.
        /// </summary>
        /// <param name="owner">owner to be created.</param>
        /// <returns>A async task.</returns>
        Task CreateAsync(Owner owner);

        /// <summary>
        /// Claim an object to an owner in async mode.
        /// </summary>
        /// <param name="username">owner who claims the object</param>
        /// <param name="deviceId">object claimed</param>
        /// <param name="body">body to include with the claim.</param>
        /// <returns>A async task.</returns>
        Task ClaimAsync(string username, string deviceId, Dictionary<string, object> body = null);

        /// <summary>
        /// Unclaim an object to an owner in async mode.
        /// </summary>
        /// <param name="username">owner how claim the object.</param>
        /// <param name="deviceId">object to unclaim.</param>
        /// <param name="body">body to include with the unclaim.</param>
        Task UnclaimAsync(string username, string deviceId, Dictionary<string, object> body = null);

        /// <summary>
        /// Batch claim objects to owners in async mode.
        /// </summary>
        /// <param name="claims">list of ClaimOrUnclaim.</param>
        /// <returns>A async task with a list of Result.</returns>
        Task<IEnumerable<Result>> BatchClaimAsync(IEnumerable<ClaimOrUnclaim> claims);

        /// <summary>
        /// Batch unclaim objects to owners in async mode.
        /// </summary>
        /// <param name="unclaims">list of ClaimOrUnclaim.</param>
        /// <returns>A async task with a list of Result.</returns>
        Task<IEnumerable<Result>> BatchUnclaimAsync(IEnumerable<ClaimOrUnclaim> unclaims);

        /// <summary>
        /// Update an existing owner in async mode.
        /// </summary>
        /// <param name="owner">new values of the owner to be updated</param>
        /// <param name="username">Owner to be updated</param>
        /// <returns>A async task.</returns>
        Task UpdateAsync(Owner owner, string username);

        /// <summary>
        /// Delete an existing Owner in async mode.
        /// </summary>
        /// <param name="username">Owner to delete</param>
        /// <returns>A async task.</returns>
        Task DeleteAsync(string username);

        /// <summary>
        /// Update the password of an existing Owner in async mode.
        /// </summary>
        /// <param name="username">username to update the password.</param>
        /// <param name="password">new password.</param>
        /// <returns>A async task.</returns>
        Task UpdatePasswordAsync(string username, string password);

        /// <summary>
        /// Create, or update in case of the owner(s) already exist, a batch of owners in async mode.
        /// </summary>
        /// <param name="owners">list of onwers to add or update.</param>
        /// <returns>the list of result.</returns>
        /// <returns>A async task.</returns>
        Task<IEnumerable<Result>> CreateUpdateAsync(IEnumerable<Owner> owners);

        /// <summary>
        /// Validate if an owner exists in async mode.
        /// </summary>
        /// <param name="username">Owner's username to validate.</param>
        /// <returns>true if the owner exists or false if not.</returns>
        Task<bool> OwnerExistsAsync(string username);

        /// <summary>
        /// Validate if a list of owners exist in async mode.
        /// </summary>
        /// <param name="usernames">list of usernames to validate. ["userA", "userB" ]</param>
        /// <returns>the dictionary of usernames with an existing boolean, true if it exists or false if not. {"userA":true},{"userB":false}</returns>
        Task<IDictionary<string, bool>> OwnersExistAsync(IList<string> usernames);
    }
}
