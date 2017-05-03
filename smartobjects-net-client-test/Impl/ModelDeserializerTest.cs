using System;
using System.Collections.Generic;
using NUnit.Framework;
using Mnubo.SmartObjects.Client.Models.DataModel;
using Mnubo.SmartObjects.Client.Impl;

namespace Con.Mnubo.Dotnetsdktest.Test.Impl
{
    [TestFixture()]
    public class ModelDeserializerTest
    {
        [Test()]
        public void TestDeserialize()
        {
            string json = @"
            {
                ""objectTypes"": [
                    {
                    ""key"": ""object_type1"",
                    ""description"": ""desc"",
                    ""objectAttributes"": [
                        {
                        ""key"": ""object_text_attribute"",
                        ""displayName"": ""dp object_text_attribute"",
                        ""description"": ""desc object_text_attribute"",
                        ""type"": {
                            ""highLevelType"": ""TEXT"",
                            ""containerType"": ""none""
                        }
                        },
                        {
                        ""key"": ""object_int_attribute"",
                        ""displayName"": ""dp object_int_attribute"",
                        ""description"": ""desc object_int_attribute"",
                        ""type"": {
                            ""highLevelType"": ""INT"",
                            ""containerType"": ""list""
                        }
                        }
                    ]
                    }
                ],
                ""eventTypes"": [
                    {
                    ""key"": ""event_type1"",
                    ""description"": ""desc"",
                    ""origin"": ""scheduled"",
                    ""timeseries"": [
                        {
                        ""key"": ""ts_number_attribute"",
                        ""displayName"": ""dp ts_number_attribute"",
                        ""description"": ""desc ts_number_attribute"",
                        ""type"": {
                            ""highLevelType"": ""DOUBLE""
                        }
                        },
                        {
                        ""key"": ""ts_text_attribute"",
                        ""displayName"": ""dp ts_text_attribute"",
                        ""description"": ""desc ts_text_attribute"",
                        ""type"": {
                            ""highLevelType"": ""TEXT""
                        }
                        }
                    ]
                    },
                    {
                    ""key"": ""event_type2"",
                    ""description"": ""desc"",
                    ""origin"": ""rule"",
                    ""timeseries"": [
                        {
                        ""key"": ""ts_text_attribute"",
                        ""displayName"": ""dp ts_text_attribute"",
                        ""description"": ""desc ts_text_attribute"",
                        ""type"": {
                            ""highLevelType"": ""TEXT""
                        }
                        }
                    ]
                    }
                ],
                ""ownerAttributes"": [
                    {
                    ""key"": ""owner_text_attribute"",
                    ""displayName"": ""dp owner_text_attribute"",
                    ""description"": ""desc owner_text_attribute"",
                    ""type"": {
                        ""highLevelType"": ""TEXT"",
                        ""containerType"": ""none""
                    }
                    }
                ],
                ""sessionizers"": [
                    {
                    ""key"": ""sessionizer"",
                    ""displayName"": ""dp sessionizer"",
                    ""description"": ""desc sessionizer"",
                    ""startEventTypeKey"": ""event_type1"",
                    ""endEventTypeKey"": ""event_type2""
                    }
                ],
                ""orphans"": {
                    ""timeseries"": [
                    {
                        ""key"": ""orphan_ts"",
                        ""displayName"": ""dp orphan_ts"",
                        ""description"": ""desc orphan_ts"",
                        ""type"": {
                        ""highLevelType"": ""ACCELERATION""
                        }
                    }
                    ],
                    ""objectAttributes"": [
                    {
                        ""key"": ""orphan_object"",
                        ""displayName"": ""dp orphan_object"",
                        ""description"": ""desc orphan_object"",
                        ""type"": {
                        ""highLevelType"": ""EMAIL"",
                        ""containerType"": ""none""
                        }
                    }
                    ]
                }
                }
            ";
            Model model = ModelDeserializer.DeserializeModel(json);
            Assert.AreEqual(2, model.EventTypes.Count, "event types count");
            Assert.AreEqual(2, model.Timeseries.Count, "timeseries count");
            Assert.AreEqual(1, model.ObjectTypes.Count, "object types count");
            Assert.AreEqual(2, model.ObjectAttributes.Count, "object attributes count");
            Assert.AreEqual(1, model.OwnerAttributes.Count, "owner attributes count");
            Assert.AreEqual(1, model.Sessionizers.Count, "sessionizers count");

            EventType type1 = model.EventTypes[0];
            Assert.AreEqual("event_type1", type1.Key);
            Assert.AreEqual("desc", type1.Description);
            Assert.AreEqual("scheduled", type1.Origin);
            Assert.AreEqual(new List<string>(){"ts_number_attribute", "ts_text_attribute"}, type1.TimeseriesKeys);
            EventType type2 = model.EventTypes[1];
            Assert.AreEqual("event_type2", type2.Key);
            Assert.AreEqual("desc", type2.Description);
            Assert.AreEqual("rule", type2.Origin);
            Assert.AreEqual(new List<string>(){"ts_text_attribute"}, type2.TimeseriesKeys);
        
            ObjectType oType = model.ObjectTypes[0];
            Assert.AreEqual("object_type1", oType.Key);
            Assert.AreEqual("desc", oType.Description);
            Assert.AreEqual(new List<string>(){"object_text_attribute", "object_int_attribute"}, oType.ObjectAttributeKeys);
            
            ObjectAttribute object1 = model.ObjectAttributes[0];
            Assert.AreEqual("object_text_attribute", object1.Key);
            Assert.AreEqual("dp object_text_attribute", object1.DisplayName);
            Assert.AreEqual("desc object_text_attribute", object1.Description);
            Assert.AreEqual("TEXT", object1.HighLevelType);
            Assert.AreEqual("none", object1.ContainerType);
            Assert.AreEqual(new List<string>(){"object_type1"}, object1.ObjectTypeKeys);
            
            ObjectAttribute object2 = model.ObjectAttributes[1];
            Assert.AreEqual("object_int_attribute", object2.Key);
            Assert.AreEqual("dp object_int_attribute", object2.DisplayName);
            Assert.AreEqual("desc object_int_attribute", object2.Description);
            Assert.AreEqual("INT", object2.HighLevelType);
            Assert.AreEqual("list", object2.ContainerType);
            Assert.AreEqual(new List<string>(){"object_type1"}, object2.ObjectTypeKeys);
            
            
            Timeseries ts1 = model.Timeseries[0];
            Assert.AreEqual("ts_number_attribute", ts1.Key);
            Assert.AreEqual("dp ts_number_attribute", ts1.DisplayName);
            Assert.AreEqual("desc ts_number_attribute", ts1.Description);
            Assert.AreEqual("DOUBLE", ts1.HighLevelType);
            Assert.AreEqual(new List<string>(){"event_type1"}, ts1.EventTypeKeys);
            
            Timeseries ts2 = model.Timeseries[1];
            Assert.AreEqual("ts_text_attribute", ts2.Key);
            Assert.AreEqual("dp ts_text_attribute", ts2.DisplayName);
            Assert.AreEqual("desc ts_text_attribute", ts2.Description);
            Assert.AreEqual("TEXT", ts2.HighLevelType);
            Assert.AreEqual(new List<string>(){"event_type1", "event_type2"}, ts2.EventTypeKeys);
            
            OwnerAttribute owner = model.OwnerAttributes[0];
            Assert.AreEqual("owner_text_attribute", owner.Key);
            Assert.AreEqual("dp owner_text_attribute", owner.DisplayName);
            Assert.AreEqual("desc owner_text_attribute", owner.Description);
            Assert.AreEqual("TEXT", owner.HighLevelType);
            Assert.AreEqual("none", owner.ContainerType);
            
            Sessionizer sess = model.Sessionizers[0];
            Assert.AreEqual("sessionizer", sess.Key);
            Assert.AreEqual("dp sessionizer", sess.DisplayName);
            Assert.AreEqual("desc sessionizer", sess.Description);
            Assert.AreEqual("event_type1", sess.StartEventTypeKey);
            Assert.AreEqual("event_type2", sess.EndEventTypeKey);

            ObjectAttribute orphanObj = model.Orphans.ObjectAttributes[0];
            Assert.AreEqual("orphan_object", orphanObj.Key);
            Assert.AreEqual("dp orphan_object", orphanObj.DisplayName);
            Assert.AreEqual("desc orphan_object", orphanObj.Description);
            Assert.AreEqual("EMAIL", orphanObj.HighLevelType);
            Assert.AreEqual("none", orphanObj.ContainerType);
            Assert.AreEqual(new List<string>(), orphanObj.ObjectTypeKeys);
            
            
            Timeseries orphanTs = model.Orphans.Timeseries[0];
            Assert.AreEqual("orphan_ts", orphanTs.Key);
            Assert.AreEqual("dp orphan_ts", orphanTs.DisplayName);
            Assert.AreEqual("desc orphan_ts", orphanTs.Description);
            Assert.AreEqual("ACCELERATION", orphanTs.HighLevelType);
            Assert.AreEqual(new List<string>(), orphanTs.EventTypeKeys);
        }
        
        [Test()]
        public void TestDeserializeEmpty()
        {
            string json = @"
            {
                ""objectTypes"": [],
                ""eventTypes"": [
                    {
                    ""key"": ""event_type1"",
                    ""description"": ""desc"",
                    ""origin"": ""scheduled"",
                    ""timeseries"": []
                    }
                ],
                ""ownerAttributes"": [],
                ""sessionizers"": [],
                ""orphans"": {}
                }
            ";
            Model model = ModelDeserializer.DeserializeModel(json);
            Assert.AreEqual(1, model.EventTypes.Count, "event types count");
            Assert.AreEqual(0, model.Timeseries.Count, "timeseries count");
            Assert.AreEqual(0, model.ObjectTypes.Count, "object types count");
            Assert.AreEqual(0, model.ObjectAttributes.Count, "object attributes count");
            Assert.AreEqual(0, model.OwnerAttributes.Count, "owner attributes count");
            Assert.AreEqual(0, model.Sessionizers.Count, "sessionizers count");
        }
    }
}