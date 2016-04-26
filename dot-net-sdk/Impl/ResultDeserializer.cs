using Mnubo.SmartObjects.Client.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using static Mnubo.SmartObjects.Client.Models.Result;

namespace Mnubo.SmartObjects.Client.Impl
{
    /// <summary>
    /// Json deserialize Result instances
    /// </summary>
    public class ResultDeserializer
    {
        private const string IdResourceProperty = "id";
        private const string ResultStateProperty = "result";
        private const string MessageProperty = "message";

        /// <summary>
        ///  deserialize a json string to a smartObject instance
        /// </summary>
        /// <param name="obj">json string</param>
        /// <returns>List of result instance</returns>
        public static List<Result> DeserializeSmartObject(string obj)
        {
            List<Result> results = new List<Result>();

            foreach (Dictionary<string, string> resultAsDictionary in
                JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(obj))
            {
                string resourceId = null;
                string ResourceMessage = null;
                ResultStates resultState = ResultStates.Error;

                foreach (KeyValuePair<string, string> token in resultAsDictionary)
                {
                    switch (token.Key)
                    {
                        case IdResourceProperty:
                            {
                                resourceId = token.Value;
                                break;
                            }
                        case ResultStateProperty:
                            {
                                if (token.Value.Equals("success"))
                                {
                                    resultState = ResultStates.Success;
                                }
                                else if (token.Value.Equals("error"))
                                {
                                    resultState = ResultStates.Error;
                                }
                                break;
                            }
                        case MessageProperty:
                            {
                                ResourceMessage = token.Value;
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                results.Add(new Result(resourceId, resultState, ResourceMessage));
            }
            return results;
        }
    }
}
