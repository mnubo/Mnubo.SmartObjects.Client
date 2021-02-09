using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspenTech.SmartObjects.Client.Config;
using AspenTech.SmartObjects.Client.Impl;
using AspenTech.SmartObjects.Client.Models;
using Nancy.Hosting.Self;

namespace AspenTech.SmartObjects.Client.Test.Impl
{
    [TestFixture()]
    public class OwnerClientTest
    {
        private readonly ClientConfig config;
        private readonly NancyHost nancy;
        private readonly int port;

        public OwnerClientTest()
        {
            var configuration = new HostConfiguration() { UrlReservations = new UrlReservations() { CreateAutomatically = true } };
            port = TestUtils.FreeTcpPort();

            nancy = new NancyHost(configuration, new Uri(string.Format("http://localhost:{0}", port)));
            nancy.Start();
            System.Diagnostics.Debug.WriteLine(string.Format("Nancy has been started on host 'http://localhost:{0}'", port));

            config =
                new ClientConfig.Builder()
                {
                    Hostname = ClientConfig.Environments.Sandbox,
                    ConsumerKey = "key",
                    ConsumerSecret = "secret"
                };
        }

        [OneTimeTearDown]
        public void Dispose()
        {
            nancy.Stop();
        }

        #region Sync Calls
        [Test()]
        public void ClientOwnerSyncCreate()
        {
            withSuccessfulResults(client =>
            {
                client.Create(TestUtils.CreateTestOwner());
            });
        }

        [Test()]
        public void ClientOwnerSyncCreateBadRequest()
        {
            withFailedResults(client =>
            {
                 Assert.That(() => client.Create(TestUtils.CreateOwnerWrongAttribute()),
                    Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [Test()]
        public void ClientOwnerSyncCreateNullBody()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.Create(null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("owner body cannot be null."));
            });
        }

        [Test()]
        public void ClientOwnerSyncCreateNotUsername()
        {
            withSuccessfulResults(client =>
            {
                Owner owner = new Owner.Builder().Build();

            Assert.That(() => client.Create(owner),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("username cannot be blank."));
            });
        }

        [TestCase("", "username cannot be blank.", "password")]
        [TestCase(null, "username cannot be blank.", "password")]
        [TestCase("username", "password cannot be blank.", null)]
        [TestCase("username", "password cannot be blank.", "")]
        public void ClientOwnerSyncCreateUsernamePasswordValidator(string username, string errorMessage, string password)
        {
            withSuccessfulResults(client =>
            {
                Owner owner = new Owner.Builder()
            {
                Username = username,
                Password = password
            };

            Assert.That(() => client.Create(owner),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo(errorMessage));
            });
        }

        [Test()]
        public void ClientOwnerSyncUpdate()
        {
            withSuccessfulResults(client =>
            {
                client.Update(TestUtils.CreateOwnerUpdateAttribute(), TestUtils.Username);
            });
        }

        [Test()]
        public void ClientOwnerSyncUpdateBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.Update(TestUtils.CreateOwnerUpdateAttribute(), TestUtils.Username),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [Test()]
        public void ClientOwnerSyncUpdateWithBlankUsernameBadRequest()
        {

            withSuccessfulResults(client =>
            {
                Assert.That(() => client.Update(TestUtils.CreateOwnerUpdateAttribute(), ""),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("username cannot be blank."));
            });
        }

        [Test()]
        public void ClientOwnerSyncUpdateWithNullUsernameBadRequest()
        {

            withSuccessfulResults(client =>
            {
                Assert.That(() => client.Update(TestUtils.CreateOwnerUpdateAttribute(), null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("username cannot be blank."));
            });
        }

        [Test()]
        public void ClientOwnerSyncUpdateWithNullAttributesUsernameBadRequest()
        {

            withSuccessfulResults(client =>
            {
                Assert.That(() => client.Update(null, TestUtils.Username),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("owner body cannot be null."));
            });
        }

        [Test()]
        public void ClientOwnerSyncDelete()
        {
            withSuccessfulResults(client =>
            {
                client.Delete(TestUtils.Username);
            });
        }

        [Test()]
        public void ClientOwnerSyncDeleteBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.Delete(TestUtils.Username),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [Test()]
        public void ClientOwnerSyncDeleteWithBlankUsernameBadRequest()
        {

            withSuccessfulResults(client =>
            {
                Assert.That(() => client.Delete(""),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("username cannot be blank."));
            });
        }

        [Test()]
        public void ClientOwnerSyncDeleteWithNullUsernameBadRequest()
        {

            withSuccessfulResults(client =>
            {
                Assert.That(() => client.Delete(null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("username cannot be blank."));
            });
        }

        [Test()]
        public void ClientOwnerSyncClaim()
        {
            withSuccessfulResults(client =>
            {
                client.Claim(TestUtils.Username, TestUtils.DeviceId);
            });
        }

        [Test()]
        public void ClientOwnerSyncClaimBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.Claim(TestUtils.Username, TestUtils.DeviceId),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [TestCase("", "", "username cannot be blank.")]
        [TestCase(null, "", "username cannot be blank.")]
        [TestCase("username", "", "device_Id cannot be blank.")]
        [TestCase("username", null, "device_Id cannot be blank.")]
        public void ClientOwnerSyncClaimInvalid(string username, string deviceId, string errorMessage)
        {
            withSuccessfulResults(client =>
            {
                Assert.That(
                    () => client.Claim(username, deviceId),
                    Throws.TypeOf<ArgumentException>().With.Message.EqualTo(errorMessage)
                );
            });
        }

        [Test()]
        public void ClientOwnerSyncUpdatePassword()
        {
            withSuccessfulResults(client =>
            {
                client.UpdatePassword(TestUtils.Username, TestUtils.Password);
            });
        }

        [Test()]
        public void ClientOwnerSyncUpdatePasswordBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.UpdatePassword(TestUtils.Username, TestUtils.Password),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [Test()]
        public void ClientOwnerSyncUpdatePasswordEmptyUsername()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.UpdatePassword("", "newPassword"),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("username cannot be blank."));
            });
        }

        [Test()]
        public void ClientOwnerSyncUpdatePasswordNullUsername()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.UpdatePassword(null, "newPassword"),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("username cannot be blank."));
            });
        }

        [Test()]
        public void ClientOwnerSyncUpdatePasswordEmptyPassword()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.UpdatePassword("username", ""),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("password cannot be blank."));
            });
        }

        [Test()]
        public void ClientOwnerSyncUpdatePasswordNullPassword()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.UpdatePassword("username", null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("password cannot be blank."));
            });
        }

        [Test()]
        public void ClientOwnerSyncBatchCreate()
        {
            IEnumerable<Result> expected = new List<Result>()
            {
                new Result("username0",Result.ResultStates.Success,null),
                new Result("username1",Result.ResultStates.Success,null),
                new Result("username2",Result.ResultStates.Success,null),
                new Result("username3",Result.ResultStates.Success,null),
                new Result("username4",Result.ResultStates.Success,null)

            };
            withSuccessfulResults(client =>
            {
                IEnumerable<Result> actual = client.CreateUpdate(TestUtils.CreateOwners(5));
                CollectionAssert.AreEqual(expected,actual);
            });
        }

        [Test()]
        public void ClientOwnerSyncBatchCreateBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.CreateUpdate(TestUtils.CreateOwners(5)),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [Test()]
        public void ClientOwnerSyncBatchNullList()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.CreateUpdate(null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("owner body list cannot be null."));
            });
        }

        [Test()]
        public void ClientOwnerSyncBatchEmptyList()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.CreateUpdate(new List<Owner>()),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Owner body list cannot be empty or biger that 1000."));
            });
        }

        [Test()]
        public void ClientOwnerSyncBatchMaxSizeList()
        {
            withSuccessfulResults(client =>
            {
                List<Owner> owners = new List<Owner>();

            for (int i = 1; i <= 1001; i++)
            {
                owners.Add(new Owner.Builder().Build());
            }

            Assert.That(() => client.CreateUpdate(owners),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Owner body list cannot be empty or biger that 1000."));
            });
        }

        [Test()]
        public void ClientOwnerSyncBatchNullUsername()
        {
            withSuccessfulResults(client =>
            {
                List<Owner> owners = new List<Owner>();

            for (int i = 1; i <= 2; i++)
            {
                owners.Add(new Owner.Builder().Build());
            }

            Assert.That(() => client.CreateUpdate(owners),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("username cannot be blank."));
            });
        }

        [Test()]
        public void ClientOwnerSyncExists()
        {
            withSuccessfulResults(client =>
            {
                Assert.IsTrue(client.OwnerExists(TestUtils.Username));
            });
        }

        [Test()]
        public void ClientOwnerSyncExistsOwnerNotExists()
        {
            withSuccessfulAndFailedResults(client =>
            {
                Assert.IsTrue(!client.OwnerExists(TestUtils.Username));
            });
        }

        [Test()]
        public void ClientOwnerSyncExistsBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.OwnerExists(TestUtils.Username),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [Test()]
        public void ClientOwnerSyncBatchExists()
        {
            IList<string> input = new List<string>() { "username0", "username1", "username2", "username3" };
            IDictionary<string, bool> expectedResults = new Dictionary<string, bool>()
            {
                { input[0].ToString(), true },
                { input[1].ToString(), false },
                { input[2].ToString(), true },
                { input[3].ToString(), false }
            };
            withSuccessfulAndFailedResults(client =>
            {
                CollectionAssert.AreEqual(expectedResults, client.OwnersExist(input));
            });
        }

        [Test()]
        public void ClientOwnerSyncBatchExistBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.OwnersExist(new List<string>() { "username0", "username1", "username2", "username3" }),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }
        #endregion

        #region Async Calls
        [Test()]
        public void ClientOwnerAsyncCreate()
        {
            withSuccessfulResults(client =>
            {
                client.CreateAsync(TestUtils.CreateTestOwner()).Wait();
            });
        }

        [Test()]
        public void ClientOwnerAsyncCreateBadRequest()
        {
            withFailedResults(client =>
            {
                Owner owner = TestUtils.CreateOwnerWrongAttribute();

                Assert.That(() => client.CreateAsync(owner).Wait(),
                    Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [Test()]
        public void ClientOwnerAsyncCreateNullBody()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.CreateAsync(null).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("owner body cannot be null."));
            });
        }

        [Test()]
        public void ClientOwnerAsyncCreateNotUsername()
        {
            withSuccessfulResults(client =>
            {
                Owner owner = new Owner.Builder().Build();

                Assert.That(() => client.CreateAsync(owner).Wait(),
                    Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("username cannot be blank."));
            });
        }

        [TestCase("", "username cannot be blank.", "password")]
        [TestCase(null, "username cannot be blank.", "password")]
        [TestCase("username", "password cannot be blank.", null)]
        [TestCase("username", "password cannot be blank.", "")]
        public void ClientOwnerAsyncCreateUsernamePasswordValidator(string username, string errorMessage, string password)
        {
            withSuccessfulResults(client =>
            {
                Owner owner = new Owner.Builder()
                {
                    Username = username,
                    Password = password
                };

                Assert.That(() => client.CreateAsync(owner).Wait(),
                    Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo(errorMessage));
            });
        }

        [Test()]
        public void ClientOwnerAsyncUpdate()
        {
            withSuccessfulResults(client =>
            {
                client.UpdateAsync(TestUtils.CreateOwnerUpdateAttribute(), TestUtils.Username).Wait();
            });
        }

        [Test()]
        public void ClientOwnerAsyncUpdateBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.UpdateAsync(TestUtils.CreateOwnerUpdateAttribute(), TestUtils.Username).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [Test()]
        public void ClientOwnerAsyncUpdateWithBlankUsernameBadRequest()
        {

            withSuccessfulResults(client =>
            {
                Assert.That(() => client.UpdateAsync(TestUtils.CreateOwnerUpdateAttribute(), "").Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("username cannot be blank."));
            });
        }

        [Test()]
        public void ClientOwnerAsyncUpdateWithNullUsernameBadRequest()
        {

            withSuccessfulResults(client =>
            {
                Assert.That(() => client.UpdateAsync(TestUtils.CreateOwnerUpdateAttribute(), null).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("username cannot be blank."));
            });
        }

        [Test()]
        public void ClientOwnerAsyncUpdateWithNullAttributesUsernameBadRequest()
        {

            withSuccessfulResults(client =>
            {
                Assert.That(() => client.UpdateAsync(null, TestUtils.Username).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("owner body cannot be null."));
            });
        }

        [Test()]
        public void ClientOwnerAsyncDelete()
        {
            withSuccessfulResults(client =>
            {
                client.DeleteAsync(TestUtils.Username).Wait();
            });
        }

        [Test()]
        public void ClientOwnerAsyncDeleteBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.DeleteAsync(TestUtils.Username).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [Test()]
        public void ClientOwnerAsyncDeleteWithBlankUsernameBadRequest()
        {

            withSuccessfulResults(client =>
            {
                Assert.That(() => client.DeleteAsync("").Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("username cannot be blank."));
            });
        }

        [Test()]
        public void ClientOwnerAsyncDeleteWithNullUsernameBadRequest()
        {

            withSuccessfulResults(client =>
            {
                Assert.That(() => client.DeleteAsync(null).Wait(),
               Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("username cannot be blank."));
            });
        }

        [Test()]
        public void ClientOwnerAsyncClaim()
        {
            withSuccessfulResults(client =>
            {
                client.ClaimAsync(TestUtils.Username, TestUtils.DeviceId).Wait();
            });
        }

        [Test()]
        public void ClientOwnerAsyncClaimBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.ClaimAsync(TestUtils.Username, TestUtils.DeviceId).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [TestCase("", "", "username cannot be blank.")]
        [TestCase(null, "", "username cannot be blank.")]
        [TestCase("username", "", "device_Id cannot be blank.")]
        [TestCase("username", null, "device_Id cannot be blank.")]
        public void ClientOwnerAsyncClaimInvalid(string username, string deviceId, string errorMessage)
        {
            withSuccessfulResults(client =>
            {
                Assert.That(
                    () => client.ClaimAsync(username, deviceId).Wait(),
                    Throws.TypeOf<AggregateException>()
                        .With.InnerException.TypeOf<ArgumentException>()
                        .With.InnerException.Message.EqualTo(errorMessage)
                );
            });
        }

        [Test()]
        public void ClientOwnerAsyncUnclaim()
        {
            withSuccessfulResults(client =>
            {
                client.UnclaimAsync(TestUtils.Username, TestUtils.DeviceId).Wait();
            });
        }

        [Test()]
        public void ClientOwnerAsyncUnclaimBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.UnclaimAsync(TestUtils.Username, TestUtils.DeviceId).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [TestCase("", "", "username cannot be blank.")]
        [TestCase(null, "", "username cannot be blank.")]
        [TestCase("username", "", "device_Id cannot be blank.")]
        [TestCase("username", null, "device_Id cannot be blank.")]
        public void ClientOwnerAsyncUnclaimInvalid(string username, string deviceId, string errorMessage)
        {
            withSuccessfulResults(client =>
            {
                Assert.That(
                    () => client.UnclaimAsync(username, deviceId).Wait(),
                    Throws.TypeOf<AggregateException>()
                        .With.InnerException.TypeOf<ArgumentException>()
                        .With.InnerException.Message.EqualTo(errorMessage)
                );
            });
        }

        [Test()]
        public void ClientOwnerAsyncUpdatePassword()
        {
            withSuccessfulResults(client =>
            {
                client.UpdatePasswordAsync(TestUtils.Username, TestUtils.Password).Wait();
            });
        }

        [Test()]
        public void ClientOwnerAsyncUpdatePasswordBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.UpdatePasswordAsync(TestUtils.Username, TestUtils.Password).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [Test()]
        public void ClientOwnerAsyncUpdatePasswordEmptyUsername()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.UpdatePasswordAsync("", "newPassword").Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("username cannot be blank."));
            });
        }

        [Test()]
        public void ClientOwnerAsyncUpdatePasswordNullUsername()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.UpdatePasswordAsync(null, "newPassword").Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("username cannot be blank."));
            });
        }

        [Test()]
        public void ClientOwnerAsyncUpdatePasswordEmptyPassword()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.UpdatePasswordAsync("username", "").Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("password cannot be blank."));
            });
        }

        [Test()]
        public void ClientOwnerAsyncUpdatePasswordNullPassword()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.UpdatePasswordAsync("username", null).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("password cannot be blank."));
            });
        }

        [Test()]
        public void ClientOwnerAsyncBatchCreate()
        {
            IEnumerable<Result> expected = new List<Result>()
            {
                new Result("username0",Result.ResultStates.Success,null),
                new Result("username1",Result.ResultStates.Success,null),
                new Result("username2",Result.ResultStates.Success,null),
                new Result("username3",Result.ResultStates.Success,null),
                new Result("username4",Result.ResultStates.Success,null)

            };
            withSuccessfulResults(client =>
            {
                Task<IEnumerable<Result>> resultTask = client.CreateUpdateAsync(TestUtils.CreateOwners(5));
                resultTask.Wait();
                CollectionAssert.AreEqual(expected, resultTask.Result);
            });
        }

        [Test()]
        public void ClientOwnerAsyncBatchCreateBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.CreateUpdateAsync(TestUtils.CreateOwners(5)).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [Test()]
        public void ClientOwnerAsyncBatchNullList()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.CreateUpdateAsync(null).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("owner body list cannot be null."));
            });
        }

        [Test()]
        public void ClientOwnerAsyncBatchEmptyList()
        {
            withSuccessfulResults(client =>
            {
                Assert.That(() => client.CreateUpdateAsync(new List<Owner>()).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("Owner body list cannot be empty or biger that 1000."));
            });
        }

        [Test()]
        public void ClientOwnerAsyncBatchMaxSizeList()
        {
            withSuccessfulResults(client =>
            {
                List<Owner> owners = new List<Owner>();

                for (int i = 1; i <= 1001; i++)
                {
                    owners.Add(new Owner.Builder().Build());
                }

                Assert.That(() => client.CreateUpdateAsync(owners).Wait(),
                    Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("Owner body list cannot be empty or biger that 1000."));
            });
        }

        [Test()]
        public void ClientOwnerAsyncBatchNullUsername()
        {
            withSuccessfulResults(client =>
            {
                List<Owner> owners = new List<Owner>();

                for (int i = 1; i <= 2; i++)
                {
                    owners.Add(new Owner.Builder().Build());
                }

                Assert.That(() => client.CreateUpdateAsync(owners).Wait(),
                    Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo("username cannot be blank."));
            });
        }

        [Test()]
        public void ClientOwnerAsyncExists()
        {
            withSuccessfulResults(client =>
            {
                Task<bool> result = client.OwnerExistsAsync(TestUtils.Username);
                result.Wait();
                Assert.IsTrue(result.Result);
            });
        }

        [Test()]
        public void ClientOwnerAsyncExistsOwnerNotExists()
        {
            withSuccessfulAndFailedResults(client =>
            {
                Task<bool> result = client.OwnerExistsAsync(TestUtils.Username);
                result.Wait();
                Assert.IsTrue(!result.Result);
            });
        }

        [Test()]
        public void ClientOwnerAsyncExistsBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.OwnerExistsAsync(TestUtils.Username).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }

        [Test()]
        public void ClientOwnerAsyncBatchExists()
        {
            IList<string> input = new List<string>() { "username0", "username1", "username2", "username3" };
            IDictionary<string, bool> expectedResults = new Dictionary<string, bool>()
            {
                { input[0].ToString(), true },
                { input[1].ToString(), false },
                { input[2].ToString(), true },
                { input[3].ToString(), false }
            };
            withSuccessfulAndFailedResults(client =>
            {
                Task<IDictionary<string, bool>> results = client.OwnersExistAsync(input);
                results.Wait();
                CollectionAssert.AreEqual(expectedResults, results.Result);
            });
        }

        [Test()]
        public void ClientOwnerAsyncBatchExistBadRequest()
        {
            withFailedResults(client =>
            {
                Assert.That(() => client.OwnersExistAsync(new List<string>() { "username0", "username1", "username2", "username3" }).Wait(),
                Throws.TypeOf<AggregateException>()
                    .With.InnerException.TypeOf<ArgumentException>()
                    .With.InnerException.Message.EqualTo(TestUtils.ErrorMessage));
            });
        }
        #endregion

        #region Utils
        private void AssertIfAreDifferent(Owner ownerA, Owner ownerB)
        {
            Assert.AreEqual(ownerA.Username, ownerB.Username);
            Assert.AreEqual(ownerA.RegistrationDate.Value.ToString(EventSerializerTest.DatetimeFormat),
                ownerB.RegistrationDate.Value.ToString(EventSerializerTest.DatetimeFormat));
            CollectionAssert.AreEqual(ownerA.Attributes, ownerB.Attributes);
        }

        //Spawn a client which respond successfully to all events
        internal void withSuccessfulResults(Action<IOwnerClient> test)
        {
            var httpClient = new HttpClient(config, "http", "localhost", port, "/succeed");
            test(new OwnerClient(httpClient));
        }

        //Spawn a client which fail all request
        internal void withFailedResults(Action<IOwnerClient> test)
        {
            var httpClient = new HttpClient(config, "http", "localhost", port, "/failed");
            test(new OwnerClient(httpClient));
        }

        //Spawn a client which fail respond with a mix of failed and successful events
        internal void withSuccessfulAndFailedResults(Action<IOwnerClient> test)
        {
            var httpClient = new HttpClient(config, "http", "localhost", port, "/batch");
            test(new OwnerClient(httpClient));
        }
        #endregion
    }
}
