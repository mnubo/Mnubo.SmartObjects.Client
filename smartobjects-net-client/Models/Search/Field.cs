using Newtonsoft.Json;

namespace Mnubo.SmartObjects.Client.Models.Search
{
    /// <summary>
    /// One field of a <see cref="DataSet"/>
    /// </summary>
    public class Field
    {
        /// <summary>
        /// Container type values
        /// </summary>
        public enum ContainerTypes
        {
            /// <summary>
            /// No container
            /// </summary>
            None,
            
            /// <summary>
            /// A list container
            /// </summary>
            List
        }

        /// <summary>
        /// Container type of the field
        /// </summary>
        [JsonProperty("containerType")]
        public ContainerTypes ContainerType { get; }

        /// <summary>
        /// Descriuption of the field
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; }

        /// <summary>
        /// Display name of the field
        /// </summary>
        [JsonProperty("displayName")]
        public string DisplayName { get; }

        /// <summary>
        /// High level type of the field
        /// </summary>
        [JsonProperty("highLevelType")]
        public string HighLevelType { get; }

        /// <summary>
        /// Key of the field
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; }

        /// <summary>
        /// Is it a primary key.
        /// </summary>
        [JsonProperty("primaryKey")]
        public bool IsPrimaryKey { get; }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="containerType">See <see cref="ContainerType" /></param>
        /// <param name="description">See <see cref="Description" /></param>
        /// <param name="displayName">See <see cref="DisplayName" /></param>
        /// <param name="highLevelType">See <see cref="HighLevelType" /></param>
        /// <param name="key">See <see cref="Key" /></param>
        /// <param name="isPrimaryKey">See <see cref="IsPrimaryKey" /></param>
        public Field(ContainerTypes containerType, string description, string displayName, string highLevelType, string key, bool isPrimaryKey)
        {
            ContainerType = containerType;
            Description = description;
            DisplayName = displayName;
            HighLevelType = highLevelType;
            Key = key;
            IsPrimaryKey = isPrimaryKey;
        }
    }
}
