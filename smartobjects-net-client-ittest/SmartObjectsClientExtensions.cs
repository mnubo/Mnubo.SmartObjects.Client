using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mnubo.SmartObjects.Client.Datalake;

namespace Mnubo.SmartObjects.Client.ITTest
{
    internal static class SmartObjectsClientExtensions
    {
        internal static Func<Task<string>> WhenTryingToCreateIt(this ISmartObjectsClient client, CreateDatasetRequest createDatasetRequest)
        {
            return async () => await client.Datalake.CreateDatasetAsync(createDatasetRequest);
        }
        
        internal static Func<Task<string>> WhenTryingToIngestData(this ISmartObjectsClient client, string datasetKey, IEnumerable<IDictionary<string, object>> rows)
        {
            return async () => await client.Datalake.SendAsync(datasetKey, rows);
        }

        internal static Func<Task<Dataset>> WhenTryingToGetTheDataset(this ISmartObjectsClient client, string datasetKey)
        {
            return async () => await client.Datalake.GetDatasetAsync(datasetKey);
        }
        
        internal static Func<Task> WhenTryingToDeleteTheDataset(this ISmartObjectsClient client, string datasetKey)
        {
            return async () => await client.Datalake.DeleteDatasetAsync(datasetKey);
        }
        
        internal static Func<Task> WhenTryingToUpdateTheDataset(this ISmartObjectsClient client, string datasetKey, UpdateDatasetRequest updateDatasetRequest)
        {
            return async () => await client.Datalake.UpdateDatasetAsync(datasetKey, updateDatasetRequest);
        }
        
        internal static Func<Task<IEnumerable<Dataset>>> WhenTryingToListDatasets(this ISmartObjectsClient client)
        {
            return async () => await client.Datalake.ListDatasetsAsync();
        }
        
        internal static Func<Task> WhenTryingToAddFieldToDataset(this ISmartObjectsClient client, string datasetKey, DatasetField field)
        {
            return async () => await client.Datalake.AddFieldAsync(datasetKey, field);
        }
        
        internal static Func<Task> WhenTryingToUpdateAFieldOfDataset(this ISmartObjectsClient client, string datasetKey, string fieldKey, UpdateDatasetFieldRequest fieldRequestToUpdate)
        {
            return async () => await client.Datalake.UpdateFieldAsync(datasetKey, fieldKey, fieldRequestToUpdate);
        }
        
        internal static async Task DeleteDataset(this ISmartObjectsClient client, string datasetKey)
        {
            await client.Datalake.DeleteDatasetAsync(datasetKey);
        }
    }
}