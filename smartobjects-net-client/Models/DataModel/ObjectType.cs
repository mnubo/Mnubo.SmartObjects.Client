using System.Collections.Generic;

namespace Mnubo.SmartObjects.Client.Models.DataModel
{
    public sealed class ObjectType {

        public string Key { get; }
        public string Description { get; }
        public List<string> ObjectAttributeKeys { get; }
        public ObjectType(string key, string description, List<string> objectAttributeKeys) 
        {
            this.Key = key;
            this.Description = description;
            this.ObjectAttributeKeys = objectAttributeKeys;
        }
    }
}