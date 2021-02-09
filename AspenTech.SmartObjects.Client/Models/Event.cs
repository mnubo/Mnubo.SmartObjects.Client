﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace AspenTech.SmartObjects.Client.Models
{
    /// <summary>
    /// Represent a single event to be sent to AspenTech. An event can have
    /// multitple timeseries (measurements) in it.
    /// </summary>
    public sealed class Event
    {
        /// <summary>
        /// Get a nullable event ID.
        /// </summary>
        /// <value>The event id.</value>
        public Guid? EventId { get; }

        /// <summary>
        /// Get the device Id
        /// </summary>
        public string DeviceId { get; }

        /// <summary>
        /// Get the event type.
        /// </summary>
        public string EventType { get; }

        /// <summary>
        /// Get a nullable timestamp
        /// </summary>
        public DateTime? Timestamp { get; }

        /// <summary>
        /// get a immutable timeseries.
        /// </summary>
        public IImmutableDictionary<string, object> Timeseries { get; }

        private Event(
            Guid? eventId,
            string deviceId,
            string eventType,
            DateTime? timestamp,
            IDictionary<string, object> timeseries)
        {
            if (string.IsNullOrEmpty(eventType))
            {
                throw new ArgumentException("x_event_type cannot be null or empty");
            }

            EventId = eventId;
            DeviceId = deviceId;
            EventType = eventType;

            if (timestamp.HasValue)
            {
                Timestamp = timestamp;
            }

            var timeseriesBuilder = ImmutableDictionary.CreateBuilder<string, object>();

            foreach (KeyValuePair<string, object> attribute in timeseries)
            {
                timeseriesBuilder.Add(attribute.Key, attribute.Value);
            }
            Timeseries = timeseriesBuilder.ToImmutable();
        }

        /// <summary>
        /// Event builder class, Use this class to build a new Event intance.
        /// </summary>
        public sealed class Builder
        {
            /// <summary>
            /// build a new immutable mnubo's Event instance from the builder.
            /// </summary>
            /// <param name="builder">Mnubo's cient Config builder</param>
            public static implicit operator Event(Builder builder)
            {
                return builder.Build();
            }

            /// <summary>
            /// Nullable event ID.
            /// </summary>
            /// <value>The event id.</value>
            public Guid? EventId { get; set; }

            /// <summary>
            /// The smart object instance.
            /// </summary>
            public string DeviceId { get; set; }

            /// <summary>
            /// The event type.
            /// </summary>
            public string EventType { get; set; }

            /// <summary>
            /// Nullable timestamp
            /// </summary>
            public DateTime? Timestamp { get; set; }

            /// <summary>
            /// Timeseries.
            /// </summary>
            public IDictionary<string, object> Timeseries { get; set; }

            /// <summary>
            /// Represent a single event to be sent to AspenTech. An event can have
            /// multitple timeseries (measurements) in it.
            /// </summary>
            public Builder()
            {
                EventId = null;
                DeviceId = null;
                EventType = null;
                Timestamp = null;
                Timeseries = new Dictionary<string, object>();
            }

            /// <summary>
            /// Build the event instance.
            /// </summary>
            /// <returns>Return the event built</returns>
            public Event Build()
            {
                return new Event(EventId, DeviceId, EventType, Timestamp, Timeseries);
            }
        }
    }
}
