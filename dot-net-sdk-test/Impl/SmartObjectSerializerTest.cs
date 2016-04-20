using System;
using System.Collections.Generic;
using NUnit.Framework;
using Mnubo.SmartObjects.Client.Models;
using Mnubo.SmartObjects.Client.Impl;
using System.Collections.Immutable;
using Mnubo.SmartObjects.Client.Test.Impl;

namespace Con.Mnubo.Dotnetsdktest.Test.Impl
{
    [TestFixture()]
    public class SmartObjectSerializerTest
    {
        [Test()]
        public void SmartObjectSerializeTest()
        {
            DateTime now = DateTime.UtcNow;

            Dictionary<string, object> attributes = new Dictionary<string, object>();
            attributes.Add("string", "stringValue");
            attributes.Add("double", 10d);
            attributes.Add("float", 10.5f);

            SmartObject myObject = new SmartObject.Builder()
            {
                EventId = Guid.Parse("98c62f5c-ad48-4ef8-8d70-dbe3a1e8b17f"),
                DeviceId = "test",
                ObjectType = "type",
                RegistrationDate = now,
                Attributes = new Dictionary<string, object>()
                {
                    { "string", "stringValue" },
                    { "double", 10d },
                    { "float", 10.5f }
                }
            };

            string json = SmartObjectSerializer.SerializeSmartObject(myObject);
            Assert.AreEqual(
                "{\"x_device_id\":\"test\"," +
                "\"x_registration_date\":\"" + now.ToString(EventSerializerTest.DatetimeFormat) + "\"," +
                "\"x_object_type\":\"type\"," +
                "\"event_id\":\"98c62f5c-ad48-4ef8-8d70-dbe3a1e8b17f\"," +
                "\"float\":10.5," +
                "\"double\":10.0," +
                "\"string\":\"stringValue\"}",
                json);
        }

        [Test()]
        public void SmartObjectSerializeTestWithSmartObject()
        {
            SmartObject myObject = new SmartObject.Builder()
            {
                EventId = Guid.Parse("671c8315-952b-4c69-8c37-d2d58a64af9e"),
                DeviceId = "test",
                Username = "owner"
            };

            string json = SmartObjectSerializer.SerializeSmartObject(myObject);
            Assert.AreEqual(
                "{\"x_device_id\":\"test\",\"event_id\":\"671c8315-952b-4c69-8c37-d2d58a64af9e\",\"x_owner\":{\"username\":\"owner\"}}",
                json);
        }

        [Test()]
        public void SmartObjectSerializeTestAttributeList()
        {
            Dictionary<string, object> attributes = new Dictionary<string, object>();
            attributes.Add("list", new List<string>() { "1", "2" });

            SmartObject myObject = new SmartObject.Builder()
            {
                Attributes = new Dictionary<string, object>()
                {
                    { "list", new List<string>() { "1", "2" } }
                }
            };

            string json = SmartObjectSerializer.SerializeSmartObject(myObject);
            Assert.AreEqual("{\"list\":[\"1\",\"2\"]}", json);
        }

        [Test()]
        public void SmartObjectSerializeTestWithEmptyObject()
        {

            SmartObject myObject = new SmartObject.Builder().Build();

            string json = SmartObjectSerializer.SerializeSmartObject(myObject);
            Assert.AreEqual("{}", json);
        }

        [Test()]
        public void SmartObjectDeserializeTest()
        {
            DateTime now = DateTime.UtcNow;

            string json =
                "{\"x_device_id\":\"test\"," +
                "\"x_object_type\":\"type\"," +
                "\"x_registration_date\":\"" + now.ToString(EventSerializerTest.DatetimeFormat) + "\"," +
                "\"age\": 89," +
                "\"weight\": 125.5," +
                "\"married\": true," +
                "\"counter\": -13582," +
                "\"list_SmartObject\": [\"val1\",\"val2\",\"val3\"]," +
                "\"x_owner\" : { \"username\":\"owner\",\"sometrash\":8888}," +
                "\"event_id\":\"46aabccd-4442-6665-a1f0-49889330eaf3\"}";

            var attributes = ImmutableDictionary.CreateBuilder<string, object>();
            attributes.Add("age", 89);
            attributes.Add("weight", 125.5);
            attributes.Add("married", true);
            attributes.Add("counter", -13582);
            attributes.Add("list_SmartObject", new string[] { "val1", "val2", "val3" });

            SmartObject SmartObject = SmartObjectSerializer.DeserializeSmartObject(json);

            Assert.AreEqual(SmartObject.DeviceId, "test");
            Assert.AreEqual(SmartObject.EventId.ToString(), "46aabccd-4442-6665-a1f0-49889330eaf3");
            Assert.AreEqual(SmartObject.ObjectType, "type");
            Assert.AreEqual(SmartObject.Owner.Username, "owner");
            Assert.IsNull(SmartObject.Owner.Password);
            Assert.IsNull(SmartObject.Owner.RegistrationDate);
            Assert.IsNull(SmartObject.Owner.EventId);
            Assert.IsNotNull(SmartObject.Owner.Attributes);
            Assert.AreEqual(SmartObject.Owner.Attributes.Count, 0);
            Assert.AreEqual(SmartObject.RegistrationDate.Value.ToString(EventSerializerTest.DatetimeFormat),
                now.ToString(EventSerializerTest.DatetimeFormat));
            CollectionAssert.AreEqual(SmartObject.Attributes, attributes.ToImmutable());
        }

        [Test()]
        public void SmartObjectDeserializeTestWithOwnerNull()
        {
            string json =
                "{\"x_owner\" : { } }";

            SmartObject SmartObject = SmartObjectSerializer.DeserializeSmartObject(json);

            Assert.IsNull(SmartObject.DeviceId);
            Assert.IsNull(SmartObject.EventId);
            Assert.IsNull(SmartObject.ObjectType);
            Assert.IsNull(SmartObject.Owner);
            Assert.IsNull(SmartObject.RegistrationDate);
            Assert.AreEqual(SmartObject.Attributes.Count, 0);
        }

        [Test()]
        public void SmartObjectDeserializeTestWithUsernameNull()
        {
            string json =
                "{\"x_owner\" : { \"sometrash\":8888} }";

            SmartObject SmartObject = SmartObjectSerializer.DeserializeSmartObject(json);

            Assert.IsNull(SmartObject.DeviceId);
            Assert.IsNull(SmartObject.EventId);
            Assert.IsNull(SmartObject.ObjectType);
            Assert.IsNull(SmartObject.Owner);
            Assert.IsNull(SmartObject.RegistrationDate);
            Assert.AreEqual(SmartObject.Attributes.Count, 0);
        }

        [Test()]
        public void SmartObjectDeserializeTestCheckNull()
        {
            string json = "{}";

            SmartObject SmartObject = SmartObjectSerializer.DeserializeSmartObject(json);

            Assert.IsNull(SmartObject.DeviceId);
            Assert.IsNull(SmartObject.ObjectType);
            Assert.IsNull(SmartObject.RegistrationDate);
            Assert.IsNull(SmartObject.EventId);
            Assert.IsNull(SmartObject.Owner);
            Assert.IsNotNull(SmartObject.Attributes);
            Assert.AreEqual(SmartObject.Attributes.Count, 0);
        }

        [Test()]
        public void SmartObjectDeserializeTestWrongDeviceIdType()
        {
            string json = "{\"x_device_id\":9898.3}";

            Assert.That(() => SmartObjectSerializer.DeserializeSmartObject(json),
                Throws.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo("Field 'x_device_id' does not match TYPE 'TEXT'"));
        }

        [Test()]
        public void SmartObjectDeserializeTestWrongEventIdType()
        {
            string json = "{\"event_id\":\"54545c5454-054-54\",\"string\":\"stringValue\"}";

            Assert.That(() => SmartObjectSerializer.DeserializeSmartObject(json),
                Throws.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo("Field 'x_event_id' does not match TYPE 'GUID'"));
        }


        [Test()]
        public void SmartObjectDeserializeTestWrongObjectType()
        {
            string json = "{\"x_object_type\":false}";

            Assert.That(() => SmartObjectSerializer.DeserializeSmartObject(json),
                Throws.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo("Field 'x_object_type' does not match TYPE 'TEXT'"));
        }

        [Test()]
        public void SmartObjectDeserializeTestWrongRegistrationTimeType()
        {
            string json = "{\"x_registration_date\":\"5454544578f\"}";

            Assert.That(() => SmartObjectSerializer.DeserializeSmartObject(json),
                Throws.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo("Field 'x_registration_date' does not match TYPE 'DATETIME'"));
        }

        [Test()]
        public void SmartObjectDeserializeTestWrongOwnerType()
        {
            string json = "{\"x_owner\":\"5454544578f\"}";

            Assert.That(() => SmartObjectSerializer.DeserializeSmartObject(json),
                Throws.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo("Field 'x_owner' does not match TYPE 'OWNER'"));
        }
    }
}