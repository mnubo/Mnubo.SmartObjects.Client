using Newtonsoft.Json;

namespace Mnubo.SmartObjects.Client.Models.Search
{
    public class Field
    {
        /// <summary>
        /// Container type values
        /// </summary>
        public enum ContainerTypes
        {
            None,
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
        /// Creat a new instance
        /// </summary>
        /// <param name="containerType">container type</param>
        /// <param name="description">drescription</param>
        /// <param name="displayName">display name</param>
        /// <param name="highLevelType">High level type</param>
        /// <param name="key">key</param>
        /// <param name="isPrimaryKey">is primary key</param>
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
