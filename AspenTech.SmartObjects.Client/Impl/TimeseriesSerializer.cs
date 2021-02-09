using System.Collections.Generic;
using AspenTech.SmartObjects.Client.Models.DataModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AspenTech.SmartObjects.Client.Impl
{
    internal class TimeseriesSerializer : Serializer<Timeseries>
    {
        private static JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        public string Serialize(IEnumerable<Timeseries> timeseries)
        {
            List<string> stringBuilder = new List<string>();
            foreach (Timeseries ts in timeseries)
            {
                stringBuilder.Add(Serialize(ts));
            }
            return "[" + string.Join(" , ", stringBuilder.ToArray()) + "]";
        }

        public string Serialize(Timeseries ts)
        {
            return JsonConvert.SerializeObject(
                new Dictionary<string, object> {
                    { "key", ts.Key },
                    { "displayName", ts.DisplayName },
                    { "description", ts.Description },
                    { "type", new Dictionary<string, object> {
                        { "highLevelType", ts.HighLevelType }
                    } },
                    { "eventTypeKeys", ts.EventTypeKeys }
                }
            );
        }

        public List<Timeseries> Deserialize(string json)
        {
            JArray root = JArray.Parse(json);
            List<Timeseries> tss = new List<Timeseries>();
            foreach (JObject rawOwner in root)
            {
                tss.Add(Deserialize(rawOwner));
            }

            return tss;
        }
        public Timeseries Deserialize(JObject rawTs) {
            return new Timeseries(
               rawTs["key"].ToObject<string>(),
               rawTs["displayName"].ToObject<string>(),
               rawTs["description"].ToObject<string>(),
               rawTs["type"]["highLevelType"].ToObject<string>(),
               SerializerUtils.JArrayToList<string>((JArray)rawTs["eventTypeKeys"])
            );
        }
    }
}