using System.Collections.Generic;

namespace Mnubo.SmartObjects.Client.Models.DataModel
{
    /// <summary>
    ///  A timeseries of the model
    /// </summary>
    public sealed class Timeseries
    {

        /// <summary>
        ///  Unique identifer of the timeseries
        /// </summary>
        public string Key { get; }

        /// <summary>
        ///  Display name of the timeseries
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        ///  Description of the timeseries
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///  High level type of the timeseries
        ///  https://smartobjects.mnubo.com/documentation/references.html#high-level-data-types
        /// </summary>
        public string HighLevelType { get; }

        /// <summary>
        ///  Event type keys that this timeseries is bound to
        /// </summary>
        public List<string> EventTypeKeys { get; }

        /// <summary>
        ///  Constructor to create an timeseries instance
        /// </summary>
        /// <param name="key">See <see cref="Key" /></param>
        /// <param name="displayName">See <see cref="DisplayName" /></param>
        /// <param name="description">See <see cref="Description" /></param>
        /// <param name="type">See <see cref="HighLevelType" /></param>
        /// <param name="eventTypeKeys">See <see cref="EventTypeKeys" /></param>
        public Timeseries(string key, string displayName, string description, string type, List<string> eventTypeKeys) 
        {
            this.Key = key;
            this.DisplayName = displayName;
            this.Description = description;
            this.HighLevelType = type;
            this.EventTypeKeys = eventTypeKeys;
        }
    }
}