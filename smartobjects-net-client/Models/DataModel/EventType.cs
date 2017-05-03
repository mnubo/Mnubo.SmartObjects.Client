using System.Collections.Generic;

namespace Mnubo.SmartObjects.Client.Models.DataModel
{
    public sealed class EventType
    {
        public string Key { get; }
        public string Description { get; }
        public string Origin { get; }
        public List<string> TimeseriesKeys { get; }
        public EventType(string key, string description, string origin, List<string> timeseriesKeys) 
        {
            this.Key = key;
            this.Description = description;
            this.Origin = origin;
            this.TimeseriesKeys = timeseriesKeys;
        }
    }
}