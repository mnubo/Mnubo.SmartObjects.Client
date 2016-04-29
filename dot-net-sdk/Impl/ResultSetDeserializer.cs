using Mnubo.SmartObjects.Client.Models.Search;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mnubo.SmartObjects.Client.Impl
{
    internal class ResultSetDeserializer
    {
        private const string LabelProperty = "label";
        private const string TypeProperty = "type";

        internal static ResultSet DeserializeResultSet(string obj)
        {
            RawResultSet rawResultSet = JsonConvert.DeserializeObject<RawResultSet>(obj,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatString = EventSerializer.DatetimeFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });

            IList<ColumnDefinition> columns = new List<ColumnDefinition>();
            foreach(IDictionary<string, string> column in rawResultSet.Columns)
            {
                columns.Add(new ColumnDefinition(column[LabelProperty], column[TypeProperty]));
            }

            IList<Row> rows = new List<Row>();
            foreach (IList<object> row in rawResultSet.Rows)
            {
                rows.Add(new Row(row));
            }

            return new ResultSet(columns, rows);
        }

        public class RawResultSet
        {
            public IList<IDictionary<string, string>> Columns { get; }
            public IList<IList<object>> Rows { get; }

            public RawResultSet(IList<IDictionary<string, string>> columns, IList<IList<object>> rows)
            {
                Columns = columns;
                Rows = rows;
            }
        }
    }
}
