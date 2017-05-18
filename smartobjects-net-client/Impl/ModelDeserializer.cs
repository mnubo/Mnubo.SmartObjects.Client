 using System;
using System.Collections.Generic;
using Mnubo.SmartObjects.Client.Models.DataModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mnubo.SmartObjects.Client.Impl
{
    using TimeseriesFields = Tuple<string, string, string, string>;
    using ObjectAttributeFields = Tuple<string, string, string, string, string>;

    /// <summary>
    /// Deserializer to read JSON as a Model
    /// </summary>
    public class ModelDeserializer
    {
        /// <summary>
        /// Deserialize a json string to a Model instance
        /// </summary>
        /// <param name="payload">json string</param>
        /// <returns>Model instance</returns>
        public static Model DeserializeModel(string payload)
        {
            JObject root = JObject.Parse(payload);
            List<ObjectType> objectTypes = new List<ObjectType>();
            Dictionary<ObjectAttributeFields, List<string>> allObjectAttributes = new Dictionary<ObjectAttributeFields, List<string>>();
            foreach (JObject rawOt in root["objectTypes"]) {
                string key = rawOt["key"].ToObject<string>();
                string description = rawOt["description"].ToObject<string>();
                List<string> objectAttributeKeys = new List<string>();

                foreach (JObject rawObjectAttributes in rawOt["objectAttributes"]) {
                    string objKey = rawObjectAttributes["key"].ToObject<string>();
                    string objDescription = rawObjectAttributes["description"].ToObject<string>();
                    string objDisplayName = rawObjectAttributes["displayName"].ToObject<string>();
                    string objHighLevelType = rawObjectAttributes["type"]["highLevelType"].ToObject<string>();
                    string objContainerType = rawObjectAttributes["type"]["containerType"].ToObject<string>();

                    objectAttributeKeys.Add(objKey);
                    ObjectAttributeFields objFields = Tuple.Create(objKey, objDisplayName, objDescription, objHighLevelType, objContainerType);
                    if (allObjectAttributes.ContainsKey(objFields)) {
                        allObjectAttributes[objFields].Add(key);
                    } else {
                        allObjectAttributes.Add(objFields, new List<string>(){key});
                    }
                }
                objectTypes.Add(new ObjectType(key, description, objectAttributeKeys));
            }
            
            List<EventType> eventTypes = new List<EventType>();
            Dictionary<TimeseriesFields, List<string>> allTimeseries = new Dictionary<TimeseriesFields, List<string>>();
            foreach (JObject rawEt in root["eventTypes"]) {
                string key = rawEt["key"].ToObject<string>();
                string description = rawEt["description"].ToObject<string>();
                string origin = rawEt["origin"].ToObject<string>();
                List<string> timeseriesKeys = new List<string>();

                foreach (JObject rawTimeseries in rawEt["timeseries"]) {
                    string tsKey = rawTimeseries["key"].ToObject<string>();
                    string tsDescription = rawTimeseries["description"].ToObject<string>();
                    string tsDisplayName = rawTimeseries["displayName"].ToObject<string>();
                    string tsHighLevelType = rawTimeseries["type"]["highLevelType"].ToObject<string>();

                    timeseriesKeys.Add(tsKey);
                    TimeseriesFields tsFields = Tuple.Create(tsKey, tsDisplayName, tsDescription, tsHighLevelType);
                    if (allTimeseries.ContainsKey(tsFields)) {
                        allTimeseries[tsFields].Add(key);
                    } else {
                        allTimeseries.Add(tsFields, new List<string>(){key});
                    }
                }
                eventTypes.Add(new EventType(key, description, origin, timeseriesKeys));
            }
            List<OwnerAttribute> ownerAttributes = new List<OwnerAttribute>();
            foreach (JObject rawOwner in root["ownerAttributes"]) {
                string key = rawOwner["key"].ToObject<string>();
                string description = rawOwner["description"].ToObject<string>();
                string displayName = rawOwner["displayName"].ToObject<string>();
                string highLevelType = rawOwner["type"]["highLevelType"].ToObject<string>();
                string containerType = rawOwner["type"]["containerType"].ToObject<string>();

                OwnerAttribute owner = new OwnerAttribute(key, displayName, description, highLevelType, containerType);
                ownerAttributes.Add(owner);
            }

            List<Sessionizer> sessionizers = new List<Sessionizer>();
            foreach (JObject rawSessionizer in root["sessionizers"]) {
                string key = rawSessionizer["key"].ToObject<string>();
                string description = rawSessionizer["description"].ToObject<string>();
                string displayName = rawSessionizer["displayName"].ToObject<string>();
                string startEventTypeKey = rawSessionizer["startEventTypeKey"].ToObject<string>();
                string endEventTypeKey = rawSessionizer["endEventTypeKey"].ToObject<string>();

                Sessionizer sess = new Sessionizer(key, displayName, description, startEventTypeKey, endEventTypeKey);
                sessionizers.Add(sess);
            }

            JToken rawOrphan = root["orphans"];

            List<ObjectAttribute> orphanObjects = new List<ObjectAttribute>();
            JToken orphanObjectsNode = rawOrphan["objectAttributes"];
            if (orphanObjectsNode != null) {
                foreach (JObject rawObjectAttributes in orphanObjectsNode) {
                    string key = rawObjectAttributes["key"].ToObject<string>();
                    string description = rawObjectAttributes["description"].ToObject<string>();
                    string displayName = rawObjectAttributes["displayName"].ToObject<string>();
                    string highLevelType = rawObjectAttributes["type"]["highLevelType"].ToObject<string>();
                    string containerType = rawObjectAttributes["type"]["containerType"].ToObject<string>();

                    ObjectAttribute obj = new ObjectAttribute(key, displayName, description, highLevelType, containerType, new List<string>(){});
                    orphanObjects.Add(obj);
                }
            }

            List<Timeseries> orphanTimeseries = new List<Timeseries>();
            JToken orphanTsNode = rawOrphan["timeseries"];
            if(orphanTsNode != null) {
                foreach (JObject rawTimeseries in orphanTsNode) {
                    string key = rawTimeseries["key"].ToObject<string>();
                    string description = rawTimeseries["description"].ToObject<string>();
                    string displayName = rawTimeseries["displayName"].ToObject<string>();
                    string highLevelType = rawTimeseries["type"]["highLevelType"].ToObject<string>();

                    Timeseries ts = new Timeseries(key, displayName, description, highLevelType, new List<string>(){});
                    orphanTimeseries.Add(ts);
                }    
            }
            
            List<ObjectAttribute> objectAttributes = new List<ObjectAttribute>();
            foreach (KeyValuePair<ObjectAttributeFields, List<string>> entry in allObjectAttributes) {
                ObjectAttributeFields objFields = entry.Key;
                List<string> objTypeKeys = entry.Value;

                ObjectAttribute obj = new ObjectAttribute(objFields.Item1, objFields.Item2, objFields.Item3, objFields.Item4, objFields.Item5, objTypeKeys);
                objectAttributes.Add(obj);
            }

            List<Timeseries> timeseries = new List<Timeseries>();
            foreach (KeyValuePair<TimeseriesFields, List<string>> entry in allTimeseries) {
                TimeseriesFields tsFields = entry.Key;
                List<string> eventTypeKeys = entry.Value;

                Timeseries ts = new Timeseries(tsFields.Item1, tsFields.Item2, tsFields.Item3, tsFields.Item4, eventTypeKeys);
                timeseries.Add(ts);
            }
            return new Model(
                eventTypes, objectTypes, timeseries, objectAttributes, ownerAttributes, sessionizers, new Orphans(orphanTimeseries, orphanObjects)
            );
        }
    }
}