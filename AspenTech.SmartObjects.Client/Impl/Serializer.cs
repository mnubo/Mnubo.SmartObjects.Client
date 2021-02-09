using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace AspenTech.SmartObjects.Client.Impl
{
    internal interface Serializer<A>
    {
        string Serialize(IEnumerable<A> v);

        string Serialize(A obj);
        List<A> Deserialize(string json);

        A Deserialize(JObject obj);
    }
}