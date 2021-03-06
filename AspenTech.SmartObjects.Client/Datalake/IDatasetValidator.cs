using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace AspenTech.SmartObjects.Client.Datalake
{
    internal interface IDatasetValidator
    {
        void ValidateDatasetKey(string datasetKey);

        void ValidateFieldKey(string fieldKey);

        void ValidateField(DatasetField field);

        void ValidateCreateDataset(CreateDatasetRequest createDatasetRequest);

        void ValidateIngestionRows(IEnumerable<IDictionary<string, object>> rows);
    }

    internal class DatasetValidator : IDatasetValidator
    {
        private readonly IEnumerable<string> _unAllowedPrefixes = new List<string>
        {
            "x_", "p_", "sa_", "da_", "ada_", "owner", "object", "event", "session", "parametrizeddatasets", "scoring", "_suggested", "analyzed"
        };
        
        public void ValidateDatasetKey(string datasetKey)
        {
            if (string.IsNullOrEmpty(datasetKey))
            {
                throw new ArgumentException("datasetKey cannot be null or empty", datasetKey);
            }

            if (datasetKey.Length > 64)
            {
                throw new ArgumentException("datasetKey cannot be longer than 64 characters", datasetKey);
            }

            if (!Regex.IsMatch(datasetKey, "^[a-zA-Z0-9_-]+$"))
            {
                throw new ArgumentException(@"datasetKey can only contain a-z, A-Z, 0-9, _ and -", datasetKey);
            }

            if (this._unAllowedPrefixes.Any(x => datasetKey.StartsWith(x, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ArgumentException("datasetKey cannot start with a reserved value", datasetKey);
            }
        }

        public void ValidateFieldKey(string fieldKey)
        {
            if (string.IsNullOrEmpty(fieldKey))
            {
                throw new ArgumentException("fieldKey cannot be null or empty", fieldKey);
            }
            
            if (fieldKey.Length > 64)
            {
                throw new ArgumentException("fieldKey cannot be longer than 64 characters", fieldKey);
            }
            
            if (!Regex.IsMatch(fieldKey, @"^[a-zA-Z0-9_]+$"))
            {
                throw new ArgumentException(@"fieldKey can only contain a-z, A-Z, 0-9 and _", fieldKey);
            }

            if (fieldKey.StartsWith("x_"))
            {
                throw new ArgumentException("fieldKey cannot start with x_", fieldKey);
            }
        }

        public void ValidateField(DatasetField field)
        {
            this.ValidateFieldKey(field.Key);

            if (field.DisplayName.Length > 255)
            {
                throw new ArgumentException("displayName cannot be longer than 255 characters", field.DisplayName);
            }
            
            if (field.Description.Length > 1024)
            {
                throw new ArgumentException("description cannot be longer than 1024 characters", field.Description);
            }
        }
        
        public void ValidateCreateDataset(CreateDatasetRequest createDatasetRequest)
        {
            this.ValidateDatasetKey(createDatasetRequest.DatasetKey);

            if (createDatasetRequest.DisplayName.Length > 255)
            {
                throw new ArgumentException("displayName cannot be longer than 255 characters", createDatasetRequest.DisplayName);
            }
            
            if (createDatasetRequest.Description.Length > 512)
            {
                throw new ArgumentException("description cannot be longer than 512 characters", createDatasetRequest.Description);
            }

            foreach (var field in createDatasetRequest.Fields)
            {
                this.ValidateField(field);
            }
        }

        public void ValidateIngestionRows(IEnumerable<IDictionary<string, object>> rows)
        {
            if (rows.Count() > 5000)
            {
                throw new ArgumentException("rows count cannot be greater than 5000", nameof(rows));
            }
        }
    }
}