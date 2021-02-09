using System.Collections.Generic;
using AspenTech.SmartObjects.Client.Models.DataModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AspenTech.SmartObjects.Client.Impl
{
    internal class EventTypeSerializer : Serializer<EventType>
    {
        public string Serialize(IEnumerable<EventType> eventTypes)
        {
            List<string> stringBuilder = new List<string>();
            foreach (EventType et in eventTypes)
            {
                stringBuilder.Add(Serialize(et));
            }
            return "[" + string.Join(" , ", stringBuilder.ToArray()) + "]";
        }

        public string Serialize(EventType et)
        {
            return JsonConvert.SerializeObject(
                new Dictionary<string, object> {
                    { "key", et.Key },
                    { "description", et.Description },
                    { "origin", et.Origin },
                    { "timeseriesKeys", et.TimeseriesKeys }
                }
            );
        }

        public List<EventType> Deserialize(string json)
        {
            JArray root = JArray.Parse(json);
            List<EventType> ets = new List<EventType>();
            foreach (JObject rawEt in root)
            {
                ets.Add(Deserialize(rawEt));
            }
            return ets;
        }

        public EventType Deserialize(JObject et)
        {

            return new EventType(
               et["key"].ToObject<string>(),
               et["description"].ToObject<string>(),
               et["origin"].ToObject<string>(),
               SerializerUtils.JArrayToList<string>((JArray)et["timeseriesKeys"])
            );
        }
    }
}