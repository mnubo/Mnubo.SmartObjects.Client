using NUnit.Framework;
using System;
using System.Collections.Generic;
using AspenTech.SmartObjects.Client.Models.Search;
using AspenTech.SmartObjects.Client.Test.Impl;

namespace AspenTech.SmartObjects.Client.Test.Model
{
    [TestFixture()]
    public class ResultSetTest
    {
        private readonly ResultSet result;
        private readonly DateTime datetime;
        private readonly Guid guid;

        public ResultSetTest()
        {
            datetime = TestUtils.GetNowIgnoringMilis();
            guid = Guid.NewGuid();

            List<ColumnDefinition> columns = new List<ColumnDefinition>();
            columns.Add(new ColumnDefinition("string", "text"));
            columns.Add(new ColumnDefinition("int", "int"));
            columns.Add(new ColumnDefinition("double", "float"));
            columns.Add(new ColumnDefinition("float", "float"));
            columns.Add(new ColumnDefinition("boolean", "boolean"));
            columns.Add(new ColumnDefinition("datetime", "datetime"));
            columns.Add(new ColumnDefinition("guid", "text"));
            columns.Add(new ColumnDefinition("null", "text"));
            columns.Add(new ColumnDefinition("wrongTypeguid", "text"));

            List<Row> rows = new List<Row>();
            List<object> values = new List<object>();
            values.Add("string");
            values.Add(20);
            values.Add(3D);
            values.Add(3.5F);
            values.Add(true);
            values.Add(datetime);
            values.Add(guid);
            values.Add(null);
            values.Add("423432.3423");
            rows.Add(new Row(values));

            result = new ResultSet(columns, rows);
        }

        [Test()]
        public void CheckRightTypes()
        {
            Assert.AreEqual("string", result.GetString(0, "string"));
            Assert.AreEqual(20, result.GetInt(0, "int"));
            Assert.AreEqual(3D, result.GetDouble(0, "double"));
            Assert.AreEqual(3.5F, result.GetFloat(0, "float"));
            Assert.AreEqual(true, result.GetBoolean(0, "boolean"));
            Assert.AreEqual(datetime, result.GetDateTime(0, "datetime"));
            Assert.AreEqual(guid, result.GetGuid(0, "guid"));
            Assert.IsTrue(result.IsNull(0, "null"));
        }

        [Test()]
        public void CheckWrongGuidTypeException()
        {
            Assert.That(() => result.GetGuid(0, "wrongTypeguid"),
                Throws.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo("Column name: wrongTypeguid is not a Guid value."));
        }

        [Test()]
        public void CheckColumnNameNotFoundException()
        {
            Assert.That(() => result.GetDateTime(0, "Unknown"),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Column name: Unknown not found."));
        }

        [Test()]
        public void CheckRowIndexOutOfRangeException()
        {
            Assert.That(() => result.GetDateTime(10, "Unknown"),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Row index out of range."));
        }

        [Test()]
        public void CheckColumnNullException()
        {
            Assert.That(() => new ResultSet(null, new List<Row>()),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("columns cannot be null."));
        }

        [Test()]
        public void CheckRowNullException()
        {
            Assert.That(() => new ResultSet(new List<ColumnDefinition>(), null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("rows cannot be null."));
        }
    }
}
