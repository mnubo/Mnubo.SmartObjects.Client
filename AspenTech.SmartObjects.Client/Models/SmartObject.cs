using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace AspenTech.SmartObjects.Client.Models
{
    /// <summary>
    /// SmartObject Bean. To build a SmartObject you must usign SmartObjectBuilder class. Note that instances of this object are immutables.
    /// </summary>
    public sealed class SmartObject
    {
        /// <summary>
        /// Get the deviceid.
        /// </summary>
        public string DeviceId { get; }

        /// <summary>
        /// Get a nullable GUID objectId.
        /// </summary>
        public Guid? ObjectId { get; }

        /// <summary>
        /// Get the type of the object.
        /// </summary>
        public string ObjectType { get; }

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
        /// Get the username
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Get the attributes
        /// </summary>
        public IImmutableDictionary<string, object> Attributes { get; }

		private SmartObject(
            string deviceId,
            Guid? objectId,
            string objectType,
            DateTimeOffset? registrationDateTime,
            string username,
            IDictionary<string, object> attributes)
        {
            DeviceId = deviceId;
            ObjectId = objectId;
            ObjectType = objectType;
            Username = username;

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
        /// SmartObject builder class. Use this class to build a new SmartObject instance
        /// </summary>
        public sealed class Builder
        {
            /// <summary>
            /// Build a new immutable AspenTech's Owner instance from the builder
            /// </summary>
            /// <param name="builder">AspenTech's cient Config builder</param>
            public static implicit operator SmartObject(Builder builder)
            {
                return builder.Build();
            }

            /// <summary>
            /// The deviceid
            /// </summary>
            public string DeviceId { get; set; }

            /// <summary>
            /// Nullable GUID objectId.
            /// </summary>
            public Guid? ObjectId { get; set; }

            /// <summary>
            /// The type of the object
            /// </summary>
            public string ObjectType { get; set; }

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
            /// The owner's username
            /// </summary>
            public string Username { get; set; }

            /// <summary>
            /// The attributes
            /// </summary>
            public IDictionary<string, object> Attributes { get; set; }

            /// <summary>
            /// Constructor for the builder
            /// </summary>
            public Builder()
            {
                DeviceId = null;
                ObjectId = null;
                ObjectType = null;
                RegistrationDate = null;
                RegistrationDateTime = null;
                Username = null;
                Attributes = new Dictionary<string, object>();
            }

            /// <summary>
            /// Build the SmartObject instance.
            /// </summary>
            /// <returns>Return a SmartObject built</returns>
            public SmartObject Build()
                {
                    DateTimeOffset? registrationDateTime =
                        (!RegistrationDateTime.HasValue && RegistrationDate.HasValue) ?
                        new DateTimeOffset(RegistrationDate.Value) : RegistrationDateTime;

                    return new SmartObject(DeviceId, ObjectId, ObjectType, registrationDateTime, Username, Attributes);
                }
            }
    }
}