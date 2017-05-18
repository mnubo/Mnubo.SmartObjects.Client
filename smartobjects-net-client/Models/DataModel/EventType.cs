using System.Collections.Generic;

namespace Mnubo.SmartObjects.Client.Models.DataModel
{
    
    /// <summary>
    ///  An event type of the model
    /// </summary>
    public sealed class EventType
    {

        /// <summary>
        ///  Unique identifer of the event type
        /// </summary>
        public string Key { get; }

        /// <summary>
        ///  Description of the event type
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///  Origin of the event type
        ///  https://smartobjects.mnubo.com/documentation/api_search.html#exportdatamodel
        /// </summary>
        public string Origin { get; }

        /// <summary>
        ///  All the timeseries that are bound to this event type
        /// </summary>
        public List<string> TimeseriesKeys { get; }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="key">See <see cref="Key" /></param>
        /// <param name="description">See <see cref="Description" /></param>
        /// <param name="origin">See <see cref="Origin" /></param>
        /// <param name="timeseriesKeys">See <see cref="TimeseriesKeys" /></param>
        public EventType(string key, string description, string origin, List<string> timeseriesKeys) 
        {
            this.Key = key;
            this.Description = description;
            this.Origin = origin;
            this.TimeseriesKeys = timeseriesKeys;
        }
    }
}