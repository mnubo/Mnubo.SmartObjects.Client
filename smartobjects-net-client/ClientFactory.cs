using Mnubo.SmartObjects.Client.Config;

namespace Mnubo.SmartObjects.Client.Impl
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