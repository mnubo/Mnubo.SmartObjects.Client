using System.Collections.Generic;

namespace Mnubo.SmartObjects.Client.Models.DataModel
{
    /// <summary>
    ///  An sessionizer of the model
    /// </summary>
    public sealed class Sessionizer
    {

        /// <summary>
        ///  Unique identifer of the sessionizer
        /// </summary>
        public string Key { get; }

        /// <summary>
        ///  Display name of the sessionizer
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        ///  Description of the sessionizer
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///  Event type that triggers a new session
        /// </summary>
        public string StartEventTypeKey { get; }

        /// <summary>
        ///  Event type that ends a session
        /// </summary>
        public string EndEventTypeKey { get; }

        /// <summary>
        ///  Constructor to create a sessionizer instance
        /// </summary>
        /// <param name="key">See <see cref="Key" /></param>
        /// <param name="displayName">See <see cref="DisplayName" /></param>
        /// <param name="description">See <see cref="Description" /></param>
        /// <param name="startEventTypeKey">See <see cref="StartEventTypeKey" /></param>
        /// <param name="endEventTypeKey">See <see cref="EndEventTypeKey" /></param>
        public Sessionizer(string key, string displayName, string description, string startEventTypeKey, string endEventTypeKey) 
        {
            this.Key = key;
            this.DisplayName = displayName;
            this.Description = description;
            this.StartEventTypeKey = startEventTypeKey;
            this.EndEventTypeKey = endEventTypeKey;
        }
    }
}