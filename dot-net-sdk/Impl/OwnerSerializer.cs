using System;
using System.Collections.Generic;
using Mnubo.SmartObjects.Client.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mnubo.SmartObjects.Client.Impl
{
    public class OwnerSerializer
    {
        internal const string RegistrationDateProperty = "x_registration_date";
        internal const string PasswordProperty = "x_password";
        internal const string UsernameProperty = "username";
        internal const string EventIdProperty = "event_id";

        public static string SerializeOwners(List<Owner> owners)
        {
            List<string> stringBuilder = new List<string>();
            foreach (Owner owner in owners)
            {
                stringBuilder.Add(SerializeOwner(owner));
            }
            return "[" + string.Join(" , ", stringBuilder.ToArray()) + "]";
        }

        public static string SerializeOwner(Owner owner)
        {
            Dictionary<string, object> ownerModelFlat = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(owner.Username))
            {
                ownerModelFlat.Add(UsernameProperty, owner.Username);
            }

            if (owner.RegistrationDate.HasValue)
            {
                ownerModelFlat.Add(RegistrationDateProperty, owner.RegistrationDate);
            }

            if (!string.IsNullOrEmpty(owner.Password))
            {
                ownerModelFlat.Add(PasswordProperty, owner.Password);
            }

            if (owner.EventId.HasValue)
            {
                ownerModelFlat.Add(EventIdProperty, owner.EventId);
            }

            foreach (KeyValuePair<string, object> attribute in owner.Attributes)
            {
                ownerModelFlat.Add(attribute.Key, attribute.Value);
            }

            return JsonConvert.SerializeObject(
                ownerModelFlat,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatString = EventSerializer.DatetimeFormat,

                });
        }

        public static Owner DeserializeOwner(string obj)
        {
            Owner.Builder builder = new Owner.Builder();
            Dictionary<string, object> attributes = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> token in JsonConvert.DeserializeObject<Dictionary<string, object>>(obj))
            {
                switch (token.Key)
                {
                    case UsernameProperty:
                        {
                            if (!(token.Value is string))
                            {
                                throw new InvalidOperationException("Field 'username' does not match TYPE 'TEXT'");
                            }
                            builder.Username = token.Value as string;
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
                    case PasswordProperty:
                        {
                            if (!(token.Value is string))
                            {
                                throw new InvalidOperationException("Field 'x_password' does not match TYPE 'TEXT'");
                            }
                            builder.Password = token.Value as string;
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