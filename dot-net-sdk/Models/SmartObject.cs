using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Mnubo.SmartObjects.Client.Models
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
        /// Get a nulable registration date.
        /// </summary>
        public DateTime? RegistrationDate { get; }

        /// <summary>
        /// Get the username.
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Get the attributes.
        /// </summary>
        public IImmutableDictionary<string, object> Attributes { get; }

        /// <summary>
        /// Get a nullable GUID event identifier.
        /// </summary>
        public Guid? EventId { get; }

		private SmartObject(
            string deviceId,
            Guid? objectId,
            string objectType,
            DateTime? registrationDate,
            string username,
            IDictionary<string, object> attributes,
            Guid? eventId)
        {
            this.DeviceId = deviceId;
            this.EventId = eventId;
            this.ObjectId = objectId;
            this.ObjectType = objectType;
            this.Username = username;

            if (registrationDate.HasValue)
            {
                this.RegistrationDate = registrationDate;
            }

            var attributesBuilder = ImmutableDictionary.CreateBuilder<string, object>();

            foreach (KeyValuePair<string, object> attribute in attributes)
            {
                attributesBuilder.Add(attribute.Key, attribute.Value);
            }
            this.Attributes = attributesBuilder.ToImmutable();
        }

        /// <summary>
        /// SmartObject builder class. Use this class to build a new SmartObject intance.
        /// </summary>
        public sealed class Builder
        {
            /// <summary>
            /// build a new immutable mnubo's Owner instance from the builder.
            /// </summary>
            /// <param name="builder">Mnubo's cient Config builder</param>
            public static implicit operator SmartObject(Builder builder)
            {
                return builder.Build();
            }

            /// <summary>
            /// the deviceid.
            /// </summary>
            public string DeviceId { get; set; }

            /// <summary>
            /// nullable GUID objectId.
            /// </summary>
            public Guid? ObjectId { get; set; }

            /// <summary>
            /// the type of the object.
            /// </summary>
            public string ObjectType { get; set; }

            /// <summary>
            /// nullable registration date.
            /// </summary>
            public DateTime? RegistrationDate { get; set; }

            /// <summary>
            /// the owner.
            /// </summary>
            public string Username { get; set; }

            /// <summary>
            /// the attributes.
            /// </summary>
            public IDictionary<string, object> Attributes { get; set; }

            /// <summary>
            /// nullable GUID event identifier.
            /// </summary>
            public Guid? EventId { get; set; }

            public Builder()
            {
                DeviceId = null;
                ObjectId = null;
                ObjectType = null;
                RegistrationDate = null;
                Username = null;
                EventId = null;
                Attributes = new Dictionary<string, object>();
            }

            /// <summary>
            /// Build the SmartObject instance.
            /// </summary>
            /// <returns>Return a SmartObject built</returns>
            public SmartObject Build()
                {
                    return new SmartObject(DeviceId, ObjectId, ObjectType, RegistrationDate, Username, Attributes, EventId);
                }
            }
    }
}