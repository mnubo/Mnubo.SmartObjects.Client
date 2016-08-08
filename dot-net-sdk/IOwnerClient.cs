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
        /// Allows create a new owner in sync mode.
        /// </summary>
        /// <param name="owner">owner to be created</param>
        void Create(Owner owner);

        /// <summary>
        /// Allows an existing owner claim one existing object in sync mode.
        /// </summary>
        /// <param name="username">owner how claim the object.</param>
        /// <param name="deviceId">object claimed.</param>
        void Claim(string username, string deviceId);

        /// <summary>
        /// Allows update an existing owner in sync mode.
        /// </summary>
        /// <param name="owner">new values of the owner to be updated.</param>
        /// <param name="username">Owner to be updated.</param>
        void Update(Owner owner, string username);

        /// <summary>
        /// Allow delete an existing Owner in sync mode.
        /// </summary>
        /// <param name="username">Owner to delete.</param>
        void Delete(String username);

        /// <summary>
        /// Allow update the password of an existing Owner in sync mode.
        /// </summary>
        /// <param name="username">username to update the password.</param>
        /// <param name="password">new password.</param>
        void UpdatePassword(string username, string password);

        /// <summary>
        /// Allow create, or update in case of the owner(s) already exist, a batch of owners in sync mode.
        /// </summary>
        /// <param name="owners">list of onwers to add or update.</param>
        /// <returns>the list of result.</returns>
        IEnumerable<Result> CreateUpdate(IEnumerable<Owner> owners);

        /// <summary>
        /// Allow validate if an owner exists.
        /// </summary>
        /// <param name="username">Owner's username to validate.</param>
        /// <returns>true if the owner exists or false if not.</returns>
        bool OwnerExists(string username);

        /// <summary>
        /// Allow validate if a list of owners exist.
        /// </summary>
        /// <param name="usernames">list of usernames to validate. ["userA", "userB" ]</param>
        /// <returns>the list of usernames with an existing boolean, true if it exists or false if not. [{"userA":true},{"userB":false}]</returns>
        IEnumerable<IDictionary<string, bool>> OwnersExist(IList<string> usernames);

        /// <summary>
        /// Allows create a new owner in async mode.
        /// </summary>
        /// <param name="owner">owner to be created.</param>
        /// <returns>A async task.</returns>
        Task CreateAsync(Owner owner);

        /// <summary>
        /// Allows an existing owner claim one existing object in async mode.
        /// </summary>
        /// <param name="username">owner how claim the object</param>
        /// <param name="deviceId">object claimed</param>
        /// <returns>A async task.</returns>
        Task ClaimAsync(string username, string deviceId);

        /// <summary>
        /// Allows update an existing owner in async mode.
        /// </summary>
        /// <param name="owner">new values of the owner to be updated</param>
        /// <param name="username">Owner to be updated</param>
        /// <returns>A async task.</returns>
        Task UpdateAsync(Owner owner, string username);

        /// <summary>
        /// Allow delete an existing Owner in async mode.
        /// </summary>
        /// <param name="username">Owner to delete</param>
        /// <returns>A async task.</returns>
        Task DeleteAsync(string username);

        /// <summary>
        /// Allow update the password of an existing Owner in async mode.
        /// </summary>
        /// <param name="username">username to update the password.</param>
        /// <param name="password">new password.</param>
        /// <returns>A async task.</returns>
        Task UpdatePasswordAsync(string username, string password);

        /// <summary>
        /// Allow create, or update in case of the owner(s) already exist, a batch of owners in async mode.
        /// </summary>
        /// <param name="owners">list of onwers to add or update.</param>
        /// <returns>the list of result.</returns>
        /// <returns>A async task.</returns>
        Task<IEnumerable<Result>> CreateUpdateAsync(IEnumerable<Owner> owners);

        /// <summary>
        /// Allow validate if an owner exists in async mode.
        /// </summary>
        /// <param name="username">Owner's username to validate.</param>
        /// <returns>true if the owner exists or false if not.</returns>
        Task<bool> OwnerExistsAsync(string username);

        /// <summary>
        /// Allow validate if a list of owners exist in async mode.
        /// </summary>
        /// <param name="usernames">list of usernames to validate. ["userA", "userB" ]</param>
        /// <returns>the list of usernames with an existing boolean, true if it exists or false if not. [{"userA":true},{"userB":false}]</returns>
        Task<IEnumerable<IDictionary<string, bool>>> OwnersExistAsync(IList<string> usernames);
    }
}