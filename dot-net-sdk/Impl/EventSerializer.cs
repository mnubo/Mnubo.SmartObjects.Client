using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Mnubo.SmartObjects.Client.Models;

namespace Mnubo.SmartObjects.Client.Impl
{
    public class EventSerializer
    {
        internal const string EventIdProperty = "event_id";
        internal const string ObjectProperty = "x_object";
        internal const string EventTypeProperty = "x_event_type";
        internal const string TimestampProperty = "x_timestamp";
        internal const string DatetimeFormat = "yyyy-MM-dd'T'HH:mm:ssZ";

        public static string SerializeEvents(List<Event> mnuboEvents)
        {
            List<string> stringBuilder = new List<string>();
            foreach (Event eve in mnuboEvents)
            {
                stringBuilder.Add(SerializeEvent(eve));
            }
            return "[" + string.Join(" , ", stringBuilder.ToArray()) + "]";
        }

        public static string SerializeEvent(Event mnuboEvent)
        {
            Dictionary<string, object> eventModelFlat = new Dictionary<string, object>();

            if( !string.IsNullOrEmpty(mnuboEvent.EventType) )
            {
                eventModelFlat.Add(EventTypeProperty, mnuboEvent.EventType);
            }

            if (mnuboEvent.Timestamp.HasValue)
            {
                eventModelFlat.Add(TimestampProperty, mnuboEvent.Timestamp);
            }

            if (mnuboEvent.EventId.HasValue)
            {
                eventModelFlat.Add(EventIdProperty, mnuboEvent.EventId);
            }

            if(mnuboEvent.SmartObject != null &&
                !string.IsNullOrEmpty(mnuboEvent.SmartObject.DeviceId))
            {
                eventModelFlat.Add(ObjectProperty, new Dictionary<string, string>()
                {
                    { SmartObjectSerializer.deviceIdProperty, mnuboEvent.SmartObject.DeviceId }
                });
            }

            foreach (KeyValuePair<string, object> attribute in mnuboEvent.Timeseries)
            {
                eventModelFlat.Add(attribute.Key, attribute.Value);
            }

            return JsonConvert.SerializeObject(
                eventModelFlat,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatString = DatetimeFormat,
                    
                });
        }

        public static Event DeserializeEvent(string obj)
        {
            Event.Builder builder = new Event.Builder();
            Dictionary<string, object> timeseries = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> token in JsonConvert.DeserializeObject<Dictionary<string, object>>(obj))
            {
                switch (token.Key)
                {
                    case TimestampProperty:
                        {
                            if (token.Value == null || !(token.Value is DateTime))
                            {
                                throw new InvalidOperationException("Field 'x_timestamp' does not match TYPE 'DATETIME'");
                            }
                            builder.Timestamp = ((DateTime)token.Value).ToUniversalTime();
                            break;
                        }
                    case EventTypeProperty:
                        {
                            if (!(token.Value is string))
                            {
                                throw new InvalidOperationException("Field 'x_event_type' does not match TYPE 'TEXT'");
                            }
                            builder.EventType = token.Value as string;
                            break;
                        }
                    case EventIdProperty:
                        {
                            Guid guid;
                            if (!(token.Value is string) ||
                                string.IsNullOrEmpty(token.Value as string) ||
                                !(Guid.TryParse(token.Value as string, out guid)))
                            {
                                throw new InvalidOperationException("Field 'x_event_id' does not match TYPE 'GUID'");
                            }
                            builder.EventId = guid;
                            break;
                        }
                    case ObjectProperty:
                        {
                            if(token.Value == null || !(token.Value is JObject))
                            {
                                throw new InvalidOperationException("Field 'x_object' does not match TYPE 'SMARTOBJECT'");
                            }

                            var deviceId = (token.Value as JObject)[SmartObjectSerializer.deviceIdProperty];
                            if(deviceId != null)
                            {
                                if (deviceId.Type != JTokenType.String)
                                {
                                    throw new InvalidOperationException("Field 'x_object' does not match TYPE 'SMARTOBJECT'");
                                }
                                builder.DeviceId = deviceId.Value<string>();
                            }
                            break;
                        }
                    default:
                        {
                            if (token.Value is JArray)
                            {
                                timeseries.Add(token.Key, (token.Value as JArray).ToObject<string[]>());
                            }
                            else
                            {
                                timeseries.Add(token.Key, token.Value);
                            }
                            break;
                        }
                }
            }
            builder.Timeseries = timeseries;
            return builder.Build();
        }
    }
}