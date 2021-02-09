using System.Collections.Generic;

namespace AspenTech.SmartObjects.Client.Models.DataModel
{
    /// <summary>
    ///  An object type of the model
    /// </summary>
    public sealed class ObjectType {

        /// <summary>
        ///  Unique identifer of the object type
        /// </summary>
        public string Key { get; }

        /// <summary>
        ///  Description of the object type
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///  All the object attributes that are bound to this object type
        /// </summary>
        public List<string> ObjectAttributeKeys { get; }
        

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="key">See <see cref="Key" /></param>
        /// <param name="description">See <see cref="Description" /></param>
        /// <param name="objectAttributeKeys">See <see cref="ObjectAttributeKeys" /></param>
        public ObjectType(string key, string description, List<string> objectAttributeKeys) 
        {
            this.Key = key;
            this.Description = description;
            this.ObjectAttributeKeys = objectAttributeKeys;
        }
    }
}