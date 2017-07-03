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
        [System.Obsolete("RegistrationDate is deprecated. Use RegistrationDateTime which supports timezone")]
        public DateTime? RegistrationDate { get; }

        /// <summary>
        /// Get a nullable registration date
        /// </summary>
        public DateTimeOffset? RegistrationDateTime { get; }


        /// <summary>
        /// Get the attributes.
        /// </summary>
        public IImmutableDictionary<string, object> Attributes { get; }

        private Owner(
            string username,
            string password,
            DateTimeOffset? registrationDateTime,
            IDictionary<string, object> attributes)
        {
            Username = username;
            Password = password;

            RegistrationDate = registrationDateTime?.UtcDateTime;
            RegistrationDateTime = registrationDateTime;

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
            /// Build a new immutable mnubo's Owner instance from the builder.
            /// </summary>
            /// <param name="builder">Mnubo's cient Config builder</param>
            public static implicit operator Owner(Builder builder)
            {
                return builder.Build();
            }

            /// <summary>
            /// The username
            /// </summary>
            public string Username { get; set; }

            /// <summary>
            /// The password
            /// </summary>
            public string Password { get; set; }

            /// <summary>
            /// Get a nullable registration date
            /// </summary>
            [System.Obsolete("RegistrationDate is deprecated. Use RegistrationDateTime which supports timezone")]
            public DateTime? RegistrationDate { get; set; }

            /// <summary>
            /// Get a nullable registration date
            /// </summary>
            public DateTimeOffset? RegistrationDateTime { get; set; }

            /// <summary>
            /// The attributes.
            /// </summary>
            public IDictionary<string, object> Attributes { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            public Builder()
            {
                Username = null;
                Password = null;
                RegistrationDate = null;
                RegistrationDateTime = null;
                Attributes = new Dictionary<String, Object>();
            }

            /// <summary>
            /// Build the Owner instance
            /// </summary>
            /// <returns>Return a Owner built</returns>
            public Owner Build()
            {
                DateTimeOffset? registrationDateTime =
                        (!RegistrationDateTime.HasValue && RegistrationDate.HasValue) ?
                        new DateTimeOffset(RegistrationDate.Value) : RegistrationDateTime;

                return new Owner(Username, Password, registrationDateTime, Attributes);
            }
        }
    }
}