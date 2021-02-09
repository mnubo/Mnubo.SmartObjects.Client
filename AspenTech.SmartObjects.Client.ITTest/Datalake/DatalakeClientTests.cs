using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspenTech.SmartObjects.Client.Datalake;
using AspenTech.SmartObjects.Client.Tests.Common;
using FluentAssertions;
using NUnit.Framework;

namespace AspenTech.SmartObjects.Client.ITTest.Datalake
{
    [TestFixture]
    public class DatalakeClientTests : DatasetTests
    {
        private readonly ISmartObjectsClient _client;

        public DatalakeClientTests()
        {
            this._client = ITTestHelper.newClient();
        }
        
        [Test]
        public async Task GivenADataset_WhenIToDeleteIt_ThenItIsDeleted()
        {
            var createDataset = this.GivenCreateDataset();
            
            await this._client
                .WhenTryingToCreateIt(createDataset)
                .Should()
                .NotThrowAsync();

            await this._client
                .WhenTryingToDeleteTheDataset(createDataset.DatasetKey)
                .Should()
                .NotThrowAsync();
        }
        
        [Test]
        public async Task GivenAValidDataset_WhenICreateIt_ThenIShouldGet200()
        {
            var createDataset = this.GivenCreateDataset();

            await this._client
                .WhenTryingToCreateIt(createDataset)
                .Should()
                .NotThrowAsync();

            await this._client.DeleteDataset(createDataset.DatasetKey);
        }
        
        [Test]
        public async Task GivenADataset_WhenIAddFields_ThenIGetA200()
        {
            var createDataset = this.GivenCreateDataset();

            await this._client
                .WhenTryingToCreateIt(createDataset)
                .Should()
                .NotThrowAsync();

            await this._client
                .WhenTryingToAddFieldToDataset(createDataset.DatasetKey, new DatasetField
                {
                    Aliases = new List<string> { "yolo" },
                    Description = "new field",
                    DisplayName = "new display name",
                    Key = "potato",
                    Type = new FieldType
                    {
                        HighLevelType = FieldHighLevelType.Double
                    }
                })
                .Should()
                .NotThrowAsync();

            await this._client.DeleteDataset(createDataset.DatasetKey);
        }
        
        [Test]
        public async Task GivenADataset_WhenIFetchIt_ThenIGetTheDataset()
        {
            var createDataset = this.GivenCreateDataset();
            
            await this._client
                .WhenTryingToCreateIt(createDataset)
                .Should()
                .NotThrowAsync();

            await this._client
                .WhenTryingToGetTheDataset(createDataset.DatasetKey)
                .ThenIGetTheCorrectDataset(createDataset.DatasetKey);
            
            await this._client.DeleteDataset(createDataset.DatasetKey);
        }
        
        [Test]
        public async Task GivenDatasets_WhenIFetchThem_ThenIGetDatasets()
        {
            var createDataset = this.GivenCreateDataset();

            await this._client
                .WhenTryingToCreateIt(createDataset)
                .Should()
                .NotThrowAsync();

            await this._client
                .WhenTryingToListDatasets()
                .ThenIGetAtLeastTheOneICreated(createDataset.DatasetKey);

            await this._client.DeleteDataset(createDataset.DatasetKey);
        }
        
        [Test]
        public async Task GivenADataset_WhenIUpdateIt_ThenItIsUpdated()
        {
            var createDataset = this.GivenCreateDataset();
            var updateDataset = this.GivenUpdateDataset();
            
            await this._client
                .WhenTryingToCreateIt(createDataset)
                .Should()
                .NotThrowAsync();

            await this._client
                .WhenTryingToUpdateTheDataset(createDataset.DatasetKey, updateDataset)
                .Should()
                .NotThrowAsync();

            await this._client.DeleteDataset(createDataset.DatasetKey);
        }
        
        [Test]
        public async Task GivenADataset_WhenIUpdateAField_ThenIGet200()
        {
            var createDataset = this.GivenCreateDataset();
            var fieldToUpdate = this.GivenUpdateField();
            var fieldKey = createDataset.Fields.First().Key;
            
            await this._client
                .WhenTryingToCreateIt(createDataset)
                .Should()
                .NotThrowAsync();

            await this._client
                .WhenTryingToUpdateAFieldOfDataset(createDataset.DatasetKey, fieldKey, fieldToUpdate)
                .Should()
                .NotThrowAsync();
            
            await this._client.DeleteDataset(createDataset.DatasetKey);
        }
        
        [Test]
        public async Task GivenADataset_WhenIIngestData_ThenIGet200()
        {
            var createDataset = this.GivenCreateDataset();

            await this._client
                .WhenTryingToCreateIt(createDataset) 
                .Should()
                .NotThrowAsync();
            
            await this.EventuallyAssert(async () =>
            {
                await this._client
                    .WhenTryingToIngestData(createDataset.DatasetKey, new List<IDictionary<string, object>>
                    {
                        new Dictionary<string, object>
                        {
                            { "rainbowdash", "some-value" }
                        }
                
                    })
                    .Should()
                    .NotThrowAsync();
            });
            
            await this._client.DeleteDataset(createDataset.DatasetKey);
        }
    }

    internal static class DatalakeClientTestsExtensions
    {
        internal static async Task ThenIGetTheCorrectDataset(this Func<Task<Dataset>> dataSetFunc, string dataSetKey)
        {
            var dataSet = await dataSetFunc();

            dataSet.Should().NotBeNull();
            dataSet.DatasetKey.Should().Be(dataSetKey);
        }
        
        internal static async Task ThenIGetAtLeastTheOneICreated(this Func<Task<IEnumerable<Dataset>>> datasetsFunc, string datasetKey)
        {
            var datasets = await datasetsFunc();

            datasets.Should().NotBeNull();
            datasets.Should().NotBeEmpty();
            datasets.Count().Should().BeGreaterOrEqualTo(1);

            var dataset = datasets.FirstOrDefault(x => x.DatasetKey.Equals(datasetKey));
            dataset.Should().NotBeNull();
        }
    }
}