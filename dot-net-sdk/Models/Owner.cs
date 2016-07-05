using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Mnubo.SmartObjects.Client.Models
{
    /// <summary>
    ///  Owner Bean. To build an Owner you must use OwnerBuilder class. Note that instances of this object are immutables.
    /// </summary>
    public sealed class Owner
    {
        /// <summary>
        /// Get the username.
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Get the password.
        /// </summary>
        public string Password { get; }

        /// <summary>
        /// Get a nullable registration date.
        /// </summary>
        public DateTime? RegistrationDate { get; }

        /// <summary>
        /// Get the attributes.
        /// </summary>
        public IImmutableDictionary<string, object> Attributes { get; }

        private Owner(
            string username,
            string password,
            DateTime? registrationDate,
            IDictionary<string, object> attributes)
        {
            Username = username;
            Password = password;

            if (registrationDate.HasValue)
            {
                RegistrationDate = registrationDate;
            }

            var attributesBuilder = ImmutableDictionary.CreateBuilder<string, object>();

            foreach (KeyValuePair<string, object> attribute in attributes)
            {
                attributesBuilder.Add(attribute.Key, attribute.Value);
            }
            Attributes = attributesBuilder.ToImmutable();
        }

        /// <summary>
        /// Owner builder class, Use this class to build a new Owner intance.
        /// </summary>
        public sealed class Builder
        {
            /// <summary>
            /// build a new immutable mnubo's Owner instance from the builder.
            /// </summary>
            /// <param name="builder">Mnubo's cient Config builder</param>
            public static implicit operator Owner(Builder builder)
            {
                return builder.Build();
            }

            /// <summary>
            /// The username.
            /// </summary>
            public string Username { get; set; }

            /// <summary>
            /// The password.
            /// </summary>
            public string Password { get; set; }

            /// <summary>
            /// nullable registration date.
            /// </summary>
            public DateTime? RegistrationDate { get; set; }

            /// <summary>
            /// The attributes.
            /// </summary>
            public IDictionary<string, object> Attributes { get; set; }

            public Builder()
            {
                Username = null;
                Password = null;
                RegistrationDate = null;
                Attributes = new Dictionary<String, Object>();
            }

            /// <summary>
            /// Build the Owner instance.
            /// </summary>
            /// <returns>Return a Owner built</returns>
            public Owner Build()
            {
                return new Owner(Username, Password, RegistrationDate, Attributes);
            }
        }
    }
}