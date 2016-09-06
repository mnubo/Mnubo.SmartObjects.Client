using Mnubo.SmartObjects.Client.Impl;
using Mnubo.SmartObjects.Client.Models;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnubo.SmartObjects.Client.Test.Impl
{
    public class CustomBootStrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            StaticConfiguration.EnableRequestTracing = true;
        }
    }

    public class NancyUtils
    {
        internal static String BodyAsString(Request request)
        {
            var entity = request.Body;
            byte[] data = new byte[entity.Length];
            entity.Read(data, 0, (int)entity.Length);

            if ( IsGzipCompressed(request) )
            {
                return GzipDecompress(data);
            }
            else
            {
                return Encoding.Default.GetString(data);
            }
        }

        internal static bool IsGzipCompressed(Request request)
        {
            return request.Headers.Any(
                header => string.Equals(header.Key, "Content-Encoding", StringComparison.OrdinalIgnoreCase) && header.Value.Contains("gzip")
            );
        }

        internal static string GzipDecompress(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            using (var gz = new GZipStream(stream, CompressionMode.Decompress))
            using (var reader = new StreamReader(gz))
            {
                return reader.ReadToEnd();
            }
        }

        internal static byte[] GzipCompress(string body)
        {
            var data = Encoding.UTF8.GetBytes(body);
            var stream = new MemoryStream();
            using (var gz = new GZipStream(stream, CompressionMode.Compress))
            {
                gz.Write(data, 0, data.Length);
            }
            return stream.ToArray();
        }
    }

    public class AuthenticationMockModule : Nancy.NancyModule
    {
        public AuthenticationMockModule()
        {
            Post["/oauth/token"] = x => {
                var response = (Response)"{\"access_token\":\"thetoken\",\"token_type\":\"Bearer\",\"expires_in\":1000000,\"scope\":\"ALL\"}";
                response.ContentType = "application/json";
                return response;
            };
        }
    }

    public class SucceedAPIsMockModule : Nancy.NancyModule
    {
        internal static string BasePath = "/succeed/api/v2/objects/";
        internal static string TestJsonString = @"[{""x_event_type"":""wind_direction_changed"",""x_object"":{""x_device_id"":""deviceId""},""wind_direction"":""sdktest854070""} , {""x_event_type"":""wind_direction_changed"",""x_object"":{""x_device_id"":""deviceId""},""wind_direction"":""sdktest90186""}]";

        public SucceedAPIsMockModule()
        {
            Post[BasePath + "objects"] = x => {
                TestUtils.AssertObjectEquals(TestUtils.CreateTestObject(), ObjectSerializer.DeserializeObject(NancyUtils.BodyAsString(this.Request)));
                return 201;
            };

            Put[BasePath + "objects"] = x => {
                List<SmartObject> objects = TestUtils.DeserializeObjects(NancyUtils.BodyAsString(this.Request));
                TestUtils.AssertObjectsEqual(TestUtils.CreateObjects(objects.Count<SmartObject>()), objects);

                List<Result> results = new List<Result>();
                foreach (SmartObject anObject in objects)
                {
                    results.Add(new Result(anObject.DeviceId, Result.ResultStates.Success, null));
                }

                return JsonConvert.SerializeObject(results);
            };

            Put[BasePath + "objects/{deviceId}"] = x => {
                TestUtils.AssertObjectEquals(TestUtils.CreateObjectUpdateAttribute(), ObjectSerializer.DeserializeObject(NancyUtils.BodyAsString(this.Request)));
                Assert.AreEqual(TestUtils.DeviceId, (string)x.deviceId);
                return 200;
            };

            Delete[BasePath + "objects/{deviceId}"] = x => {

                Assert.AreEqual(TestUtils.DeviceId, (string)x.deviceId);
                return 200;
            };

            Get[BasePath + "objects/exists/{deviceId}"] = x => {
                var deviceId = (string)x.deviceId;
                Assert.AreEqual(TestUtils.DeviceId, deviceId);
                IDictionary<string, bool> result = new Dictionary<string, bool>()
                {
                    {deviceId,true}
                };
                return JsonConvert.SerializeObject(result);
            };

            Post[BasePath + "owners"] = x => {
                TestUtils.AssertOwnerEquals(TestUtils.CreateTestOwner(), OwnerSerializer.DeserializeOwner(NancyUtils.BodyAsString(this.Request)));
                return 201;
            };

            Put[BasePath + "owners"] = x => {
                List<Owner> owners =TestUtils.DeserializeOwners(NancyUtils.BodyAsString(this.Request));
                TestUtils.AssertOwnersEqual(TestUtils.CreateOwners(owners.Count<Owner>()), owners);

                List<Result> results = new List<Result>();
                foreach(Owner owner in owners)
                {
                    results.Add(new Result(owner.Username, Result.ResultStates.Success, null));
                }

                return JsonConvert.SerializeObject(results);
            };

            Put[BasePath + "owners/{username}"] = x => {
                TestUtils.AssertOwnerEquals(TestUtils.CreateOwnerUpdateAttribute(), OwnerSerializer.DeserializeOwner(NancyUtils.BodyAsString(this.Request)));
                Assert.AreEqual(TestUtils.Username, (string)x.username);
                return 200;
            };

            Put[BasePath + "owners/{username}/password"] = x => {
                Dictionary<string, string> body = JsonConvert.DeserializeObject<Dictionary<string, string>>(NancyUtils.BodyAsString(this.Request));
                Assert.IsTrue(body.ContainsKey("x_password"));
                String password = "";
                body.TryGetValue("x_password", out password);
                Assert.AreEqual(TestUtils.Password, password);
                Assert.AreEqual(TestUtils.Username, (string)x.username);
                return 200;
            };

            Post[BasePath + "owners/{username}/objects/{deviceId}/claim"] = x => {
                Assert.AreEqual(TestUtils.DeviceId, (string)x.deviceId);
                Assert.AreEqual(TestUtils.Username, (string)x.username);
                return 200;
            };

            Delete[BasePath + "owners/{username}"] = x => {

                Assert.AreEqual(TestUtils.Username, (string)x.username);
                return 200;
            };

            Get[BasePath + "owners/exists/{username}"] = x => {
                string username = (string)x.username;
                Assert.AreEqual(TestUtils.Username, username);
                IDictionary<string, bool> result = new Dictionary<string, bool>()
                {
                    {username,true}
                };
                return JsonConvert.SerializeObject(result);
            };

            Post[BasePath + "events"] = x => {
                bool reportResults = (bool)this.Request.Query["report_results"];

                Assert.AreEqual(true, reportResults);
                List<EventResult> results = TestUtils.EventsToSuccesfullResults(NancyUtils.BodyAsString(this.Request));
                return JsonConvert.SerializeObject(results);
            };

            Get[BasePath + "events/exists/{eventId}"] = x => {
                IDictionary<string, bool> result = new Dictionary<string, bool>()
                {
                    {(string)x.eventId,true}
                };
                return JsonConvert.SerializeObject(result);
            };

            Get[BasePath + "search/datasets"] = x => {
                return JsonConvert.SerializeObject(TestUtils.CreateDatasets());
            };

            Post[BasePath + "search/basic"] = x => {
                Assert.AreEqual(TestUtils.CreateQuery(), NancyUtils.BodyAsString(this.Request));
                return TestUtils.CreateExpectedSearchResult();
            };

            // test HttpClient itself
            Post[BasePath + "compressed"] = x => {
                if (!NancyUtils.IsGzipCompressed(this.Request))
                {
                    return FailedAPIsMockModule.badRequest();
                }

                if (!this.Request.Headers.AcceptEncoding.Contains("gzip"))
                {
                    return FailedAPIsMockModule.badRequest();
                }

                var body = NancyUtils.BodyAsString(this.Request);
                if(body != TestJsonString)
                {
                    return FailedAPIsMockModule.badRequest();
                }

                var data = Encoding.UTF8.GetBytes(body);
                var response = new Response();
                response.Headers.Add("Content-Encoding", "gzip");
                response.Headers.Add("Content-Type", "application/json");
                response.Contents = stream =>
                {
                    using (var gz = new GZipStream(stream, CompressionMode.Compress))
                    {
                        gz.Write(data, 0, data.Length);
                        gz.Flush();
                    }
                };
                return response;
            };

            Post[BasePath + "decompressed"] = x => {
                if (NancyUtils.IsGzipCompressed(this.Request))
                {
                    return FailedAPIsMockModule.badRequest();
                }

                var body = NancyUtils.BodyAsString(this.Request);
                if (body != TestJsonString)
                {
                    return FailedAPIsMockModule.badRequest();
                }

                return TestJsonString;
            };
        }
    }

    public class BatchAPIsMockModule : Nancy.NancyModule
    {
        internal static string BasePath = "/batch/api/v2/objects/";
        internal static string ErrorMessage = "event failed";

        public BatchAPIsMockModule()
        {

            Put[BasePath + "owners"] = x => {
                List<Owner> owners = TestUtils.DeserializeOwners(NancyUtils.BodyAsString(this.Request));
                CollectionAssert.AreEqual(TestUtils.CreateOwners(owners.Count<Owner>()), owners);

                int counter = 0;
                List<Result> results = new List<Result>();
                foreach (Owner owner in owners)
                {
                    if( counter % 2 == 0)
                    {
                        results.Add(new Result(owner.Username, Result.ResultStates.Success, null));
                    }
                    else
                    {
                        results.Add(new Result(owner.Username, Result.ResultStates.Error, TestUtils.ErrorMessage));
                    }
                    
                }

                var response = (Response)JsonConvert.SerializeObject(results);
                response.StatusCode = HttpStatusCode.MultipleStatus;
                response.ContentType = "application/json";
                return response;
            };

            Post[BasePath + "owners/exists"] = x => {
                List<string> usernames = JsonConvert.DeserializeObject<List<string>>(NancyUtils.BodyAsString(this.Request));

                List<Dictionary<string, bool>> results = new List<Dictionary<string, bool>>();

                int counter = 0;
                foreach (var username in usernames)
                {
                    results.Add(new Dictionary<string, bool>()
                    {
                        { username, counter % 2 == 0 ? true : false }
                    });
                    ++counter;
                }

                return JsonConvert.SerializeObject(results);
            };

            Get[BasePath + "owners/exists/{username}"] = x => {
                var username = (string)x.username;
                Assert.AreEqual(TestUtils.Username, username);
                IDictionary<string, bool> result = new Dictionary<string, bool>()
                {
                    {username,false}
                };
                return JsonConvert.SerializeObject(result);
            };

            Put[BasePath + "objects"] = x => {
                List<SmartObject> objects = TestUtils.DeserializeObjects(NancyUtils.BodyAsString(this.Request));
                CollectionAssert.AreEqual(TestUtils.CreateObjects(objects.Count<SmartObject>()), objects);

                int counter = 0;
                List<Result> results = new List<Result>();
                foreach (SmartObject anObject in objects)
                {
                    if (counter % 2 == 0)
                    {
                        results.Add(new Result(anObject.DeviceId, Result.ResultStates.Success, null));
                    }
                    else
                    {
                        results.Add(new Result(anObject.DeviceId, Result.ResultStates.Error, TestUtils.ErrorMessage));
                    }

                }

                var response = (Response)JsonConvert.SerializeObject(results);
                response.StatusCode = HttpStatusCode.MultipleStatus;
                response.ContentType = "application/json";
                return response;
            };

            Post[BasePath + "objects/exists"] = x => {
                List<string> deviceIds = JsonConvert.DeserializeObject<List<string>>(NancyUtils.BodyAsString(this.Request));

                List<Dictionary<string, bool>> results = new List<Dictionary<string, bool>>();

                int counter = 0;
                foreach (var deviceId in deviceIds)
                {
                    results.Add(new Dictionary<string, bool>()
                    {
                        { deviceId, counter % 2 == 0 ? true : false }
                    });
                    ++counter;
                }

                return JsonConvert.SerializeObject(results);
            };

            Get[BasePath + "objects/exists/{deviceId}"] = x => {
                string deviceId = (string)x.deviceId;
                Assert.AreEqual(TestUtils.DeviceId, deviceId);
                IDictionary<string, bool> result = new Dictionary<string, bool>()
                {
                    {deviceId,false}
                };
                return JsonConvert.SerializeObject(result);
            };

            Post[BasePath + "events"] = x =>
            {
                List<EventResult> results = new List<EventResult>();
                int i = 0;
                bool eventIdExists = false;

                foreach (var jsonObject in JArray.Parse(NancyUtils.BodyAsString(this.Request)))
                {

                    JObject jsonObj = (JObject)jsonObject;
                    Guid eventId;
                    JToken eventIdProperty;

                    if (jsonObj.TryGetValue(TestUtils.EventIdProperty, out eventIdProperty))
                    {
                        eventId = Guid.Parse((string)eventIdProperty);
                        eventIdExists = true;
                    }
                    else
                    {
                        eventId = Guid.NewGuid();
                        eventIdExists = false;
                    }

                    //Pair result succeed, odd failed
                    if (eventIdExists)
                    {
                        results.Add(new EventResult(eventId, true, EventResult.ResultStates.conflict, null));
                    }
                    else if (i % 3 == 1)
                    {
                        results.Add(new EventResult(eventId, true, EventResult.ResultStates.success, null));
                    }
                    else if (i % 3 == 2)
                    {
                        results.Add(new EventResult(eventId, false, EventResult.ResultStates.error, "Event Failed"));
                    }
                    else
                    {
                        results.Add(new EventResult(eventId, false, EventResult.ResultStates.success, null));
                    }
                    i++;
                }

                var response = (Response)JsonConvert.SerializeObject(results);
                response.StatusCode = HttpStatusCode.MultipleStatus;
                response.ContentType = "application/json";
                return response;
            };

            Post[BasePath + "events/exists"] = x => {
                List<Guid> ids = JsonConvert.DeserializeObject<List<Guid>>(NancyUtils.BodyAsString(this.Request));

                List<Dictionary<Guid, bool>> results = new List<Dictionary<Guid, bool>>();

                int counter = 0;
                foreach (var id in ids)
                {
                    results.Add(new Dictionary<Guid, bool>()
                    {
                        { id, counter % 2 == 0 ? true : false }
                    });
                    ++counter;
                }

                return JsonConvert.SerializeObject(results);
            };

            Get[BasePath + "events/exists/{eventId}"] = x => {
                IDictionary<string, bool> result = new Dictionary<string, bool>()
                {
                    {(string)x.eventId,false}
                };
                return JsonConvert.SerializeObject(result);
            };
        }
    }

    public class FailedAPIsMockModule : Nancy.NancyModule
    {
        internal static string BasePath = "/failed/api/v2/objects/";

        public FailedAPIsMockModule()
        {
            Post[BasePath + "owners"] = x => {
                return badRequest();
            };

            Put[BasePath + "owners"] = x => {
                return badRequest();
            };

            Put[BasePath + "owners/{username}"] = x => {
                return badRequest();
            };

            Put[BasePath + "owners/{username}/password"] = x => {
                return badRequest();
            };

            Post[BasePath + "owners/{username}/objects/{deviceId}/claim"] = x => {
                return badRequest();
            };

            Delete[BasePath + "owners/{username}"] = x => {
                return badRequest();
            };

            Post[BasePath + "owners/exists"] = x => {
                return badRequest();
            };

            Get[BasePath + "owners/exists/{username}"] = x => {
                return badRequest();
            };

            Post[BasePath + "objects"] = x => {
                return badRequest();
            };

            Put[BasePath + "objects"] = x => {
                return badRequest();
            };

            Put[BasePath + "objects/{deviceId}"] = x => {
                return badRequest();
            };

            Delete[BasePath + "objects/{deviceId}"] = x => {
                return badRequest();
            };

            Post[BasePath + "objects/exists"] = x => {
                return badRequest();
            };

            Get[BasePath + "objects/exists/{deviceId}"] = x => {
                return badRequest();
            };

            Post[BasePath + "events"] = x =>
            {
                return badRequest();
            };

            Post[BasePath + "events/exists"] = x => {
                return badRequest();
            };

            Get[BasePath + "events/exists/{id}"] = x => {
                return badRequest();
            };

            Get[BasePath + "search/datasets"] = x => {
                return badRequest();
            };

            Post[BasePath + "search/basic"] = x => {
                return badRequest();
            };
        }

        public static Response badRequest()
        {
            var response = (Response)TestUtils.ErrorMessage;
            response.StatusCode = HttpStatusCode.BadRequest;
            response.ContentType = "text/plain";
            return response;
        }
    }
}
