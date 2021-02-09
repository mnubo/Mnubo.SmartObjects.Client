using System;
using NUnit.Framework;
using System.Collections.Generic;
using AspenTech.SmartObjects.Client.Models;

namespace AspenTech.SmartObjects.Client.ITTest
{
    [TestFixture()]
    public class EventsITTests
    {
        private readonly ISmartObjectsClient client;

        public EventsITTests()
        {
            client = ITTestHelper.newClient();
        }

        [Test()]
        public void TestSendEvents()
        {
            Guid uuid1 = Guid.NewGuid();
            String value1 = "value-" + uuid1.ToString();
            Event event1 = new Event.Builder() {
                EventId = uuid1,
                EventType = "event_type1",
                DeviceId = "device-" + uuid1.ToString(),
                Timeseries = new Dictionary<string, object>() {
                    {"ts_text_attribute", value1}
                }
            };

            Guid uuid2 = Guid.NewGuid();
            String value2 = "value-" + uuid2.ToString();
            Event event2 = new Event.Builder() {
                EventId = uuid2,
                EventType = "event_type1",
                DeviceId = "device-" + uuid2.ToString(),
                Timeseries = new Dictionary<string, object>() {
                    {"ts_text_attribute", value2}
                }
            };

            IEnumerable<EventResult> results = client.Events.Send(new List<Event>() { event1, event2 });
            foreach (EventResult result in results) {
                Assert.AreEqual(EventResult.ResultStates.success, result.Result);
        	}
        }
    }
}
