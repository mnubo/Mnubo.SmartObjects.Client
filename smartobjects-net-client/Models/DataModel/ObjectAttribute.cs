using System.Collections.Generic;

namespace Mnubo.SmartObjects.Client.Models.DataModel
{
    /// <summary>
    ///  An object attribute of the model
    /// </summary>
    public sealed class ObjectAttribute
    {
        /// <summary>
        ///  Unique identifer of the object attribute
        /// </summary>
        public string Key { get; }

        /// <summary>
        ///  Display name of the object attribute
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        ///  Description of the object attribute
        /// </summary>
        public string Description { get; }


        /// <summary>
        ///  High level type of the object attribute
        ///  https://smartobjects.mnubo.com/documentation/references.html#high-level-data-types
        /// </summary>
        public string HighLevelType { get; }

        /// <summary>
        ///  Container type of the object attribute
        /// </summary>
        public string ContainerType { get; }

        /// <summary>
        ///  Object type keys that this object attribute is bound to
        /// </summary>
        public List<string> ObjectTypeKeys { get; }

        /// <summary>
        ///  Constructor to create an object attribute instance
        /// </summary>
        /// <param name="key">See <see cref="Key" /></param>
        /// <param name="displayName">See <see cref="DisplayName" /></param>
        /// <param name="description">See <see cref="Description" /></param>
        /// <param name="highLevelType">See <see cref="HighLevelType" /></param>
        /// <param name="containerType">See <see cref="ContainerType" /></param>
        /// <param name="objectTypeKeys">See <see cref="ObjectTypeKeys" /></param>
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