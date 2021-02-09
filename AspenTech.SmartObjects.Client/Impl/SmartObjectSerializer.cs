﻿using System;
using System.Collections.Generic;
using Mnubo.SmartObjects.Client.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mnubo.SmartObjects.Client.Impl
{
    /// <summary>
    /// Json Serilize or deserialize object instances
    /// </summary>
    public class SmartObjectSerializer
    {
        internal const string DeviceIdProperty = "x_device_id";
        private const string ObjectTypeProperty = "x_object_type";
        private const string RegistrationDateProperty = "x_registration_date";
        private const string OwnerProperty = "x_owner";
        private const string EventIdProperty = "event_id";

        /// <summary>
        /// Serialize a list of smartobject to a Json string
        /// </summary>
        /// <param name="smartObjects">List of smartObjects</param>
        /// <returns>json string</returns>
        public static string SerializeSmartObjects(List<SmartObject> smartObjects)
        {
            List<string> stringBuilder = new List<string>();
            foreach (SmartObject smartObject in smartObjects)
            {
                stringBuilder.Add(SerializeSmartObject(smartObject));
            }
            return "[" + string.Join(" , ", stringBuilder.ToArray()) + "]";
        }

        /// <summary>
        /// serialize a smartobject instance to a JSON string
        /// </summary>
        /// <param name="smartObject">smartObject instance</param>
        /// <returns>json string</returns>
        public static string SerializeSmartObject(SmartObject smartObject)
        {
            Dictionary<string, object> objectModelFlat = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(smartObject.DeviceId))
            {
                objectModelFlat.Add(DeviceIdProperty, smartObject.DeviceId);
            }

            if (smartObject.RegistrationDate.HasValue)
            {
                objectModelFlat.Add(RegistrationDateProperty, smartObject.RegistrationDate);
            }

            if (!string.IsNullOrEmpty(smartObject.ObjectType))
            {
                objectModelFlat.Add(ObjectTypeProperty, smartObject.ObjectType);
            }

            if (smartObject.EventId.HasValue)
            {
                objectModelFlat.Add(EventIdProperty, smartObject.EventId);
            }

            if (smartObject.Owner != null &&
                !string.IsNullOrEmpty(smartObject.Owner.Username))
            {
                objectModelFlat.Add(OwnerProperty, new Dictionary<string, string>()
                {
                    { OwnerSerializer.UsernameProperty, smartObject.Owner.Username }
                });
            }

            foreach (KeyValuePair<string, object> attribute in smartObject.Attributes)
            {
                objectModelFlat.Add(attribute.Key, attribute.Value);
            }

            return JsonConvert.SerializeObject(
                objectModelFlat,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatString = EventSerializer.DatetimeFormat,

                });
        }

        /// <summary>
        /// deserialize a json string to a smartObject instance
        /// </summary>
        /// <param name="obj">json string</param>
        /// <returns>Smart Object instance</returns>
        public static SmartObject DeserializeSmartObject(string obj)
        {
            SmartObject.Builder builder = new SmartObject.Builder();
            Dictionary<string, object> attributes = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> token in JsonConvert.DeserializeObject<Dictionary<string, object>>(obj))
            {
                switch (token.Key)
                {
                    case DeviceIdProperty:
                        {
                            if (!(token.Value is string))
                            {
                                throw new InvalidOperationException("Field 'x_device_id' does not match TYPE 'TEXT'");
                            }
                            builder.DeviceId = token.Value as string;
                            break;
                        }
                    case RegistrationDateProperty:
                        {
                            if (token.Value == null || !(token.Value is DateTime))
                            {
                                throw new InvalidOperationException("Field 'x_registration_date' does not match TYPE 'DATETIME'");
                            }
                            builder.RegistrationDate = ((DateTime)token.Value).ToUniversalTime();
                            break;
                        }
                    case ObjectTypeProperty:
                        {
                            if (!(token.Value is string))
                            {
                                throw new InvalidOperationException("Field 'x_object_type' does not match TYPE 'TEXT'");
                            }
                            builder.ObjectType = token.Value as string;
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
                    case OwnerProperty:
                        {
                            if (token.Value == null || !(token.Value is JObject))
                            {
                                throw new InvalidOperationException("Field 'x_owner' does not match TYPE 'OWNER'");
                            }

                            var username = (token.Value as JObject)[OwnerSerializer.UsernameProperty];
                            if (username != null)
                            {
                                if (username.Type != JTokenType.String)
                                {
                                    throw new InvalidOperationException("Field 'x_owner' does not match TYPE 'OWNER'");
                                }
                                builder.Username = username.Value<string>();
                            }
                            break;
                        }
                    default:
                        {
                            if (token.Value is JArray)
                            {
                                attributes.Add(token.Key, (token.Value as JArray).ToObject<string[]>());
                            }
                            else
                            {
                                attributes.Add(token.Key, token.Value);
                            }
                            break;
                        }
                }
            }
            builder.Attributes = attributes;
            return builder.Build();
        }
    }
}