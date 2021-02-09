using Newtonsoft.Json;
using System.Collections.Generic;
using AspenTech.SmartObjects.Client.Models;

namespace AspenTech.SmartObjects.Client.Impl
{
    internal class ResultDeserializer
    {
        private const string IdResourceProperty = "id";
        private const string ResultStateProperty = "result";
        private const string MessageProperty = "message";

        internal static List<Result> DeserializeSmartObject(string obj)
        {
            List<Result> results = new List<Result>();

            foreach (Dictionary<string, string> resultAsDictionary in
                JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(obj))
            {
                string resourceId = null;
                string ResourceMessage = null;
                Result.ResultStates resultState = Result.ResultStates.Error;

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
                                    resultState = Result.ResultStates.Success;
                                }
                                else if (token.Value.Equals("error"))
                                {
                                    resultState = Result.ResultStates.Error;
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
