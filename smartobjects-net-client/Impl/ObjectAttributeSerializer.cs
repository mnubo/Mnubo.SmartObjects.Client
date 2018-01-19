using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Mnubo.SmartObjects.Client.Models.DataModel;

namespace Mnubo.SmartObjects.Client.Impl
{
    internal class ObjectAttributeSerializer : Serializer<ObjectAttribute>
    {
        private static JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        public string Serialize(IEnumerable<ObjectAttribute> objectAttributes)
        {
            List<string> stringBuilder = new List<string>();
            foreach (ObjectAttribute obj in objectAttributes)
            {
                stringBuilder.Add(Serialize(obj));
            }
            return "[" + string.Join(" , ", stringBuilder.ToArray()) + "]";
        }

        public string Serialize(ObjectAttribute obj)
        {
            return JsonConvert.SerializeObject(
                new Dictionary<string, object> {
                    { "key", obj.Key },
                    { "displayName", obj.DisplayName },
                    { "description", obj.Description },
                    { "type", new Dictionary<string, object> {
                        { "highLevelType", obj.HighLevelType },
                        { "containerType", obj.ContainerType }
                    } },
                    { "objectTypeKeys", obj.ObjectTypeKeys }
                }
            );
        }
        public List<ObjectAttribute> Deserialize(string json)
        {
            JArray root = JArray.Parse(json);
            List<ObjectAttribute> objs = new List<ObjectAttribute>();
            foreach (JObject rawObj in root)
            {
                objs.Add(Deserialize(rawObj));
            }
            return objs;
        }

        public ObjectAttribute Deserialize(JObject obj)
        {
            return new ObjectAttribute(
               obj["key"].ToObject<string>(),
               obj["displayName"].ToObject<string>(),
               obj["description"].ToObject<string>(),
               obj["type"]["highLevelType"].ToObject<string>(),
               obj["type"]["containerType"].ToObject<string>(),
               SerializerUtils.JArrayToList<string>((JArray)obj["objectTypeKeys"])
            );
        }
    }
}