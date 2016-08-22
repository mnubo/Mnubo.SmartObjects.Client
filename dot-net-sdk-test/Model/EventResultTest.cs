using System;
using System.Collections.Generic;
using Mnubo.SmartObjects.Client.Models;
using NUnit.Framework;
using Mnubo.SmartObjects.Client.Test.Impl;
using Newtonsoft.Json;

namespace Mnubo.SmartObjects.Client.Test.Model
{
    [TestFixture()]
    public class EventResultTest
    {
        [Test()]
        public void DeserializeEventResultWithNoMessageTest()
        {
            var jsonString = @"{
                id: 'f8d6dd81-035a-459f-8998-41a387c1250f',
                objectExists: false,
                result: 'conflict' 
            }";

            var actual = JsonConvert.DeserializeObject<EventResult>(jsonString);

            Assert.AreEqual(new EventResult(
                Guid.Parse("f8d6dd81-035a-459f-8998-41a387c1250f"), 
                false, 
                EventResult.ResultStates.conflict, 
                null), actual);
        }

        [Test()]
        public void DeserializeEventResultWithMessageTest()
        {
            var jsonString = @"{
                id: 'f8d6dd81-035a-459f-8998-41a387c1250f',
                objectExists: false,
                result: 'error' ,
                message: 'event failed'
            }";

            var actual = JsonConvert.DeserializeObject<EventResult>(jsonString);

            Assert.AreEqual(new EventResult(
                Guid.Parse("f8d6dd81-035a-459f-8998-41a387c1250f"),
                false,
                EventResult.ResultStates.error,
                "event failed"), actual);
        }

        [Test()]
        public void SerializeEventResultTest()
        {
            var expJsonString = "{\"id\":\"f8d6dd81-035a-459f-8998-41a387c1250f\",\"objectExists\":false,\"result\":\"error\",\"message\":\"event failed\"}";

            var actual = JsonConvert.SerializeObject(
                new EventResult(
                    Guid.Parse("f8d6dd81-035a-459f-8998-41a387c1250f"),
                    false,
                    EventResult.ResultStates.error,
                    "event failed"));

            Assert.AreEqual(expJsonString, actual);
        }
    }
}