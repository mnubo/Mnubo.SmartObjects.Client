using Mnubo.SmartObjects.Client.Models;
using Mnubo.SmartObjects.Client.Impl;
using Mnubo.SmartObjects.Client.Config;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace Mnubo.SmartObjects.Client.ITTest
{
    public class ITTestHelper
    {

        public static ISmartObjectsClient newClient()  {
            if (Environment.GetEnvironmentVariable("CONSUMER_KEY") == null ||
                Environment.GetEnvironmentVariable("CONSUMER_SECRET") == null) {
                throw new Exception("Consumer key/secret are unvailable");
            }

            return ClientFactory.Create(
                new ClientConfig.Builder()
                {
                    Hostname = ClientConfig.Environments.Sandbox,
                    ConsumerKey = Environment.GetEnvironmentVariable("CONSUMER_KEY"),
                    ConsumerSecret = Environment.GetEnvironmentVariable("CONSUMER_SECRET")
                }
            );
        }
        private const int timeout = 1000 * 240;
        private const int delay = 5000;

        public static readonly Func<String, String> searchObjectQuery = deviceId =>
            @"{""from"":""object"",""select"":[{""value"":""x_device_id""},{""value"":""object_text_attribute""}],""where"":{""x_device_id"":{""eq"":""" + deviceId + @"""}}}";

        public static readonly Func<String, String> searchEventQuery = eventId =>
            @"{""from"":""event"",""select"":[{""value"":""event_id""},{""value"":""ts_text_attribute""}],""where"":{""event_id"":{""eq"":""" + eventId + @"""}}}";

        public static readonly Func<String, String> searchOwnerQuery = username =>
            @"{""from"":""owner"",""select"":[{""value"":""username""},{""value"":""owner_text_attribute""}],""where"":{""username"":{""eq"":""" + username + @"""}}}";

        public static readonly Func<String, String> searchObjectByOwnerQuery = username =>
            @"{""from"":""object"",""select"":[{""value"":""x_device_id""},{""value"":""object_text_attribute""}],""where"":{""x_owner.username"":{""eq"":""" + username + @"""}}}";
        
        public static void AllFailed(IEnumerable<Result> results)
        {
            if (results != null) {
                foreach (Result result in results) {
                    Assert.AreEqual(Result.ResultStates.Error, result.ResultState);
                }
            }
        }
        public static void AllSuccess(IEnumerable<Result> results)
        {
            if (results != null) {
                foreach (Result result in results) {
                    Assert.AreEqual(Result.ResultStates.Success, result.ResultState);
                }
            }
        }
    }
}
