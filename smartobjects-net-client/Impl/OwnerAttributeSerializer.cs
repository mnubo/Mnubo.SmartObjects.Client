using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Mnubo.SmartObjects.Client.Models.DataModel;

namespace Mnubo.SmartObjects.Client.Impl
{
    internal class OwnerAttributeSerializer : Serializer<OwnerAttribute>
    {
        private static JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        public string Serialize(IEnumerable<OwnerAttribute> ownerAttributes)
        {
            List<string> stringBuilder = new List<string>();
            foreach (OwnerAttribute owner in ownerAttributes)
            {
                stringBuilder.Add(Serialize(owner));
            }
            return "[" + string.Join(" , ", stringBuilder.ToArray()) + "]";
        }

        public string Serialize(OwnerAttribute owner)
        {
            return JsonConvert.SerializeObject(
                new Dictionary<string, object> {
                    { "key", owner.Key },
                    { "displayName", owner.DisplayName },
                    { "description", owner.Description },
                    { "type", new Dictionary<string, object> {
                        { "highLevelType", owner.HighLevelType },
                        { "containerType", owner.ContainerType }
                    } }
                },
                settings
            );
        }

        public List<OwnerAttribute> Deserialize(string json)
        {
            JArray root = JArray.Parse(json);
            List<OwnerAttribute> owners = new List<OwnerAttribute>();
            foreach (JObject rawOwner in root)
            {
                owners.Add(Deserialize(rawOwner));
            }
            return owners;
        }

        public OwnerAttribute Deserialize(JObject obj) {
            return new OwnerAttribute(
               obj["key"].ToObject<string>(),
               obj["displayName"].ToObject<string>(),
               obj["description"].ToObject<string>(),
               obj["type"]["highLevelType"].ToObject<string>(),
               obj["type"]["containerType"].ToObject<string>()
            );
        }
    }
}