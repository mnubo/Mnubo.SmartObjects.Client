using System;

namespace Mnubo.SmartObjects.Client
{
    /// <summary>
    /// Interface for work with all mnubo's client SmartObjects
    /// </summary>
    public interface ISmartObjectsClient : IDisposable
    {
        /// <summary>
        /// Returns SmartObject Client, giving access to mnubo SmartObjects
        /// </summary>
        IObjectClient Objects { get; }

        /// <summary>
        /// Returns Owner Client, giving access to mnubo owners
        /// </summary>
        IOwnerClient Owners { get; }

        /// <summary>
        /// Returns Event Client, giving access to post events
        /// </summary>
        IEventClient Events { get; }

        /// <summary>
        /// Returns Search API Client, giving access to post search requests
        /// </summary>
        IRestitutionClient Restitution { get;  }

        /// <summary>
        /// Returns Model API Client, giving access to the model
        /// </summary>
        IModelClient Model { get;  }
    }
}