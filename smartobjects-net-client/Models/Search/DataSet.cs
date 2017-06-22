using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mnubo.SmartObjects.Client.Models.Search
{
    /// <summary>
    /// One Dataset in a result from a <see cref="IRestitutionClient.GetDataSets"/> call
    /// </summary>
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
        /// Fields of the dataset
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
        /// <param name="description">See <see cref="Description" /></param>
        /// <param name="displayName">See <see cref="DisplayName" /></param>
        /// <param name="fields">See <see cref="Fields" /></param>
        /// <param name="key">See <see cref="Key" /></param>
        public DataSet(string description, string displayName, HashSet<Field> fields, string key)
        {
            Description = description;
            DisplayName = displayName;
            Fields = fields;
            Key = key;
        }
    }
}
