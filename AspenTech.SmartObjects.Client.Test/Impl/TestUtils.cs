using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using AspenTech.SmartObjects.Client.Impl;
using AspenTech.SmartObjects.Client.Models;
using AspenTech.SmartObjects.Client.Models.Search;

namespace AspenTech.SmartObjects.Client.Test.Impl
{
    public class TestUtils
    {
        internal const string ResourceBase = "sdktest";
        internal const string Username = "username";
        internal const string Password = "pppp";
        internal const string ObjectTypeName = "wind_turbine";
        internal const string OwnerRegistrationDate = "2016-06-04T02:20:00.000Z";

        internal const string EventIdProperty = "event_id";
        internal const string DeviceId = "deviceid";
        internal const string EventType = "wind_direction_changed";
        internal const string OwnerAttributeName = "attribute1";
        internal const string ObjectRegistrationDate = "2016-05-03T14:20:00.000Z";
        internal const string ObjectAttributeName = "color";
        internal const string TimeSeriesName = "wind_direction";

        internal const string ErrorMessage = "request failed";

        private static readonly Random randomGenerator = new Random();

        public static string GetRandomResource()
        {
            return ResourceBase + randomGenerator.Next(1000000).ToString();
        }

        public static Owner CreateOwner(String username)
        {
            return new Owner.Builder()
            {
                Username = username,
                Password = Password,
                Attributes = new Dictionary<string, object>()
                {
                    { OwnerAttributeName, "test" }
                },
                RegistrationDate = DateTime.Parse(OwnerRegistrationDate)
            };
        }

        public static Owner CreateTestOwner()
        {
            return CreateOwner(TestUtils.Username);
        }

        public static List<Owner> CreateOwners(int numberOfOwners)
        {
            List<Owner> owners = new List<Owner>();

            for (int count = 0; count < numberOfOwners; count++)
            {
                owners.Add(CreateOwner(TestUtils.Username + count.ToString()));
            }
            return owners;
        }


        public static Owner CreateOwnerUpdateAttribute()
        {
            return new Owner.Builder()
            {
                Attributes = new Dictionary<string, object>()
                    {
                        { OwnerAttributeName, "modified" }
                    }
            };
        }

        public static Owner CreateOwnerWrongAttribute()
        {
            return new Owner.Builder()
            {
                Username = GetRandomResource(),
                Password = Password,
                Attributes = new Dictionary<string, object>()
                {
                    { "Unknow", "5" }
                }
            };
        }

        public static SmartObject CreateObject(string deviceId, string username)
        {
            return new SmartObject.Builder()
            {
                DeviceId = deviceId,
                ObjectType = ObjectTypeName,
                Attributes = new Dictionary<string, object>()
                {
                    { ObjectAttributeName, "test" }
                },
                RegistrationDate = DateTime.Parse(TestUtils.ObjectRegistrationDate),
                Username = username
            };
        }

        public static SmartObject CreateTestObject()
        {
            return CreateObject(TestUtils.DeviceId, TestUtils.Username);
        }

        public static SmartObject CreateObjectUpdateAttribute()
        {
            return new SmartObject.Builder()
            {
                Attributes = new Dictionary<string, object>()
                    {
                        { ObjectAttributeName, "modified" }
                    }
            };
        }

        public static List<SmartObject> CreateObjects(int numberOfObjects)
        {
            List<SmartObject> objects = new List<SmartObject>();

            for (int count = 0; count < numberOfObjects; count++)
            {
                objects.Add(CreateObject(TestUtils.DeviceId + count.ToString(), TestUtils.Username + count.ToString()));
            }
            return objects;
        }

        public static List<Event> CreateEvents(string deviceId, int numberOfEvents)
        {
            List<Event> events = new List<Event>();

            for(int count = 0; count < numberOfEvents; count++)
            {
                events.Add(CreateEvent(deviceId));
            }
            return events;
        }

        public static List<Event> CreateEventsAndSuccessfulResults(string deviceId, int numberOfEvents, out IEnumerable<EventResult> expectedResults )
        {
            List<Event> events = new List<Event>();
            List<EventResult> results = new List<EventResult>();

            for (int count = 0; count < numberOfEvents; count++)
            {
                Event createdEvent = CreateEvent(deviceId);
                events.Add(createdEvent);
                results.Add(new EventResult(createdEvent.EventId.Value, true, EventResult.ResultStates.success, null));
            }

            expectedResults = results;
            return events;
        }

        public static Event CreateFullEvent(string deviceId)
        {
            return new Event.Builder()
            {
                EventType = EventType,
                DeviceId = deviceId,
                Timeseries = new Dictionary<string, object>()
                {
                    { TimeSeriesName, GetRandomResource() }
                },
                Timestamp = TestUtils.GetNowIgnoringMilis(),
                EventId = Guid.NewGuid()
            };
        }

        public static Event CreateEvent(string deviceId)
        {
            return new Event.Builder()
            {
                EventType = EventType,
                DeviceId = deviceId,
                EventId = Guid.NewGuid(),
                Timeseries = new Dictionary<string, object>()
                {
                    { TimeSeriesName, GetRandomResource() }
                },
                Timestamp = TestUtils.GetNowIgnoringMilis()
            };
        }

        public static Event CreateEventWithEventId(string deviceId, Guid eventId)
        {
            return new Event.Builder()
            {
                EventType = EventType,
                DeviceId = deviceId,
                EventId = eventId,
                Timeseries = new Dictionary<string, object>()
                {
                    { TimeSeriesName, GetRandomResource() }
                },
                Timestamp = TestUtils.GetNowIgnoringMilis()
            };
        }

        public static Event CreateBasicEvent(string deviceId)
        {
            return new Event.Builder()
            {
                EventType = EventType,
                DeviceId = deviceId,
                Timeseries = new Dictionary<string, object>()
                {
                    { TimeSeriesName, GetRandomResource() }
                }
            };
        }

        public static IEnumerable<DataSet> CreateDatasets()
        {
            return new List<DataSet>()
            {
                new DataSet(
                    "a description A",
                    "a displayName A",
                    new HashSet<Field>()
                    {
                        new Field(Field.ContainerTypes.List, "field description A1", "display name A1", "DOUBLE", "fieldA1", false),
                        new Field(Field.ContainerTypes.None, "field description A2", "display name A2", "TEXT", "fieldA2", true),
                    },
                    "datasetA"),
                new DataSet(
                    "a description B",
                    "a displayName B",
                    new HashSet<Field>()
                    {
                        new Field(Field.ContainerTypes.None, "field description B1", "display name B1", "DOUBLE", "fieldB1", true),
                        new Field(Field.ContainerTypes.None, "field description B2", "display name B2", "TEXT", "fieldB2", false),
                    },
                    "datasetB")
            };
        }

        public static string CreateQuery()
        {
            return
            "{" +
                "\"from\":\"event\"," +
                "\"select\":" +
                    "[" +
                        "{\"value\":\"x_object.x_device_id\"}," +
                        "{\"value\":\"x_timestamp\"}," +
                        "{\"value\":\"x_event_type\"}," +
                        "{\"value\":\"" + TimeSeriesName + "\"}" +
                    "]," +
                "\"where\":" +
                    "{" +
                        "\"and\":" +
                            "[" +
                                "{\"x_object.x_device_id\":" +
                                        "{\"EQ\":\"" + DeviceId + "\"}}," +
                                "{\"x_event_type\":" +
                                        "{\"EQ\":\"" + EventType + "\"}}" +
                            "]" +
                    "}" +
            "}";
        }

        public static string CreateExpectedSearchResult()
        {
            return "{\"columns\":[{\"label\":\"x_object.x_device_id\",\"type\": \"text\"},{\"label\": \"x_timestamp\",\"type\": \"datetime\"},{\"label\":\"x_event_type\",\"type\":\"text\"},{\"label\":\"wind_direction\",\"type\":\"text\"}],\"rows\":[[\"device0\",\"2016-01-01T02:00:01.000Z\",\"wind_direction_changed\",\"N\"],[\"device0\",\"2016-01-01T02:12:01.000Z\",\"wind_direction_changed\",\"NE\"],[\"device0\",\"2016-01-01T04:00:01.000Z\",\"wind_direction_changed\",\"S\"]]}";
        }

        public static string CreateExpectedSerializedResultSet()
        {
            return "{\"columns\":[{\"label\":\"x_object.x_device_id\",\"type\":\"text\"},{\"label\":\"x_timestamp\",\"type\":\"datetime\"},{\"label\":\"x_event_type\",\"type\":\"text\"},{\"label\":\"wind_direction\",\"type\":\"text\"}],\"rows\":[{\"Values\":[\"device0\",\"2016-01-01T02:00:01Z\",\"wind_direction_changed\",\"N\"]},{\"Values\":[\"device0\",\"2016-01-01T02:12:01Z\",\"wind_direction_changed\",\"NE\"]},{\"Values\":[\"device0\",\"2016-01-01T04:00:01Z\",\"wind_direction_changed\",\"S\"]}]}";
        }


        public static DateTime GetNowIgnoringMilis()
        {
            DateTime now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc);
        }

        public static void AssertOwnerEquals(Owner owner1, Owner owner2)
        {
            Assert.AreEqual(owner1.Username, owner2.Username);
            Assert.AreEqual(owner1.Password, owner2.Password);
            Assert.AreEqual(owner1.RegistrationDate, owner2.RegistrationDate);
            Assert.AreEqual(owner1.Attributes.Count, owner2.Attributes.Count);

            foreach( var kvp in owner1.Attributes)
            {
                Object value2;
                Assert.IsTrue(owner2.Attributes.TryGetValue(kvp.Key, out value2));
                Assert.AreEqual(kvp.Value, value2);
            }
        }

        public static void AssertOwnersEqual(IEnumerable<Owner> owners1, IEnumerable<Owner> owners2)
        {
            Assert.AreEqual(owners1.Count() , owners2.Count());
            for(int i=0; i<owners1.Count();i++)
            {
                AssertOwnerEquals(owners1.ElementAt<Owner>(i), owners2.ElementAt<Owner>(i));
            }
        }

        public static void AssertObjectEquals(SmartObject object1, SmartObject object2)
        {
            Assert.AreEqual(object1.DeviceId, object2.DeviceId);
            Assert.AreEqual(object1.Username, object2.Username);
            Assert.AreEqual(object1.ObjectType, object2.ObjectType);
            Assert.AreEqual(object1.RegistrationDate, object2.RegistrationDate);
            Assert.AreEqual(object1.Attributes.Count, object2.Attributes.Count);

            foreach (var kvp in object1.Attributes)
            {
                Object value2;
                Assert.IsTrue(object2.Attributes.TryGetValue(kvp.Key, out value2));
                Assert.AreEqual(kvp.Value, value2);
            }
        }

        public static void AssertObjectsEqual(IEnumerable<SmartObject> objects1, IEnumerable<SmartObject> objects2)
        {
            Assert.AreEqual(objects1.Count(), objects2.Count());
            for (int i = 0; i < objects1.Count(); i++)
            {
                AssertObjectEquals(objects1.ElementAt<SmartObject>(i), objects2.ElementAt<SmartObject>(i));
            }
        }

        public static void AssertDataSetFieldEquals(Field field1, Field field2)
        {
            Assert.AreEqual(field1.Key, field2.Key);
            Assert.AreEqual(field1.Description, field2.Description);
            Assert.AreEqual(field1.DisplayName, field2.DisplayName);
            Assert.AreEqual(field1.HighLevelType, field2.HighLevelType);
            Assert.AreEqual(field1.IsPrimaryKey, field2.IsPrimaryKey);
        }

        public static void AssertDataSetEquals(DataSet ds1, DataSet ds2)
        {
            Assert.AreEqual(ds1.Key, ds2.Key);
            Assert.AreEqual(ds1.Description, ds2.Description);
            Assert.AreEqual(ds1.DisplayName, ds2.DisplayName);
            Assert.AreEqual(ds1.Fields.Count(), ds2.Fields.Count());

            for (int i = 0; i < ds1.Fields.Count(); i++)
            {
                AssertDataSetFieldEquals(ds1.Fields.ElementAt<Field>(i), ds2.Fields.ElementAt<Field>(i));
            }
        }


		public static void AssertJsonEquals(string json, List<string> items)
		{
			foreach(var item in items)
			{
				int first = json.IndexOf(item);
				int last = json.LastIndexOf(item);
				Assert.GreaterOrEqual(first, 0);
				Assert.AreEqual(first, last);
			}

			Assert.AreEqual(json.Count((c) => c == ','), items.Count - 1);
		}

		public  static List<Owner> DeserializeOwners(string json)
        {
            List<Owner> owners = new List<Owner>();
            foreach (var jsonObject in JArray.Parse(json))
            {
                owners.Add(OwnerSerializer.DeserializeOwner(jsonObject.ToString(Formatting.None)));
            }
            return owners;
        }

        public static List<SmartObject> DeserializeObjects(string json)
        {
            List<SmartObject> objects = new List<SmartObject>();
            foreach (var jsonObject in JArray.Parse(json))
            {
                objects.Add(ObjectSerializer.DeserializeObject(jsonObject.ToString(Formatting.None)));
            }
            return objects;
        }

        public static List<EventResult> EventsToSuccesfullResults(string json)
        {
            List<EventResult> results = new List<EventResult>();
            foreach (var jsonObject in JArray.Parse(json))
            {

                JObject jsonObj = (JObject)jsonObject;
                Guid eventId;
                JToken eventIdProperty;

                if (jsonObj.TryGetValue(EventIdProperty, out eventIdProperty))
                {
                    eventId = Guid.Parse((string)eventIdProperty);
                }
                else
                {
                    eventId = Guid.NewGuid();
                }

                results.Add(new EventResult(eventId, true, EventResult.ResultStates.success, null));
            }
            return results;
        }

        //Took from http://stackoverflow.com/questions/138043/find-the-next-tcp-port-in-net
        public static int FreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

    }

    public class EventResultWithIgnoredIdComparer : IComparer
    {

        internal static bool EqualsIgnoreEventId(EventResult expected, EventResult actual)
        {
            return (expected.ObjectExists == actual.ObjectExists) && (expected.Result == actual.Result) && (expected.Message == actual.Message);
        }

        int IComparer.Compare(object x, object y)
        {
            EventResult first = (EventResult)x;
            EventResult second = (EventResult)y;

            if (EqualsIgnoreEventId(first, second))
            {
                return 0;
            }
            else
            {
                return -1;
            }

        }
    }
}
