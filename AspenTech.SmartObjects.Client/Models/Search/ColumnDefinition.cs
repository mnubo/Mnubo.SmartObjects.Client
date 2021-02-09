using Newtonsoft.Json;

namespace AspenTech.SmartObjects.Client.Models.Search
{
    
    /// <summary>
    /// One column in a result from a <see cref="IRestitutionClient.Search(string)"/> call
    /// </summary>
    public class ColumnDefinition
    {
        /// <summary>
        /// Label associated to the column
        /// </summary>
        [JsonProperty("label")]
        public string Label { get; }

        /// <summary>
        /// Highlevel type associated to the column
        /// </summary>
        [JsonProperty("type")]
        public string HighLevelType { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="label">See <see cref="Label" /></param>
        /// <param name="highLevelType">See <see cref="HighLevelType" /></param>
        public ColumnDefinition(string label, string highLevelType)
        {
            Label = label;
            HighLevelType = highLevelType;
        }
    }
}
