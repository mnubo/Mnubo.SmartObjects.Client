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
            DateTimeOffset now = new DateTimeOffset(2017, 06, 12, 18, 05, 05, TimeSpan.FromHours(5d));

            Dictionary<string, object> attributes = new Dictionary<string, object>();
            attributes.Add("string", "stringValue");
            attributes.Add("double", 10d);
            attributes.Add("float", 10.5f);

            SmartObject myObject = new SmartObject.Builder()
            {
                DeviceId = "test",
                ObjectType = "type",
                RegistrationDateTime = now,
                Attributes = new SortedDictionary<string, object>()
                {
                    { "string", "stringValue" },
                    { "double", 10d },
                    { "float", 10.5f }
                }
            };

            string json = ObjectSerializer.SerializeObject(myObject);
			TestUtils.AssertJsonEquals(json, new List<string>
			{
				"\"x_device_id\":\"test\"",
				"\"x_registration_date\":\"2017-06-12T13:05:05Z\"",
				"\"x_object_type\":\"type\"",
				"\"float\":10.5",
				"\"double\":10.0",
				"\"string\":\"stringValue\""
			});
        }

        [Test()]
        public void SmartObjectSerializeTestWithDeprecatedDate()
        {
            DateTimeOffset now = new DateTimeOffset(2017, 06, 12, 18, 05, 05, TimeSpan.FromHours(5d));

            Dictionary<string, object> attributes = new Dictionary<string, object>();
            attributes.Add("string", "stringValue");
            attributes.Add("double", 10d);
            attributes.Add("float", 10.5f);

            SmartObject myObject = new SmartObject.Builder()
            {
                DeviceId = "test",
                ObjectType = "type",
                RegistrationDate = now.UtcDateTime,
                Attributes = new SortedDictionary<string, object>()
                {
                    { "string", "stringValue" },
                    { "double", 10d },
                    { "float", 10.5f }
                }
            };

            string json = ObjectSerializer.SerializeObject(myObject);
			TestUtils.AssertJsonEquals(json, new List<string>
			{
				"\"x_device_id\":\"test\"",
				"\"x_registration_date\":\"2017-06-12T13:05:05Z\"",
				"\"x_object_type\":\"type\"",
				"\"float\":10.5",
				"\"double\":10.0",
				"\"string\":\"stringValue\""
			});
        }

        [Test()]
        public void SmartObjectSerializeTestWithSmartObject()
        {
            SmartObject myObject = new SmartObject.Builder()
            {
                DeviceId = "test",
                Username = "owner"
            };

            string json = ObjectSerializer.SerializeObject(myObject);
            Assert.AreEqual(
                "{\"x_device_id\":\"test\",\"x_owner\":{\"username\":\"owner\"}}",
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

            string json = ObjectSerializer.SerializeObject(myObject);
            Assert.AreEqual("{\"list\":[\"1\",\"2\"]}", json);
        }

        [Test()]
        public void SmartObjectSerializeTestWithEmptyObject()
        {

            SmartObject myObject = new SmartObject.Builder().Build();

            string json = ObjectSerializer.SerializeObject(myObject);
            Assert.AreEqual("{}", json);
        }

        [Test()]
        public void SmartObjectDeserializeTest()
        {
            DateTime now = TestUtils.GetNowIgnoringMilis();

            string json =
                "{\"x_device_id\":\"test\"," +
                "\"x_object_type\":\"type\"," +
                "\"x_registration_date\":\"" + now.ToString(EventSerializerTest.DatetimeFormat) + "\"," +
                "\"age\": 89," +
                "\"weight\": 125.5," +
                "\"married\": true," +
                "\"counter\": -13582," +
                "\"list_SmartObject\": [\"val1\",\"val2\",\"val3\"]," +
                "\"x_owner\" : { \"username\":\"owner\",\"sometrash\":8888}}";

            var attributes = ImmutableDictionary.CreateBuilder<string, object>();
            attributes.Add("age", 89);
            attributes.Add("weight", 125.5);
            attributes.Add("married", true);
            attributes.Add("counter", -13582);
            attributes.Add("list_SmartObject", new string[] { "val1", "val2", "val3" });

            SmartObject SmartObject = ObjectSerializer.DeserializeObject(json);

            Assert.AreEqual(SmartObject.DeviceId, "test");
            Assert.AreEqual(SmartObject.ObjectType, "type");
            Assert.AreEqual(SmartObject.Username, "owner");
            Assert.AreEqual(SmartObject.RegistrationDate.Value.ToString(EventSerializerTest.DatetimeFormat),
                now.ToString(EventSerializerTest.DatetimeFormat));
            Assert.AreEqual(SmartObject.RegistrationDateTime.Value.ToString(EventSerializerTest.DatetimeFormat),
                now.ToString(EventSerializerTest.DatetimeFormat));
            CollectionAssert.AreEqual(SmartObject.Attributes, attributes.ToImmutable());
        }

        [Test()]
        public void SmartObjectDeserializeTestWithOwnerNull()
        {
            string json =
                "{\"x_owner\" : { } }";

            SmartObject SmartObject = ObjectSerializer.DeserializeObject(json);

            Assert.IsNull(SmartObject.DeviceId);
            Assert.IsNull(SmartObject.ObjectType);
            Assert.IsNull(SmartObject.Username);
            Assert.IsNull(SmartObject.RegistrationDate);
            Assert.AreEqual(SmartObject.Attributes.Count, 0);
        }

        [Test()]
        public void SmartObjectDeserializeTestWithUsernameNull()
        {
            string json =
                "{\"x_owner\" : { \"sometrash\":8888} }";

            SmartObject SmartObject = ObjectSerializer.DeserializeObject(json);

            Assert.IsNull(SmartObject.DeviceId);
            Assert.IsNull(SmartObject.ObjectType);
            Assert.IsNull(SmartObject.Username);
            Assert.IsNull(SmartObject.RegistrationDate);
            Assert.AreEqual(SmartObject.Attributes.Count, 0);
        }

        [Test()]
        public void SmartObjectDeserializeTestCheckNull()
        {
            string json = "{}";

            SmartObject SmartObject = ObjectSerializer.DeserializeObject(json);

            Assert.IsNull(SmartObject.DeviceId);
            Assert.IsNull(SmartObject.ObjectType);
            Assert.IsNull(SmartObject.RegistrationDate);
            Assert.IsNull(SmartObject.Username);
            Assert.IsNotNull(SmartObject.Attributes);
            Assert.AreEqual(SmartObject.Attributes.Count, 0);
        }

        [Test()]
        public void SmartObjectDeserializeTestWrongDeviceIdType()
        {
            string json = "{\"x_device_id\":9898.3}";

            Assert.That(() => ObjectSerializer.DeserializeObject(json),
                Throws.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo("Field 'x_device_id' does not match TYPE 'TEXT'"));
        }

        [Test()]
        public void SmartObjectDeserializeTestWrongObjectType()
        {
            string json = "{\"x_object_type\":false}";

            Assert.That(() => ObjectSerializer.DeserializeObject(json),
                Throws.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo("Field 'x_object_type' does not match TYPE 'TEXT'"));
        }

        [Test()]
        public void SmartObjectDeserializeTestWrongRegistrationTimeType()
        {
            string json = "{\"x_registration_date\":\"5454544578f\"}";

            Assert.That(() => ObjectSerializer.DeserializeObject(json),
                Throws.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo("Field 'x_registration_date' does not match TYPE 'DATETIME'"));
        }

        [Test()]
        public void SmartObjectDeserializeTestWrongOwnerType()
        {
            string json = "{\"x_owner\":\"5454544578f\"}";

            Assert.That(() => ObjectSerializer.DeserializeObject(json),
                Throws.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo("Field 'x_owner' does not match TYPE 'OWNER'"));
        }
    }
}