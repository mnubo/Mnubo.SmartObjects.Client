using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mnubo.SmartObjects.Client.Datalake;
using NUnit.Framework;
using Polly;

namespace Mnubo.SmartObjects.Client.Tests.Common
{
    internal static class DatasetTestsExtensions
    {
        public static async Task EventuallyAssert(this DatasetTests tests, Func<Task> assert)
        {
            await Policy
                .Handle<AssertionException>()
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                .ExecuteAsync(async () =>
                {
                    await assert();
                });
        }
        
        internal static CreateDatasetRequest GivenCreateDataset(this DatasetTests tests)
        {
            return new()
            {
                DatasetKey = $"netit-{Guid.NewGuid():n}",
                Description = "Dataset created by the IT tests of the smartobjects .net sdk",
                DisplayName = "Rainbow Dash",
                Fields = new List<DatasetField>
                {
                    new()
                    {
                        Key = "rainbowdash",
                        Aliases = new List<string>
                        {
                            "apple",
                            "jack"
                        },
                        DisplayName = "The rainbow dash field",
                        Description = "Yolo description",
                        Type = new FieldType
                        {
                            HighLevelType = FieldHighLevelType.Text
                        }
                    }
                }
            };
        }

        internal static Dataset GivenDataset(this DatasetTests tests)
        {
            return new()
            {
                DatasetKey = $"netit-{Guid.NewGuid():n}",
                Description = "Dataset created by the IT tests of the smartobjects .net sdk",
                DisplayName = "Rainbow Dash",
                Fields = new List<DatasetField>
                {
                    new()
                    {
                        Key = "rainbowdash",
                        Aliases = new List<string>
                        {
                            "apple",
                            "jack"
                        },
                        DisplayName = "The rainbow dash field",
                        Description = "Yolo description",
                        Type = new FieldType
                        {
                            HighLevelType = FieldHighLevelType.Acceleration
                        }
                    }
                }
            };
        }

        internal static UpdateDatasetRequest GivenUpdateDataset(this DatasetTests tests)
        {
            return new()
            {
                Description = "Updated",
                DisplayName = "Updated"
            };
        }

        internal static UpdateDatasetFieldRequest GivenUpdateField(this DatasetTests tests)
        {
            return new()
            {
                Description = "Updated",
                DisplayName = "Updated"
            };
        }
    }
}