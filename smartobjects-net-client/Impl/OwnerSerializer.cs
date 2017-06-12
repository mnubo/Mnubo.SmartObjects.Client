using System;
using System.Collections.Generic;
using Mnubo.SmartObjects.Client.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mnubo.SmartObjects.Client.Impl
{
    /// <summary>
    /// Json Serilize or deserialize owner instances
    /// </summary>
    public class OwnerSerializer
    {
        private const string RegistrationDateProperty = "x_registration_date";
        private const string PasswordProperty = "x_password";
        internal const string UsernameProperty = "username";

        /// <summary>
        /// Serialize a list of owner to a Json string
        /// </summary>
        /// <param name="owners">List of owner</param>
        /// <returns>json string</returns>
        public static string SerializeOwners(IEnumerable<Owner> owners)
        {
            List<string> stringBuilder = new List<string>();
            foreach (Owner owner in owners)
            {
                stringBuilder.Add(SerializeOwner(owner));
            }
            return "[" + string.Join(" , ", stringBuilder.ToArray()) + "]";
        }

        /// <summary>
        /// serialize an owner instance to a JSON string
        /// </summary>
        /// <param name="owner">owner instance</param>
        /// <returns>json string</returns>
        public static string SerializeOwner(Owner owner)
        {
            Dictionary<string, object> ownerModelFlat = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(owner.Username))
            {
                ownerModelFlat.Add(UsernameProperty, owner.Username);
            }

            if (owner.RegistrationDateTime.HasValue)
            {
                ownerModelFlat.Add(RegistrationDateProperty, owner.RegistrationDateTime.Value.UtcDateTime);
            } else if (owner.RegistrationDate.HasValue) {
                ownerModelFlat.Add(RegistrationDateProperty, owner.RegistrationDate.Value);
            }

            if (!string.IsNullOrEmpty(owner.Password))
            {
                ownerModelFlat.Add(PasswordProperty, owner.Password);
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
                    DateFormatString = EventSerializer.DatetimeFormat
                });
        }

        /// <summary>
        /// deserialize a json string to an owner instance
        /// </summary>
        /// <param name="obj">json string</param>
        /// <returns>owner instance</returns>
        public static Owner DeserializeOwner(string obj)
        {
            Owner.Builder builder = new Owner.Builder();
            Dictionary<string, object> attributes = new Dictionary<string, object>();

            var rawOwner = JsonConvert.DeserializeObject<Dictionary<string, object>>(obj,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatString = EventSerializer.DatetimeFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });

            foreach (KeyValuePair<string, object> token in rawOwner)
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
                            builder.RegistrationDateTime = new DateTimeOffset((DateTime)token.Value);
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