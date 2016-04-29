using System.Collections.Generic;

namespace Mnubo.SmartObjects.Client.Models.Search
{
    public class DataSet
    {
        /// <summary>
        /// Description of the dataset
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Displayname of the dataset
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// fields of the dataset
        /// </summary>
        public HashSet<Field> Fields { get; }

        /// <summary>
        /// Key of the dataset
        /// </summary>
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
