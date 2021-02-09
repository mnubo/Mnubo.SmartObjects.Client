using System.Collections.Generic;
using AspenTech.SmartObjects.Client.Models.DataModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AspenTech.SmartObjects.Client.Impl
{
    internal class ObjectTypeSerializer : Serializer<ObjectType>
    {
        private static JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        public string Serialize(IEnumerable<ObjectType> objectTypes)
        {
            List<string> stringBuilder = new List<string>();
            foreach (ObjectType ot in objectTypes)
            {
                stringBuilder.Add(Serialize(ot));
            }
            return "[" + string.Join(" , ", stringBuilder.ToArray()) + "]";
        }

        public string Serialize(ObjectType ot)
        {
            return JsonConvert.SerializeObject(
                new Dictionary<string, object> {
                    { "key", ot.Key },
                    { "description", ot.Description },
                    { "objectAttributesKeys", ot.ObjectAttributeKeys }
                },
                settings
            );
        }

        public List<ObjectType> Deserialize(string json)
        {
            JArray root = JArray.Parse(json);
            List<ObjectType> ots = new List<ObjectType>();
            foreach (JObject rawOt in root)
            {
                ots.Add(Deserialize(rawOt));
            }
            return ots;
        }

        public ObjectType Deserialize(JObject ot)
        {
            return new ObjectType(
               ot["key"].ToObject<string>(),
               ot["description"].ToObject<string>(),
               SerializerUtils.JArrayToList<string>((JArray)ot["objectAttributesKeys"])
            );
        }
    }
}