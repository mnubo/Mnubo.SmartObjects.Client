using System.Collections.Generic;

namespace Mnubo.SmartObjects.Client.Models.DataModel
{
    /// <summary>
    /// </summary>
    public sealed class Sessionizer
    {
        public string Key { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public string StartEventTypeKey { get; }
        public string EndEventTypeKey { get; }
        public Sessionizer(string key, string displayName, string description, string startEventTypeKey, string endEventTypeKey) 
        {
            this.Key = key;
            this.DisplayName = displayName;
            this.Description = description;
            this.StartEventTypeKey = startEventTypeKey;
            this.EndEventTypeKey = endEventTypeKey;
        }
    }
}