using System;
using System.Collections.Generic;
using Mnubo.SmartObjects.Client.Models;
using NUnit.Framework;
using Mnubo.SmartObjects.Client.Test.Impl;

namespace Mnubo.SmartObjects.Client.Test.Model
{
    [TestFixture()]
    public class OwnerTest
    {
        private Dictionary<string, object> attributes = new Dictionary<string, object>() { { "String", "text" } };

        [Test()]
        public void OwnerBuilderTest()
        {
            DateTime now = TestUtils.GetNowIgnoringMilis();
            Guid eventId = Guid.NewGuid();

            Owner owner = new Owner.Builder()
            {
                Username = "username",
                Password = "password",
                RegistrationDate = now,
                Attributes = new Dictionary<string, object>()
                {
                    {"String", "text" }
                },
                EventId = eventId
            };

            Assert.AreEqual(owner.EventId, eventId);
            Assert.AreEqual(owner.Password, "password");
            Assert.AreEqual(owner.Username, "username");
            Assert.AreEqual(owner.RegistrationDate, now);
            CollectionAssert.AreEqual(owner.Attributes, new Dictionary<string, object>() { { "String", "text" } });
        }

        [Test()]
        public void OwnerBuilderAddAttributesTest()
        {
            Dictionary<string, object> attributes = new Dictionary<string, object>();
            attributes.Add("String", "text");
            attributes.Add("int", 10);
            attributes.Add("double", 10.7);
            attributes.Add("boolean", true);

            Owner owner = new Owner.Builder()
            {
                Attributes = new Dictionary<string, object>()
                {
                    { "String", "text" },
                    { "int", 10 },
                    { "double", 10.7 },
                    { "boolean", true }
                }
            };

            Assert.IsNull(owner.EventId);
            Assert.IsNull(owner.Password);
            Assert.IsNull(owner.Username);
            Assert.IsNull(owner.RegistrationDate);
            CollectionAssert.AreEqual(owner.Attributes, attributes);
        }

        [Test()]
        public void OwnerBuilderEmpty()
        {
            Owner owner = new Owner.Builder().Build();

            Assert.IsNull(owner.Username);
            Assert.IsNull(owner.EventId);
            Assert.IsNull(owner.Password);
            Assert.IsNull(owner.RegistrationDate);
            Assert.True(owner.Attributes.Count == 0);
        }
    }
}