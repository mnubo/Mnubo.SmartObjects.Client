using System.Collections.Generic;
using Newtonsoft.Json;

namespace AspenTech.SmartObjects.Client.Impl
{
    internal class UpdateEntitySerializer
    {
        public string Serialize(UpdateEntity ue)
        {
            return JsonConvert.SerializeObject(
                new Dictionary<string, object> {
                    { "displayName", ue.DisplayName },
                    { "description", ue.Description }
                }
            );
        }
    }
}