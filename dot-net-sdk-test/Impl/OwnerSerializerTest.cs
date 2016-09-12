using System;
using System.Collections.Generic;
using NUnit.Framework;
using Mnubo.SmartObjects.Client.Models;
using Mnubo.SmartObjects.Client.Impl;
using System.Collections.Immutable;

namespace Mnubo.SmartObjects.Client.Test.Impl
{
    [TestFixture()]
    public class OwnerSerializerTest
    {
        [Test()]
        public void OwnerSerializeTest()
        {
            DateTime now = TestUtils.GetNowIgnoringMilis();

            Dictionary<string, object> attributes = new Dictionary<string, object>();
            attributes.Add("string", "stringValue");
            attributes.Add("double", 10d);
            attributes.Add("float", 10.5f);
            attributes.Add("boolean", false);

            Owner owner = new Owner.Builder()
            {
                Username = "username",
                Password = "password",
                RegistrationDate = now,
                Attributes = attributes,
            };

            string json = OwnerSerializer.SerializeOwner(owner);
			TestUtils.AssertJsonEquals(json, new List<string>
			{
				"\"username\":\"username\"",
				$"\"x_registration_date\":\"{now.ToString(EventSerializerTest.DatetimeFormat)}\"",
				"\"x_password\":\"password\"",
				"\"float\":10.5",
				"\"boolean\":false",
				"\"double\":10.0",
				"\"string\":\"stringValue\""
			});
        }

        [Test()]
        public void OwnerSerializeTestAttributeList()
        {
            Dictionary<string, object> attributes = new Dictionary<string, object>();
            attributes.Add("list", new List<string>() { "1", "2" });

            Owner owner = new Owner.Builder()
            {
                Attributes = attributes
            };

            string json = OwnerSerializer.SerializeOwner(owner);
            Assert.AreEqual("{\"list\":[\"1\",\"2\"]}", json);
        }

        [Test()]
        public void OwnerSerializeTestWithEmptyOwner()
        {
            Owner owner = new Owner.Builder().Build();

            string json = OwnerSerializer.SerializeOwner(owner);
            Assert.AreEqual("{}", json);
        }

        [Test()]
        public void OwnerDeserializeTest()
        {
            DateTime now = TestUtils.GetNowIgnoringMilis();

            string json =
                "{\"username\":\"test\"," +
                "\"x_password\":\"password\"," +
                "\"x_registration_date\":\"" + now.ToString(EventSerializerTest.DatetimeFormat) + "\"," +
                "\"age\": 89," +
                "\"weight\": 125.5," +
                "\"married\": true," +
                "\"counter\": -13582," +
                "\"list_owner\": [\"val1\",\"val2\",\"val3\"]}";

            var attributes = ImmutableDictionary.CreateBuilder<string, object>();
            attributes.Add("age", 89);
            attributes.Add("weight", 125.5);
            attributes.Add("married", true);
            attributes.Add("counter", -13582);
            attributes.Add("list_owner", new string[] { "val1", "val2", "val3" });

            Owner owner = OwnerSerializer.DeserializeOwner(json);

            Assert.AreEqual(owner.Username, "test");
            Assert.AreEqual(owner.Password, "password");
            Assert.AreEqual(owner.RegistrationDate.Value.ToString(EventSerializerTest.DatetimeFormat),
                now.ToString(EventSerializerTest.DatetimeFormat));
            CollectionAssert.AreEqual(owner.Attributes, attributes.ToImmutable());
        }

        [Test()]
        public void OwnerDeserializeTestCheckNull()
        {
            string json = "{}";

            Owner owner = OwnerSerializer.DeserializeOwner(json);

            Assert.IsNull(owner.Password);
            Assert.IsNull(owner.Username);
            Assert.IsNull(owner.RegistrationDate);
            Assert.IsNotNull(owner.Attributes);
            Assert.AreEqual(owner.Attributes.Count, 0);
        }

        [Test()]
        public void OwnerDeserializeTestWrongPasswordType()
        {
            string json = "{\"x_password\":9898.3}";

            Assert.That(() => OwnerSerializer.DeserializeOwner(json),
                Throws.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo("Field 'x_password' does not match TYPE 'TEXT'"));
        }

        [Test()]
        public void OwnerDeserializeTestWrongUsernameType()
        {
            string json = "{\"username\":false}";

            Assert.That(() => OwnerSerializer.DeserializeOwner(json),
                Throws.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo("Field 'username' does not match TYPE 'TEXT'"));
        }

        [Test()]
        public void OwnerDeserializeTestWrongRegistrationTimeType()
        {
            string json = "{\"x_registration_date\":\"6565g\"}";

            Assert.That(() => OwnerSerializer.DeserializeOwner(json),
               Throws.TypeOf<InvalidOperationException>()
               .With.Message.EqualTo("Field 'x_registration_date' does not match TYPE 'DATETIME'"));
        }
    }
}