using System;
using AspenTech.SmartObjects.Client.Datalake;

namespace AspenTech.SmartObjects.Client
{
    public interface ISmartObjectsClient : IDisposable
    {
        /// <summary>
        /// Returns SmartObject Client, giving access to AspenTech SmartObjects
        /// </summary>
        IObjectClient Objects { get; }

        /// <summary>
        /// Returns Owner Client, giving access to AspenTech owners
        /// </summary>
        IOwnerClient Owners { get; }

        /// <summary>
        /// Returns Event Client, giving access to post events
        /// </summary>
        IEventClient Events { get; }

        /// <summary>
        /// Returns Search API Client, giving access to post search requests
        /// </summary>
        IRestitutionClient Restitution { get; }

        /// <summary>
        /// Returns Model API Client, giving access to the model
        /// </summary>
        IModelClient Model { get; }

        IDatalakeClient Datalake { get; }
    }
}