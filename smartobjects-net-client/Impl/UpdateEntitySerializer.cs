using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Mnubo.SmartObjects.Client.Models.DataModel;

namespace Mnubo.SmartObjects.Client.Impl
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