using System;
using System.Collections.Generic;
using Mnubo.SmartObjects.Client.Models;
using NUnit.Framework;
using Mnubo.SmartObjects.Client.Test.Impl;

namespace Mnubo.SmartObjects.Client.Test.Model
{
    [TestFixture()]
    class SmartObjectTest
    {
        [Test()]
        public void SmartObjectBuilderTest()
        {
            DateTime now = TestUtils.GetNowIgnoringMilis();
            Guid objectId = Guid.NewGuid();
            Dictionary<string, object> attributes = new Dictionary<string, object>();
            attributes.Add("String", "text");

            SmartObject myObject = new SmartObject.Builder()
            {
                ObjectId = objectId,
                ObjectType = "objectType",
                RegistrationDate = now,
                Attributes = new Dictionary<string, object>()
                {
                    { "String", "text" }
                },
                DeviceId = "deviceId",
                Username = "username"
            };

            Assert.AreEqual(myObject.ObjectId, objectId);
            Assert.AreEqual(myObject.ObjectType, "objectType");
            Assert.AreEqual(myObject.RegistrationDate, now);
            CollectionAssert.AreEqual(myObject.Attributes, attributes);
            Assert.AreEqual(myObject.DeviceId, "deviceId");
            Assert.AreEqual(myObject.Username, "username");
        }

        [Test()]
        public void SmartObjectBuilderTestWithOwner()
        {
            DateTime now = TestUtils.GetNowIgnoringMilis();
            Guid objectId = Guid.NewGuid();
            Dictionary<string, object> attributes = new Dictionary<string, object>();
            attributes.Add("String", "text");

            Owner owner = new Owner.Builder()
            {
                Username = "username"
            };

            SmartObject myObject = new SmartObject.Builder()
            {
                ObjectId = objectId,
                ObjectType = "objectType",
                RegistrationDate = now,
                Attributes = new Dictionary<string, object>()
                {
                    { "String", "text" }
                },
                DeviceId = "deviceId",
                Username = "username"
            };

            Assert.AreEqual(myObject.ObjectId, objectId);
            Assert.AreEqual(myObject.ObjectType, "objectType");
            Assert.AreEqual(myObject.RegistrationDate, now);
            CollectionAssert.AreEqual(myObject.Attributes, attributes);
            Assert.AreEqual(myObject.DeviceId, "deviceId");
            Assert.AreEqual(myObject.Username, "username");
        }

        [Test()]
        public void SmartObjectBuilderAddAttributesTest()
        {
            Dictionary<string, object> attributes = new Dictionary<string, object>();
            attributes.Add("String", "text");
            attributes.Add("int", 10);
            attributes.Add("double", 10.7);
            attributes.Add("boolean", true);

            SmartObject myObject = new SmartObject.Builder()
            {
                Attributes = new Dictionary<string, object>()
                {
                    { "String", "text" },
                    { "int", 10 },
                    { "double", 10.7},
                    { "boolean", true }
                }
            };

            Assert.IsNull(myObject.DeviceId);
            Assert.IsNull(myObject.ObjectId);
            Assert.IsNull(myObject.Username);
            Assert.IsNull(myObject.RegistrationDate);
            Assert.IsNull(myObject.ObjectType);
            CollectionAssert.AreEqual(myObject.Attributes, attributes);
        }

        [Test()]
        public void SmartObjectBuilderOwnerEmpty()
        {
            SmartObject myObject = new SmartObject.Builder().Build();

            Assert.IsNull(myObject.DeviceId);
            Assert.IsNull(myObject.ObjectId);
            Assert.IsNull(myObject.Username);
            Assert.IsNull(myObject.RegistrationDate);
            Assert.IsNull(myObject.ObjectType);
            Assert.True(myObject.Attributes.Count == 0);
        }
    }
}