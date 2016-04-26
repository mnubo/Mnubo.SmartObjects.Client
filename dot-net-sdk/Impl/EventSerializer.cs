using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Mnubo.SmartObjects.Client.Models;

namespace Mnubo.SmartObjects.Client.Impl
{
    /// <summary>
    /// Json Serilize or deserialize event instances
    /// </summary>
    public class EventSerializer
    {
        private const string EventIdProperty = "event_id";
        private const string ObjectProperty = "x_object";
        private const string EventTypeProperty = "x_event_type";
        private const string TimestampProperty = "x_timestamp";
        internal const string DatetimeFormat = "yyyy-MM-dd'T'HH:mm:ssZ";

        /// <summary>
        /// Serialize a list of events to a Json string
        /// </summary>
        /// <param name="mnuboEvents">List of events</param>
        /// <returns>json string</returns>
        public static string SerializeEvents(IEnumerable<Event> mnuboEvents)
        {
            List<string> stringBuilder = new List<string>();
            foreach (Event eve in mnuboEvents)
            {
                stringBuilder.Add(SerializeEvent(eve));
            }
            return "[" + string.Join(" , ", stringBuilder.ToArray()) + "]";
        }

        /// <summary>
        /// serialize an event instance to a JSON string
        /// </summary>
        /// <param name="mnuboEvent">event instance</param>
        /// <returns>json string</returns>
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

            if(!string.IsNullOrEmpty(mnuboEvent.DeviceId))
            {
                eventModelFlat.Add(ObjectProperty, new Dictionary<string, string>()
                {
                    { ObjectSerializer.DeviceIdProperty, mnuboEvent.DeviceId }
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

        /// <summary>
        /// deserialize a json string to an event instance
        /// </summary>
        /// <param name="obj">json string</param>
        /// <returns>event instance</returns>
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

                            var deviceId = (token.Value as JObject)[ObjectSerializer.DeviceIdProperty];
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