using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Mnubo.SmartObjects.Client.Models.DataModel;

namespace Mnubo.SmartObjects.Client.Impl
{
    internal interface Serializer<A>
    {
        string Serialize(IEnumerable<A> v);

        string Serialize(A obj);
        List<A> Deserialize(string json);

        A Deserialize(JObject obj);
    }
}