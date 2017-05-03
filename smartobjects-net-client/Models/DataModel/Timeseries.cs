using System.Collections.Generic;

namespace Mnubo.SmartObjects.Client.Models.DataModel
{
    public sealed class Timeseries
    {
        public string Key { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public string HighLevelType { get; }
        public List<string> EventTypeKeys { get; }
        public Timeseries(string key, string displayName, string description, string type, List<string> eventTypeKeys) 
        {
            this.Key = key;
            this.DisplayName = displayName;
            this.Description = description;
            this.HighLevelType = type;
            this.EventTypeKeys = eventTypeKeys;
        }
    }
}