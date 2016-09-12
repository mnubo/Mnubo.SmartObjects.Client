using Mnubo.SmartObjects.Client.Models.Search;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mnubo.SmartObjects.Client.Impl
{
    internal class ExistResultsDeserializer
    {
        internal static IDictionary<string, bool> DeserializeExistResults(string obj)
        {
            IDictionary<string, bool> results = new Dictionary<string, bool>();
            IEnumerable<IDictionary<string, bool>> rawResults =
                JsonConvert.DeserializeObject<IEnumerable<IDictionary<string, bool>>>(obj);

            foreach (IDictionary<string, bool> singleResult in rawResults)
            {
                foreach (KeyValuePair<string, bool> entry in singleResult)
                {
                    results.Add(entry.Key, entry.Value);
                }
            }

            return results;
        }
    }
}
