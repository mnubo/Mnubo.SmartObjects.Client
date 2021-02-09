using AspenTech.SmartObjects.Client.Config;
using AspenTech.SmartObjects.Client.Impl;

namespace AspenTech.SmartObjects.Client
{
    /// <summary>
    /// Facility to instantiate a SmartObjectClient
    /// </summary>
    public sealed class ClientFactory
    {
        /// <summary>
        /// Function to create a SmartObjectClient from a ClientConfig
        /// </summary>
        public static ISmartObjectsClient Create(ClientConfig config)
        {
            return new SmartObjectsClient(config);
        }
    }
}