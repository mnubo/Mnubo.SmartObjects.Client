using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AspenTech.SmartObjects.Client.Datalake;
using AspenTech.SmartObjects.Client.Impl;
using AspenTech.SmartObjects.Client.Tests.Common;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace AspenTech.SmartObjects.Client.Test.Datalake
{
    [TestFixture]
    public class DatalakeClientTests : DatasetTests
    {
        private Mock<IHttpClient> _httpClientMock;
        private Mock<IDatasetValidator> _datasetValidator;
        private IDatalakeClient _datalakeClient;

        [SetUp]
        public void SetUp()
        {
            this._httpClientMock = new Mock<IHttpClient>();
            this._datasetValidator = new Mock<IDatasetValidator>();
            this._datalakeClient = new DatalakeClient(this._httpClientMock.Object, this._datasetValidator.Object);
        }

        [Test]
        public async Task WhenICreateADataset_ThenIValidateIt()
        {
            var createDataset = this.GivenCreateDataset();

            await this._datalakeClient.CreateDatasetAsync(createDataset);
            
            this._datasetValidator.Verify(x => x.ValidateCreateDataset(It.IsAny<CreateDatasetRequest>()), Times.Once);
        }

        [Test]
        public async Task WhenIGetDataset_ThenIValidateTheKey()
        {
            this._httpClientMock
                .Setup(x => x.SendAsyncRequestWithResult(It.IsAny<HttpMethod>(), It.IsAny<string>()))
                .ReturnsAsync(JsonConvert.SerializeObject(this.GivenDataset()));
            
            await this._datalakeClient.GetDatasetAsync("some_key");
            
            this._datasetValidator.Verify(x => x.ValidateDatasetKey(It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public async Task WhenIDeleteDataset_ThenIValidateTheKey()
        {
            await this._datalakeClient.DeleteDatasetAsync("some_key");
            
            this._datasetValidator.Verify(x => x.ValidateDatasetKey(It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public async Task WhenIUpdateDataset_ThenIValidateTheKey()
        {
            await this._datalakeClient.UpdateDatasetAsync("some_key", new UpdateDatasetRequest());
            
            this._datasetValidator.Verify(x => x.ValidateDatasetKey(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task WhenIAddField_ThenIValidateTheDatasetKeyAndField()
        {
            await this._datalakeClient.AddFieldAsync("some_key", new DatasetField());
            
            this._datasetValidator.Verify(x => x.ValidateDatasetKey(It.IsAny<string>()), Times.Once);
            this._datasetValidator.Verify(x => x.ValidateField(It.IsAny<DatasetField>()), Times.Once);
        }
        
        [Test]
        public async Task WhenIUpdateField_ThenIValidateTheDatasetKeyAndField()
        {
            await this._datalakeClient.UpdateFieldAsync("some_key", "some-other-key", new UpdateDatasetFieldRequest());
            
            this._datasetValidator.Verify(x => x.ValidateDatasetKey(It.IsAny<string>()), Times.Once);
            this._datasetValidator.Verify(x => x.ValidateFieldKey(It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public async Task WhenIIngestData_ThenIValidateTheDatasetKeyAndRows()
        {
            var createDataset = this.GivenCreateDataset();

            await this._datalakeClient.SendAsync(createDataset.DatasetKey, new List<IDictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "rainbowdash", "some-value" }
                }
                
            });
            
            this._datasetValidator.Verify(x => x.ValidateDatasetKey(It.IsAny<string>()), Times.Once);
            this._datasetValidator.Verify(x => x.ValidateIngestionRows(It.IsAny<IEnumerable<IDictionary<string, object>>>()), Times.Once);
        }
    }
}