using System.Collections.Generic;

namespace Mnubo.SmartObjects.Client.Models.DataModel
{
    public sealed class ObjectAttribute
    {
        public string Key { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public string HighLevelType { get; }
        public string ContainerType { get; }
        public List<string> ObjectTypeKeys { get; }
        public ObjectAttribute(string key, string displayName, string description, string highLevelType, string containerType, List<string> objectTypeKeys) 
        {
            this.Key = key;
            this.DisplayName = displayName;
            this.Description = description;
            this.HighLevelType = highLevelType;
            this.ContainerType = containerType;
            this.ObjectTypeKeys = objectTypeKeys;
        }
    }
}