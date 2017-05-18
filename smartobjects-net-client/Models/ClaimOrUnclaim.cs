using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Mnubo.SmartObjects.Client.Models
{
    /// <summary>
    ///  ClaimOrUnclaim are to be used within batch claim or unclaim
    /// </summary>
    public sealed class ClaimOrUnclaim
    {
        /// <summary>
        /// Get the username
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Get the device id
        /// </summary>
        public string DeviceId { get; }

        /// <summary>
        /// Get the attributes
        /// </summary>
        public IImmutableDictionary<string, object> Attributes { get; }

        /// <summary>
        /// Constructor to build a ClaimOrUnclaim instance
        /// <param name="username">See <see cref="Username" /></param>
        /// <param name="deviceId">See <see cref="DeviceId" /></param>
        /// <param name="attributes">See <see cref="Attributes" /></param>
        /// </summary>
        public ClaimOrUnclaim(
            string username,
            string deviceId,
            IDictionary<string, object> attributes = null)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("username cannot be blank.");
            }
            
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentException("deviceId cannot be blank.");
            }
            Username = username;
            DeviceId = deviceId;

            var attributesBuilder = ImmutableDictionary.CreateBuilder<string, object>();

            if(attributes != null) {
                foreach (KeyValuePair<string, object> attribute in attributes) {
                    attributesBuilder.Add(attribute.Key, attribute.Value);
                }    
            }
            
            Attributes = attributesBuilder.ToImmutable();
        }
    }
}