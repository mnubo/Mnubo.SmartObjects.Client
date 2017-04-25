using Mnubo.SmartObjects.Client.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace Mnubo.SmartObjects.Client.ITTest
{
    public class ITTestHelper
    {
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

        public static void EventuallyAssert(Action assert)
        {
            int stopper = Environment.TickCount + timeout;
            AssertionException lastAssertionException = null;
            while (Environment.TickCount < stopper) {
                try {
                    assert();
                    return;
                } catch (AssertionException e) {
                    lastAssertionException = e;
                }
                Thread.Sleep(delay);
            }
            if (lastAssertionException != null) {
                throw lastAssertionException;
            }
        }
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
