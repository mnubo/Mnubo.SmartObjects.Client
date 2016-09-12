using Mnubo.SmartObjects.Client.Config;

namespace Mnubo.SmartObjects.Client.Impl
{
    public sealed class ClientFactory
    {
        public static ISmartObjectsClient Create(ClientConfig config)
        {
            return new SmartObjectsClient(config);
        }
    }
}