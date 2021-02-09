using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Collections.Immutable;
using AspenTech.SmartObjects.Client.Impl;
using AspenTech.SmartObjects.Client.Models;

namespace AspenTech.SmartObjects.Client.Test.Impl
{
    [TestFixture()]
    public class EventSerializerTest
    {
        internal const string DatetimeFormat = "yyyy-MM-dd'T'HH:mm:ssZ";

        [Test()]
        public void EventSerializeTest()
        {
            DateTime now = TestUtils.GetNowIgnoringMilis();

            Dictionary<string, object> timeseries = new Dictionary<string, object>();
            timeseries.Add("string", "stringValue");
            timeseries.Add("double", 10d);
            timeseries.Add("float", 10.5f);
            timeseries.Add("boolean", false);

            Event eventBuilt = new Event.Builder()
            {
                EventId = Guid.Parse("98c62f5c-ad48-4ef8-8d70-dbe3a1e8b17f"),
                Timestamp = now,
                EventType = "type",
                Timeseries = new Dictionary<string, object>()
                {
                    { "string", "stringValue" },
                    { "double", 10d },
                    { "float", 10.5f },
                    { "boolean", false }
                }
            };

            string json = EventSerializer.SerializeEvent(eventBuilt);
			TestUtils.AssertJsonEquals(json, new List<string>
			{
				"\"x_event_type\":\"type\"",
				$"\"x_timestamp\":\"{now.ToString(DatetimeFormat)}\"",
				"\"event_id\":\"98c62f5c-ad48-4ef8-8d70-dbe3a1e8b17f\"",
				"\"boolean\":false",
				"\"float\":10.5",
				"\"double\":10.0",
				"\"string\":\"stringValue\""
			});
        }

        [Test()]
        public void EventSerializeTestWithOwner()
        {
            Event eventBuilt = new Event.Builder()
            {
                EventType = "test",
                DeviceId = "deviceId"
            };

            string json = EventSerializer.SerializeEvent(eventBuilt);
            Assert.AreEqual(
                "{\"x_event_type\":\"test\",\"x_object\":{\"x_device_id\":\"deviceId\"}}",
                json);
        }

        [Test()]
        public void EventSerializeTestAttributeList()
        {
            Dictionary<string, object> timeseries = new Dictionary<string, object>();
            timeseries.Add("list", new List<string>() { "1", "2" });

            Event eventBuilt = new Event.Builder()
            {
                Timeseries = new Dictionary<string, object>()
                {
                    { "list", new List<string>() { "1", "2" } }
                },
                EventType = "test"
            };

            string json = EventSerializer.SerializeEvent(eventBuilt);
            Assert.AreEqual("{\"x_event_type\":\"test\",\"list\":[\"1\",\"2\"]}", json);
        }

        [Test()]
        public void EventSerializeTestAttributeListAsInt()
        {
            Dictionary<string, object> timeseries = new Dictionary<string, object>();
            timeseries.Add("list", new List<int>() { 1, 2 });

            Event eventBuilt = new Event.Builder()
            {
                Timeseries = new Dictionary<string, object>()
                {
                    { "list", new List<int>() { 1, 2 } }
                },
                EventType = "test"
            };

            string json = EventSerializer.SerializeEvent(eventBuilt);
            Assert.AreEqual("{\"x_event_type\":\"test\",\"list\":[1,2]}", json);
        }

        [Test()]
        public void EventSerializeTestWithEmptyObject()
        {
            Event eventBuilt = new Event.Builder()
            {
                EventType = "test"
            };

            string json = EventSerializer.SerializeEvent(eventBuilt);
            Assert.AreEqual("{\"x_event_type\":\"test\"}", json);
        }

        [Test()]
        public void EventSerializeTestWithoutEventType()
        {
            Assert.That(() => new Event.Builder().Build(),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("x_event_type cannot be null or empty"));
        }

        [Test()]
        public void EventDeserializeTest()
        {
            DateTime now = TestUtils.GetNowIgnoringMilis();

            string json =
                "{\"x_event_type\":\"type\"," +
                "\"x_timestamp\":\"" + now.ToString(DatetimeFormat) + "\"," +
                "\"age\": 89," +
                "\"weight\": 125.5," +
                "\"married\": true," +
                "\"counter\": -13582," +
                "\"list_SmartObject\": [\"val1\",\"val2\",\"val3\"]," +
                "\"x_object\" : { \"x_device_id\":\"deviceID\",\"sometrash\":8888}," +
                "\"event_id\":\"46aabccd-4442-6665-a1f0-49889330eaf3\"}";

            var attributes = ImmutableDictionary.CreateBuilder<string, object>();
            attributes.Add("age", 89);
            attributes.Add("weight", 125.5);
            attributes.Add("married", true);
            attributes.Add("counter", -13582);
            attributes.Add("list_SmartObject", (Object)new string[] { "val1", "val2", "val3" });

            Event eventBuilt = EventSerializer.DeserializeEvent(json);

            Assert.AreEqual(eventBuilt.EventId.ToString(), "46aabccd-4442-6665-a1f0-49889330eaf3");
            Assert.AreEqual(eventBuilt.EventType, "type");
            Assert.AreEqual(eventBuilt.DeviceId, "deviceID");
            Assert.AreEqual(eventBuilt.Timestamp.Value.ToString(DatetimeFormat),
                now.ToString(DatetimeFormat));
            CollectionAssert.AreEqual(eventBuilt.Timeseries, attributes.ToImmutable());
        }

        [Test()]
        public void EventDeserializeTestWithSmartObjectNull()
        {
            DateTime now = TestUtils.GetNowIgnoringMilis();

            string json =
                "{\"x_event_type\":\"type\"," +
                "\"x_object\" : { } }";

            Event eventBuilt = EventSerializer.DeserializeEvent(json);

            Assert.IsNull(eventBuilt.EventId);
            Assert.AreEqual(eventBuilt.EventType, "type");
            Assert.IsNull(eventBuilt.DeviceId);
            Assert.IsNull(eventBuilt.Timestamp);
            Assert.AreEqual(eventBuilt.Timeseries.Count, 0);
        }

        [Test()]
        public void EventDeserializeTestWithSmartDeviceIdNull()
        {
            DateTime now = TestUtils.GetNowIgnoringMilis();

            string json =
                "{\"x_event_type\":\"type\"," +
                "\"x_object\" : { \"sometrash\":8888 } }";

            Event eventBuilt = EventSerializer.DeserializeEvent(json);

            Assert.IsNull(eventBuilt.EventId);
            Assert.AreEqual(eventBuilt.EventType, "type");
            Assert.IsNull(eventBuilt.DeviceId);
            Assert.IsNull(eventBuilt.Timestamp);
            Assert.AreEqual(eventBuilt.Timeseries.Count, 0);
        }

        [Test()]
        public void EventDeserializeTestCheckNull()
        {
            string json = "{}";

            Assert.That(() => EventSerializer.DeserializeEvent(json),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("x_event_type cannot be null or empty"));
        }

        [Test()]
        public void EventDeserializeTestCheckNullWithEventType()
        {
            string json = "{\"x_event_type\":\"type\"}";

            Event eventBuilt = EventSerializer.DeserializeEvent(json);

            Assert.AreEqual(eventBuilt.EventType, "type");
            Assert.IsNull(eventBuilt.Timestamp);
            Assert.IsNull(eventBuilt.EventId);
            Assert.IsNull(eventBuilt.DeviceId);
            Assert.IsNotNull(eventBuilt.Timeseries);
            Assert.AreEqual(eventBuilt.Timeseries.Count, 0);
        }

        [Test()]
        public void EventDeserializeTestWrongEventIdType()
        {
            string json = "{\"event_id\":\"54545c5454-054-54\",\"string\":\"stringValue\"}";

            Assert.That(() => EventSerializer.DeserializeEvent(json),
                Throws.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo("Field 'x_event_id' does not match TYPE 'GUID'"));
        }


        [Test()]
        public void EventDeserializeTestWrongEventType()
        {
            string json = "{\"x_event_type\":false}";

            Assert.That(() => EventSerializer.DeserializeEvent(json),
                Throws.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo("Field 'x_event_type' does not match TYPE 'TEXT'"));
        }

        [Test()]
        public void EventDeserializeTestWrongTimestampType()
        {
            string json = "{\"x_timestamp\":\"544578\"}";

            Assert.That(() => EventSerializer.DeserializeEvent(json),
                Throws.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo("Field 'x_timestamp' does not match TYPE 'DATETIME'"));
        }

        [Test()]
        public void EventDeserializeTestWrongSmartObjectType()
        {
            string json = "{\"x_object\":\"5454544578f\"}";

            Assert.That(() => EventSerializer.DeserializeEvent(json),
                Throws.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo("Field 'x_object' does not match TYPE 'SMARTOBJECT'"));
        }
    }
}