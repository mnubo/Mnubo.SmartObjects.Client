using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AspenTech.SmartObjects.Client.Impl;

namespace AspenTech.SmartObjects.Client.Datalake
{
    public interface IDatalakeClient
    {
        Task<string> CreateDatasetAsync(CreateDatasetRequest createDatasetRequest);

        Task<Dataset> GetDatasetAsync(string datasetKey);

        Task DeleteDatasetAsync(string datasetKey);

        Task UpdateDatasetAsync(string datasetKey, UpdateDatasetRequest updateDatasetRequest);

        Task<IEnumerable<Dataset>> ListDatasetsAsync();

        Task AddFieldAsync(string datasetKey, DatasetField field);

        Task UpdateFieldAsync(string datasetKey, string fieldKey, UpdateDatasetFieldRequest fieldRequestToUpdate);
        
        Task<string> SendAsync(string datasetKey, IEnumerable<IDictionary<string, object>> rows);
    }

    internal class DatalakeClient : IDatalakeClient
    {
        private readonly IHttpClient _client;
        private readonly JsonSerializerOptions _serializerSettings;
        private readonly IDatasetValidator _datasetValidator;

        public DatalakeClient(IHttpClient client, IDatasetValidator validator)
        {
            this._client = client;
            this._datasetValidator = validator;
            
            this._serializerSettings = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new JsonStringEnumMemberConverter()
                }
            };
        }

        /// <summary>
        /// Allows to create a dataset
        /// </summary>
        /// <param name="createDatasetRequest">The dataset to create</param>
        /// <returns>The error if there is one</returns>
        public async Task<string> CreateDatasetAsync(CreateDatasetRequest createDatasetRequest)
        {
            this._datasetValidator.ValidateCreateDataset(createDatasetRequest);

            return await this._client.SendAsyncRequestWithResult(
                HttpMethod.Post,
                "api/v1/definition/streaming/datasets",
                JsonSerializer.Serialize(createDatasetRequest, this._serializerSettings));
        }

        /// <summary>
        /// Gets the dataset
        /// </summary>
        /// <param name="datasetKey">The dataset key</param>
        /// <returns>The dataset</returns>
        public async Task<Dataset> GetDatasetAsync(string datasetKey)
        {
            this._datasetValidator.ValidateDatasetKey(datasetKey);

            var result = await this._client.SendAsyncRequestWithResult(
                HttpMethod.Get,
                $"api/v1/definition/streaming/datasets/{datasetKey}");

            return JsonSerializer.Deserialize<Dataset>(result, this._serializerSettings);
        }

        /// <summary>
        /// Allows to delete the dataset
        /// </summary>
        /// <param name="datasetKey">The dataset key</param>
        /// <returns>The error if there is one</returns>
        public async Task DeleteDatasetAsync(string datasetKey)
        {
            this._datasetValidator.ValidateDatasetKey(datasetKey);
            
            await this._client.SendAsyncRequest(
                HttpMethod.Delete,
                $"api/v1/definition/streaming/datasets/{datasetKey}");
        }

        /// <summary>
        /// Allows to update the dataset
        /// </summary>
        /// <param name="datasetKey">The dataset key</param>
        /// <param name="updateDatasetRequest">The values to update it with</param>
        /// <returns>The error if there is one</returns>
        public async Task UpdateDatasetAsync(string datasetKey, UpdateDatasetRequest updateDatasetRequest)
        {
            this._datasetValidator.ValidateDatasetKey(datasetKey);
            
            await this._client.SendAsyncRequest(
                HttpMethod.Put,
                $"api/v1/definition/streaming/datasets/{datasetKey}",
                JsonSerializer.Serialize(updateDatasetRequest, this._serializerSettings));
        }

        /// <summary>
        /// Lists all datasets
        /// </summary>
        /// <returns>The list of datasets</returns>
        public async Task<IEnumerable<Dataset>> ListDatasetsAsync()
        {
            var result = await this._client.SendAsyncRequestWithResult(
                HttpMethod.Get,
                $"api/v1/definition/streaming/datasets");

            return JsonSerializer.Deserialize<IEnumerable<Dataset>>(result, this._serializerSettings);
        }

        /// <summary>
        /// Appends a new field to an existing dataset
        /// </summary>
        /// <param name="datasetKey">The dataset key</param>
        /// <param name="field">The field to append</param>
        /// <returns>The error if there is one</returns>
        public async Task AddFieldAsync(string datasetKey, DatasetField field)
        {
            this._datasetValidator.ValidateDatasetKey(datasetKey);
            this._datasetValidator.ValidateField(field);

            await this._client.SendAsyncRequest(
                HttpMethod.Post,
                $"api/v1/definition/streaming/datasets/{datasetKey}/fields",
                JsonSerializer.Serialize(field, this._serializerSettings));
        }

        /// <summary>
        /// Updates an existing dataset field
        /// </summary>
        /// <param name="datasetKey">The dataset key</param>
        /// <param name="fieldKey">The field key</param>
        /// <param name="fieldRequestToUpdate">The field data to update with</param>
        /// <returns>The error if there is one</returns>
        public async Task UpdateFieldAsync(string datasetKey, string fieldKey, UpdateDatasetFieldRequest fieldRequestToUpdate)
        {
            this._datasetValidator.ValidateDatasetKey(datasetKey);
            this._datasetValidator.ValidateFieldKey(fieldKey);
            
            await this._client.SendAsyncRequest(
                HttpMethod.Put,
                $"api/v1/definition/streaming/datasets/{datasetKey}/fields/{fieldKey}",
                JsonSerializer.Serialize(fieldRequestToUpdate, this._serializerSettings));
        }
        
        /// <summary>
        /// Allows to ingest data into the dataset. The rows are represented by a list of dictionaries.
        ///
        /// The dictionaries key value pairs are the fields defined into the dataset
        /// </summary>
        /// <param name="datasetKey">The dataset key</param>
        /// <param name="rows">The rows of data</param>
        /// <returns>The error if there is one</returns>
        public async Task<string> SendAsync(string datasetKey, IEnumerable<IDictionary<string, object>> rows)
        {
            this._datasetValidator.ValidateDatasetKey(datasetKey);
            this._datasetValidator.ValidateIngestionRows(rows);
            
            return await this._client.SendAsyncRequestWithResult(
                HttpMethod.Post, 
                $"/api/v3/ingestion/datasets/{datasetKey}", 
                JsonSerializer.Serialize(rows, this._serializerSettings));
        }
    }

    public class UpdateDatasetRequest
    {
        public UpdateDatasetRequest()
        {
            this.Metadata = new Dictionary<string, string>();
        }
        
        public string Description { get; set; }
        
        public string DisplayName { get; set; }
        
        public IDictionary<string, string> Metadata { get; set; }
    }

    public class CreateDatasetRequest
    {
        public CreateDatasetRequest()
        {
            this.Fields = new List<DatasetField>();
            this.Metadata = new Dictionary<string, string>();
        }
        
        public string DatasetKey { get; set; }
        
        public string Description { get; set; }
        
        public string DisplayName { get; set; }
        
        public IEnumerable<DatasetField> Fields { get; set; }
        
        public IDictionary<string, string> Metadata { get; set; }
    }
    
    public class Dataset
    {
        public Dataset()
        {
            this.Fields = new List<DatasetField>();
            this.Metadata = new Dictionary<string, string>();
        }
        
        public string DatasetKey { get; set; }
        
        public string Description { get; set; }
        
        public string DisplayName { get; set; }
        
        public long Generation { get; set; }
        
        public long Version { get; set; }
        
        public IEnumerable<DatasetField> Fields { get; set; }
        
        public DatasetChanges Changes { get; set; }
        
        public IDictionary<string, string> Metadata { get; set; }
    }

    public class DatasetField
    {
        public string Key { get; set; }

        public string DisplayName { get; set; }
        
        public string Description { get; set; }
        
        public FieldType Type { get; set; }
    }

    public class UpdateDatasetFieldRequest
    {
        public string Description { get; set; }
        
        public string DisplayName { get; set; }
    }

    public class FieldType
    {
        public FieldHighLevelType HighLevelType { get; set; }
    }

    public enum FieldHighLevelType
    {
        [EnumMember(Value = "BOOLEAN")]
        Boolean,
        
        [EnumMember(Value = "INT")]
        Int,
        
        [EnumMember(Value = "LONG")]
        Long,
        
        [EnumMember(Value = "FLOAT")]
        Float,
        
        [EnumMember(Value = "DOUBLE")]
        Double,
        
        [EnumMember(Value = "TEXT")]
        Text,

        [EnumMember(Value = "TIME")]
        Time,
        
        [EnumMember(Value = "DATETIME")]
        DateTime,
        
        [EnumMember(Value = "VOLUME")]
        Volume,
        
        [EnumMember(Value = "ACCELERATION")]    
        Acceleration,
        
        [EnumMember(Value = "SPEED")]
        Speed,
        
        [EnumMember(Value = "STATE")]
        State,
        
        [EnumMember(Value = "MASS")]
        Mass,
        
        [EnumMember(Value = "EMAIL")]
        Email,
        
        [EnumMember(Value = "TEMPERATURE")]
        Temperature,
        
        [EnumMember(Value = "AREA")]
        Area,
        
        [EnumMember(Value = "LENGTH")]
        Length,
        
        [EnumMember(Value = "COUNTRYISO")]
        CountryIso,
        
        [EnumMember(Value = "SUBDIVISION_1_ISO")]
        Subdivision1Iso,
        
        [EnumMember(Value = "SUBDIVISION_2_ISO")]
        Subdivision2Iso,

        [EnumMember(Value = "TIME_ZONE")]
        TimeZone,
        
        [EnumMember(Value = "DURATION")]
        Duration
    }

    public class DatasetChanges
    {
        public string CreatedBy { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public string ModifiedBy { get; set; }
        
        public DateTime ModifiedAt { get; set; }
    }
}