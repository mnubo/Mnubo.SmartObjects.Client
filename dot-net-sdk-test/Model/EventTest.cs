using System;
using System.Collections.Generic;
using Mnubo.SmartObjects.Client.Models;
using NUnit.Framework;

namespace Mnubo.SmartObjects.Client.Test.Model
{
    [TestFixture()]
    public class EventTest
    {
        [Test()]
        public void EventBuilderTest()
        {
            DateTime now = DateTime.UtcNow;
            Guid eventId = Guid.NewGuid();
            Dictionary<string, object> timeseries = new Dictionary<string, object>();
            timeseries.Add("String", "text");

            Event eventBuilt = new Event.Builder()
            { 
                EventId = eventId,
                EventType = "event type",
                DeviceId = "Device Id",
                Timestamp = now,
                Timeseries = new Dictionary<string, object>()
                {
                    { "String", "text" }
                }
            };

            Assert.AreEqual(eventBuilt.EventId, eventId);
            Assert.True(eventBuilt.EventType.Equals("event type"));
            Assert.True(eventBuilt.SmartObject.DeviceId.Equals("Device Id"));
            Assert.AreEqual(eventBuilt.Timestamp, now);
            CollectionAssert.AreEqual(eventBuilt.Timeseries, timeseries);
        }

        [Test()]
        public void EventBuilderTestWithObject()
        {
            DateTime now = DateTime.UtcNow;
            Guid eventId = Guid.NewGuid();
            Dictionary<string, object> timeseries = new Dictionary<string, object>();
            timeseries.Add("String", "text");

            SmartObject myObject = new SmartObject.Builder()
            {
                DeviceId = "Device Id"
            };

            Event eventBuilt = new Event.Builder()
            {
                EventId = eventId,
                EventType = "event type",
                DeviceId = myObject.DeviceId,
                Timestamp = now,
                Timeseries = new Dictionary<string, object>()
                {
                    { "String", "text" }
                }
            };

            Assert.AreEqual(eventBuilt.EventId, eventId);
            Assert.True(eventBuilt.EventType.Equals("event type"));
            Assert.True(eventBuilt.SmartObject.DeviceId.Equals("Device Id"));
            Assert.AreEqual(eventBuilt.Timestamp, now);
            CollectionAssert.AreEqual(eventBuilt.Timeseries, timeseries);
        }

        [Test()]
        public void EventBuilderAddTimeseriesTest()
        {
            Dictionary<string, object> attributes = new Dictionary<string, object>();
            attributes.Add("String", "text");
            attributes.Add("int", 10);
            attributes.Add("double", 10.7);
            attributes.Add("boolean", true);

            Event eventBuilt = new Event.Builder()
            {
                EventType = "event type",
                Timeseries = new Dictionary<string, object>()
                {
                    { "String", "text" },
                    { "int", 10 },
                    { "double", 10.7 },
                    { "boolean", true }
                }
            };

            Assert.IsNull(eventBuilt.EventId);
            Assert.True(eventBuilt.EventType.Equals("event type"));
            Assert.IsNull(eventBuilt.SmartObject);
            Assert.IsNull(eventBuilt.Timestamp);
            CollectionAssert.AreEqual(eventBuilt.Timeseries, attributes);
        }

        [Test()]
        public void EventBuilderEventEmpty()
        {
            Event eventBuilt = new Event.Builder()
            {
                EventType = "event type"
            };

            Assert.IsNull(eventBuilt.EventId);
            Assert.IsNull(eventBuilt.SmartObject);
            Assert.IsNull(eventBuilt.Timestamp);
            Assert.True(eventBuilt.Timeseries.Count == 0);
            Assert.True(eventBuilt.EventType.Equals("event type"));
        }

        [Test()]
        public void EventBuilderEventTypeNotFill()
        {
            Assert.That(() => new Event.Builder().Build(),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("x_event_type cannot be null or empty"));
        }

        [Test()]
        public void EventBuilderEventTypeNull()
        {
            Event eventBuilt;

            Assert.That(() => eventBuilt = new Event.Builder()
            {
                EventType = null,
            },
            Throws.TypeOf<ArgumentException>()
            .With.Message.EqualTo("x_event_type cannot be null or empty"));
        }

        [Test()]
        public void EventBuilderEventTypeEmpty()
        {
            Event eventBuilt;

            Assert.That(() => eventBuilt = new Event.Builder()
            {
                EventType = "",
            },
            Throws.TypeOf<ArgumentException>()
            .With.Message.EqualTo("x_event_type cannot be null or empty"));
        }
    }
}