using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mnubo.SmartObjects.Client.Impl
{
    internal class SerializerUtils
    {
        internal static List<A> JArrayToList<A>(JArray array) {
            List<A> list = new List<A>();
            if (array == null) {
                return list;
            }
            
            foreach (var item in array)
            {
                list.Add((A) Convert.ChangeType(item, typeof(A)));
            }
            return list;
        }
    }
}
