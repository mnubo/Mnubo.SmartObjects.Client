using System;

namespace Mnubo.SmartObjects.Client
{
    public interface ISmartObjectsClient : IDisposable
    {
        /// <summary>
        /// Returns SmartObject Client, giving access to handle mnubo smartObjects.
        /// </summary>
        /// <returns>Object client</returns>
        IObjectClient Objects { get; }

        /// <summary>
        /// Returns Owner Client, giving access to handle mnubo owners.
        /// </summary>
        /// <returns>Owner client</returns>
        IOwnerClient Owners { get; }

        /// <summary>
        /// Returns Event Client, giving access to post events.
        /// </summary>
        /// <returns>Event client</returns>
        IEventClient Events { get; }
    }
}