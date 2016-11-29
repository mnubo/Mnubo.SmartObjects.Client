using Newtonsoft.Json;

namespace Mnubo.SmartObjects.Client.Models.Search
{
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
        /// <param name="label">Label associate to the column</param>
        /// <param name="highLevelType">High level type associate to the column</param>
        public ColumnDefinition(string label, string highLevelType)
        {
            Label = label;
            HighLevelType = highLevelType;
        }
    }
}
