using Mnubo.SmartObjects.Client.Config;
using Mnubo.SmartObjects.Client.Impl;
using Mnubo.SmartObjects.Client.Models;
using Mnubo.SmartObjects.Client.Models.Search;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mnubo.SmartObjects.Client.Test.Impl
{
    [TestFixture()]
    public class RestitutionClientTest
    {
        private const string UsernameColumnName = "username";
        private const string RegistrationDateColumnName = "x_registration_date";
        private const string OwnerAttributeColumnName = TestUtils.OwnerAttributeName;
        private const string DeviceIdColumnName = "x_device_id";
        private const string ObjectAttributeColumnName = TestUtils.ObjectAttributeName;
        private const string ObjectTypeColumnName = "x_object_type";
        private const string EventObjectColumnName = "x_object.x_device_id";
        private const string TimeStampColumnName = "x_timestamp";
        private const string EventTypeColumnName = "x_event_type";
        private const string TimeSerieColumnName = TestUtils.TimeSeriesName;

        private readonly DateTime startPoint = TestUtils.GetNowIgnoringMilis();
        private readonly ClientConfig config;
        private readonly ISmartObjectsClient client;
        private readonly IRestitutionClient restiturionclient;
        private readonly Owner owner;
        private readonly SmartObject obj;
        private readonly List<Event> events;

        public RestitutionClientTest()
        {
            //configuring the SDK
            config =
                new ClientConfig.Builder()
                {
                    Environment = ClientConfig.Environments.Sandbox,
                    ConsumerKey = Config.ConsumerKey,
                    ConsumerSecret = Config.ConsumerSecret
                };
            //Creating client
            client = ClientFactory.Create(config);
            restiturionclient = client.Restitution;

            //creating owner, object and events
            owner = TestUtils.CreateOwner();
            obj = TestUtils.CreateObject();
            events = TestUtils.CreateEvents(obj.DeviceId, 2);

            //posting owners, objects and events lists
            client.Objects.Create(obj);
            client.Owners.Create(owner);
            client.Events.Post(events);

            WaitForLoading(GetSearchEventQuery(events[0].DeviceId));
            WaitForLoading(GetSearchOwnerQuery(owner.Username));
        }

        ~RestitutionClientTest()
        {
            //disposing owners and objects
            client.Owners.Delete(owner.Username);
            client.Objects.Delete(obj.DeviceId);
        }

        #region Sync
        [Test()]
        public void ClientRestitutionSyncGetDatasets()
        {
            validDatasets(restiturionclient.GetDataSets());
        }

        [Test()]
        public void ClientRestitutionSyncSearchGetOwner()
        {
            ValidOwner(client.Restitution.Search(GetSearchOwnerQuery(owner.Username)));
        }

        [Test()]
        public void ClientRestitutionSyncSearchGetObject()
        {
            ValidObject(client.Restitution.Search(GetSearchObjectQuery(obj.DeviceId)));
        }

        [Test()]
        public void ClientRestitutionSyncSearchGetEvents()
        {
            ValidEvents(client.Restitution.Search(GetSearchEventQuery(events[0].DeviceId)));
        }

        [Test()]
        public void ClientRestitutionSyncSearchGetEventsCheckingResultSet()
        {
            ResultSet resultSet = client.Restitution.Search(GetSearchEventQuery(events[0].DeviceId));
            Assert.AreEqual(events[0].DeviceId, resultSet.GetString(0, EventObjectColumnName));
            Assert.AreEqual(events[0].Timestamp, resultSet.GetDateTime(0, TimeStampColumnName));
            Assert.AreEqual(events[0].EventType, resultSet.GetString(0, EventTypeColumnName));
            Assert.AreEqual(events[0].Timeseries[TimeSerieColumnName], resultSet.GetString(0, TimeSerieColumnName));
            Assert.AreEqual(events[1].DeviceId, resultSet.GetString(1, EventObjectColumnName));
            Assert.AreEqual(events[1].Timestamp, resultSet.GetDateTime(1, TimeStampColumnName));
            Assert.AreEqual(events[1].EventType, resultSet.GetString(1, EventTypeColumnName));
            Assert.AreEqual(events[1].Timeseries[TimeSerieColumnName], resultSet.GetString(1, TimeSerieColumnName));
        }

        [Test()]
        public void ClientRestitutionSyncSearchBadRequest()
        {
            Assert.That(() => client.Restitution.SearchAsync(GetSearchBadQuery()).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("Field 'unknown' is unknown."));
        }

        [TestCase("")]
        [TestCase(null)]
        public void ClientRestitutionSyncSearchGetEventsQueryValidator(string query)
        {
            Assert.That(() => client.Restitution.SearchAsync(query).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("Query cannot be empty or null."));
        }
        #endregion

        #region Async
        [Test()]
        public void ClientRestitutionAsyncGetDatasets()
        {
            Task<IEnumerable<DataSet>> result = restiturionclient.GetDataSetsAsync();
            result.Wait();

            validDatasets(result.Result);
        }

        [Test()]
        public void ClientRestitutionAsyncSearchGetOwner()
        {
            Task<ResultSet> task = client.Restitution.SearchAsync(GetSearchOwnerQuery(owner.Username));
            task.Wait();

            ValidOwner(task.Result);
        }

        [Test()]
        public void ClientRestitutionAsyncSearchGetObject()
        {
            Task<ResultSet> task = client.Restitution.SearchAsync(GetSearchObjectQuery(obj.DeviceId));
            task.Wait();

            ValidObject(task.Result);
        }

        [Test()]
        public void ClientRestitutionAsyncSearchGetEvents()
        {
            Task<ResultSet> task = client.Restitution.SearchAsync(GetSearchEventQuery(events[0].DeviceId));
            task.Wait();

            ValidEvents(task.Result);
        }

        [Test()]
        public void ClientRestitutionAsyncSearchBadRequest()
        {
            Assert.That(() => client.Restitution.Search(GetSearchBadQuery()),
                    Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo("Field 'unknown' is unknown."));
        }

        [TestCase("")]
        [TestCase(null)]
        public void ClientRestitutionAsyncSearchGetEventsQueryValidator(string query)
        {
            Assert.That(() => client.Restitution.SearchAsync(query).Wait(),
                Throws.TypeOf<AggregateException>()
                .With.InnerException.TypeOf<ArgumentException>()
                .With.InnerException.Message.EqualTo("Query cannot be empty or null."));
        }
        #endregion

        private void validDatasets(IEnumerable<DataSet> datasets)
        {
            int count = 0;

            foreach (DataSet dataset in datasets)
            {
                switch (dataset.Key)
                {
                    case "owner":
                        {
                            Assert.AreEqual("Owners", dataset.DisplayName);
                            Assert.IsNull(dataset.Description);
                            Assert.Greater(dataset.Fields.Count, 0);
                            break;
                        }
                    case "event":
                        {
                            Assert.AreEqual("Events", dataset.DisplayName);
                            Assert.IsNull(dataset.Description);
                            Assert.Greater(dataset.Fields.Count, 0);
                            break;
                        }
                    case "object":
                        {
                            Assert.AreEqual("Objects", dataset.DisplayName);
                            Assert.IsNull(dataset.Description);
                            Assert.Greater(dataset.Fields.Count, 0);
                            break;
                        }
                    case "session":
                        {
                            Assert.AreEqual("Sessions", dataset.DisplayName);
                            Assert.IsNull(dataset.Description);
                            Assert.Greater(dataset.Fields.Count, 0);
                            break;
                        }
                    default:
                        {
                            Assert.Fail("Unknown type");
                            break;
                        }
                }
                count++;
            }
            Assert.AreEqual(4, count);
        }

        private string GetSearchBadQuery()
        {
            return
            "{" +
                "\"from\":\"owner\"," +
                "\"select\":" +
                    "[" +
                        "{\"value\":\"" + UsernameColumnName + "\"}," +
                        "{\"value\":\"" + RegistrationDateColumnName + "\"}," +
                        "{\"value\":\"" + OwnerAttributeColumnName + "\"}" +
                    "]," +
                "\"where\":" +
                    "{\"unknown\":" +
                        "{\"EQ\":\"empty\"}}" +
            "}";
        }

        private string GetSearchOwnerQuery(string username)
        {
            return 
            "{" +
                "\"from\":\"owner\"," +
                "\"select\":" +
                    "[" +
                        "{\"value\":\"" + UsernameColumnName + "\"}," +
                        "{\"value\":\"" + RegistrationDateColumnName + "\"}," +
                        "{\"value\":\"" + OwnerAttributeColumnName + "\"}" +
                    "]," +
                "\"where\":" +
                    "{\"" + UsernameColumnName + "\":" +
                        "{\"EQ\":\"" + username + "\"}}" +
            "}";
        }

        private string GetSearchObjectQuery(string deviceId)
        {
            return
            "{" +
                "\"from\":\"object\"," +
                "\"select\":" +
                    "[" +
                        "{\"value\":\"" + DeviceIdColumnName + "\"}," +
                        "{\"value\":\"" + RegistrationDateColumnName + "\"}," +
                        "{\"value\":\"" + ObjectTypeColumnName + "\"}," +
                        "{\"value\":\"" + ObjectAttributeColumnName + "\"}" +
                    "]," +
                "\"where\":" +
                    "{\"" + DeviceIdColumnName + "\":" +
                        "{\"EQ\":\"" + deviceId + "\"}}" +
            "}";
        }

        private static string GetSearchEventQuery(string deviceId)
        {
            return
            "{" +
                "\"from\":\"event\"," +
                "\"select\":" +
                    "[" +
                        "{\"value\":\"" + EventObjectColumnName + "\"}," +
                        "{\"value\":\"" + TimeStampColumnName + "\"}," +
                        "{\"value\":\"" + EventTypeColumnName + "\"}," +
                        "{\"value\":\"" + TimeSerieColumnName + "\"}" +
                    "]," +
                "\"where\":" +
                    "{" +
                        "\"and\":" +
                            "[" +
                                "{\"" + EventObjectColumnName + "\":" +
                                        "{\"EQ\":\"" + deviceId + "\"}}," +
                                "{\"" + EventTypeColumnName + "\":" +
                                        "{\"EQ\":\"" + TestUtils.EventType + "\"}}" +
                            "]" +
                    "}" +
            "}";
        }

        private void ValidOwner(ResultSet resultSet)
        {
            //valid column items in owner
            Assert.AreEqual(3, resultSet.Columns.Count);
            Assert.AreEqual(UsernameColumnName, resultSet.Columns[0].Label);
            Assert.AreEqual("text", resultSet.Columns[0].HighLevelType);
            Assert.AreEqual(RegistrationDateColumnName, resultSet.Columns[1].Label);
            Assert.AreEqual("datetime", resultSet.Columns[1].HighLevelType);
            Assert.AreEqual(OwnerAttributeColumnName, resultSet.Columns[2].Label);
            Assert.AreEqual("text", resultSet.Columns[2].HighLevelType);

            //validing row items in owner
            Assert.AreEqual(1, resultSet.Rows.Count);
            Assert.AreEqual(3, resultSet.Rows[0].Values.Count);
            Assert.AreEqual(owner.Username, resultSet.Rows[0].Values[0]);
            Assert.AreEqual(owner.RegistrationDate, resultSet.Rows[0].Values[1]);
            Assert.AreEqual(owner.Attributes[OwnerAttributeColumnName], resultSet.Rows[0].Values[2]);
        }

        private void ValidObject(ResultSet resultSet)
        {
            //valid column items in Object
            Assert.AreEqual(4, resultSet.Columns.Count);
            Assert.AreEqual(DeviceIdColumnName, resultSet.Columns[0].Label);
            Assert.AreEqual("text", resultSet.Columns[0].HighLevelType);
            Assert.AreEqual(RegistrationDateColumnName, resultSet.Columns[1].Label);
            Assert.AreEqual("datetime", resultSet.Columns[1].HighLevelType);
            Assert.AreEqual(ObjectTypeColumnName, resultSet.Columns[2].Label);
            Assert.AreEqual("text", resultSet.Columns[2].HighLevelType);
            Assert.AreEqual(ObjectAttributeColumnName, resultSet.Columns[3].Label);
            Assert.AreEqual("text", resultSet.Columns[3].HighLevelType);

            //valid row items in object
            Assert.AreEqual(1, resultSet.Rows.Count);
            Assert.AreEqual(4, resultSet.Rows[0].Values.Count);
            Assert.AreEqual(obj.DeviceId, resultSet.Rows[0].Values[0]);
            Assert.AreEqual(obj.RegistrationDate, resultSet.Rows[0].Values[1]);
            Assert.AreEqual(obj.ObjectType, resultSet.Rows[0].Values[2]);
            Assert.AreEqual(obj.Attributes[ObjectAttributeColumnName], resultSet.Rows[0].Values[3]);
        }

        private void ValidEvents(ResultSet resultSet)
        {
            //valid column items in events
            Assert.AreEqual(4, resultSet.Columns.Count);
            Assert.AreEqual(EventObjectColumnName, resultSet.Columns[0].Label);
            Assert.AreEqual("text", resultSet.Columns[0].HighLevelType);
            Assert.AreEqual(TimeStampColumnName, resultSet.Columns[1].Label);
            Assert.AreEqual("datetime", resultSet.Columns[1].HighLevelType);
            Assert.AreEqual(EventTypeColumnName, resultSet.Columns[2].Label);
            Assert.AreEqual("text", resultSet.Columns[2].HighLevelType);
            Assert.AreEqual(TimeSerieColumnName, resultSet.Columns[3].Label);
            Assert.AreEqual("text", resultSet.Columns[3].HighLevelType);

            //valid rows items in events
            Assert.AreEqual(2, resultSet.Rows.Count);
            Assert.AreEqual(4, resultSet.Rows[0].Values.Count);
            Assert.AreEqual(events[0].DeviceId, resultSet.Rows[0].Values[0]);
            Assert.AreEqual(events[0].Timestamp, resultSet.Rows[0].Values[1]);
            Assert.AreEqual(events[0].EventType, resultSet.Rows[0].Values[2]);
            Assert.AreEqual(events[0].Timeseries[TimeSerieColumnName], resultSet.Rows[0].Values[3]);
            Assert.AreEqual(4, resultSet.Rows[1].Values.Count);
            Assert.AreEqual(events[1].DeviceId, resultSet.Rows[1].Values[0]);
            Assert.AreEqual(events[1].Timestamp, resultSet.Rows[1].Values[1]);
            Assert.AreEqual(events[1].EventType, resultSet.Rows[1].Values[2]);
            Assert.AreEqual(events[1].Timeseries[TimeSerieColumnName], resultSet.Rows[1].Values[3]);
        }

        private void WaitForLoading(string query)
        {
            int counter = 15;

            //waiting to be ready to be read
            Task<ResultSet> task = null;
            int factor = 1;
            do
            {
                Thread.Sleep(1000 * factor++);
                task = client.Restitution.SearchAsync(query);
                task.Wait();
            } while (task.Result.Rows.Count == 0 && counter-- > 0);
        }
    }
}
