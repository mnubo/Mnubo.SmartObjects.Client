using Mnubo.SmartObjects.Client.Models;
using System;
using System.Collections.Generic;

namespace Mnubo.SmartObjects.Client.Test.Impl
{
    public class TestUtils
    {
        private const string ResourceBase = "sdktest";
        private const string Password = "pppp";
        private const string ObjectTypeName = "wind_direction";

        internal const string EventType = "wind_direction";
        internal const string OwnerAttributeName = "attribute1";
        internal const string ObjectAttributeName = "color";
        internal const string TimeSeriesName = "wind_direction";

        private static readonly Random randomGenerator = new Random();

        public static string GetRandomResource()
        {
            return ResourceBase + randomGenerator.Next(1000000).ToString();
        }

        public static Owner CreateOwner()
        {
            return new Owner.Builder()
            {
                Username = GetRandomResource(),
                Password = Password,
                RegistrationDate = GetNowIgnoringMilis(),
                Attributes = new Dictionary<string, object>()
                    {
                        { OwnerAttributeName, "test" }
                    }
            };
        }

        public static Owner CreateFullOwner()
        {
            return new Owner.Builder()
            {
                Username = GetRandomResource(),
                Password = Password,
                Attributes = new Dictionary<string, object>()
                {
                    { OwnerAttributeName, "test" }
                },
                RegistrationDate = GetNowIgnoringMilis(),
                EventId = Guid.NewGuid()
            };
        }

        public static Owner CreateBasicOwner()
        {
            return new Owner.Builder()
            {
                Username = GetRandomResource(),
                Password = Password
            };
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

        public static List<Owner> CreateOwners(int numberOfOwners)
        {
            List<Owner> owners = new List<Owner>();

            for (int count = 0; count < numberOfOwners; count++)
            {
                owners.Add(CreateOwner());
            }
            return owners;
        }

        public static SmartObject CreateBasicObject()
        {
            return new SmartObject.Builder()
            {
                DeviceId = GetRandomResource(),
                ObjectType = ObjectTypeName
            };
        }

        public static SmartObject CreateFullObject(string username)
        {
            return new SmartObject.Builder()
            {
                DeviceId = GetRandomResource(),
                ObjectType = ObjectTypeName,
                Attributes = new Dictionary<string, object>()
                {
                    { ObjectAttributeName, "test" }
                },
                RegistrationDate = GetNowIgnoringMilis(),
                EventId = Guid.NewGuid(),
                Username = username
            };
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

        public static SmartObject CreateObjectWrongAttribute()
        {
            return new SmartObject.Builder()
            {
                DeviceId = GetRandomResource(),
                ObjectType = ObjectTypeName,
                Attributes = new Dictionary<string, object>()
                {
                    { "Unknow", "5" }
                }
            };
        }

        public static SmartObject CreateObject()
        {
            return new SmartObject.Builder()
            {
                DeviceId = GetRandomResource(),
                ObjectType = ObjectTypeName,
                Attributes = new Dictionary<string, object>()
                    {
                        { ObjectAttributeName, "test" }
                    },
                RegistrationDate = GetNowIgnoringMilis()
            };
        }

        public static List<SmartObject> CreateObjects(int numberOfObjects)
        {
            List<SmartObject> objs = new List<SmartObject>();

            for (int count = 0; count < numberOfObjects; count++)
            {
                objs.Add(CreateObject());
            }
            return objs;
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

        public static Event CreateEventWrongTimeserie(string deviceId)
        {
            return new Event.Builder()
            {
                EventType = EventType,
                DeviceId = deviceId,
                Timeseries = new Dictionary<string, object>()
                {
                    { "Unknown", GetRandomResource() }
                }
            };
        }

        public static DateTime GetNowIgnoringMilis()
        {
            DateTime now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc);
        }
    }
}
