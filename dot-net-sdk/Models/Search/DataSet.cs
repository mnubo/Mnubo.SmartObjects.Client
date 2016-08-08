using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mnubo.SmartObjects.Client.Models.Search
{
    public class DataSet
    {
        /// <summary>
        /// Description of the dataset
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; }

        /// <summary>
        /// Displayname of the dataset
        /// </summary>
        [JsonProperty("displayName")]
        public string DisplayName { get; }

        /// <summary>
        /// fields of the dataset
        /// </summary>
        [JsonProperty("fields")]
        public HashSet<Field> Fields { get; }

        /// <summary>
        /// Key of the dataset
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="description">description</param>
        /// <param name="displayName">display name</param>
        /// <param name="fields">fields</param>
        /// <param name="key">key</param>
        public DataSet(string description, string displayName, HashSet<Field> fields, string key)
        {
            Description = description;
            DisplayName = displayName;
            Fields = fields;
            Key = key;
        }
    }
}
