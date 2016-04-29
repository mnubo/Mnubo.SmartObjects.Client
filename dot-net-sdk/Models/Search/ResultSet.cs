using System;
using System.Collections.Generic;

namespace Mnubo.SmartObjects.Client.Models.Search
{
    public class ResultSet
    {
        /// <summary>
        /// List of columns of the result set.
        /// </summary>
        public IList<ColumnDefinition> Columns { get;  }

        /// <summary>
        /// List the wors of the result set
        /// </summary>
        public IList<Row> Rows { get; }

        /// <summary>
        /// create a new instance
        /// </summary>
        /// <param name="columns">columns</param>
        /// <param name="rows">rows</param>
        public ResultSet(IList<ColumnDefinition> columns, IList<Row> rows)
        {
            if (columns == null)
            {
                throw new ArgumentException("columns cannot be null.");
            }
            if (rows == null)
            {
                throw new ArgumentException("rows cannot be null.");
            }
            Columns = columns;
            Rows = rows;
        }

        /// <summary>
        /// Checks if a value is null according to the index of the value
        /// </summary>the
        /// <param name="i">index of the value</param>
        /// <returns>True if the value is null</returns>
        public bool IsNull(int rowIndex, int columnIndex)
        {
            validRowIndex(rowIndex);
            return Rows[rowIndex].Values[columnIndex] == null;
        }

        /// <summary>
        /// Checks if a value is null according to the name of the value
        /// </summary>
        /// <param name="i">name of the value</param>
        /// <returns>True if the value is null</returns>
        public bool IsNull(int rowIndex, String name)
        {
            validRowIndex(rowIndex);
            return Rows[rowIndex].Values[findColumnIndex(name)] == null;
        }

        /// <summary>
        /// Get a string value according to the name of the value
        /// </summary>
        /// <param name="name">name of the value to get</param>
        /// <returns>String value</returns>
        public String GetString(int rowIndex, String name)
        {
            validRowIndex(rowIndex);
            return Convert.ToString(Rows[rowIndex].Values[findColumnIndex(name)]);
        }

        /// <summary>
        /// Get a string value according to the index of the value
        /// </summary>
        /// <param name="i">index of the string value to get</param>
        /// <returns>string value</returns>
        public String GetString(int rowIndex, int columnIndex)
        {
            validRowIndex(rowIndex);
            return Convert.ToString(Rows[rowIndex].Values[columnIndex]);
        }

        /// <summary>
        /// Get an integer value according to the name of the value
        /// </summary>
        /// <param name="name">name of the value to get</param>
        /// <returns>integer value</returns>
        public int GetInt(int rowIndex, String name)
        {
            validRowIndex(rowIndex);
            return Convert.ToInt32(Rows[rowIndex].Values[findColumnIndex(name)]);
        }

        /// <summary>
        /// Get an integer value according to the index of the value
        /// </summary>
        /// <param name="i">index of the value</param>
        /// <returns>integer value</returns>
        public int GetInt(int rowIndex, int columnIndex)
        {
            validRowIndex(rowIndex);
            return Convert.ToInt32(Rows[rowIndex].Values[columnIndex]);
        }

        /// <summary>
        /// Get a long value according to the name of the value
        /// </summary>
        /// <param name="name">name of the value to get</param>
        /// <returns>long value</returns>
        public long GetLong(int rowIndex, String name)
        {
            validRowIndex(rowIndex);
            return Convert.ToInt64(Rows[rowIndex].Values[findColumnIndex(name)]);
        }

        /// <summary>
        /// Get a long value according to the index of the value
        /// </summary>
        /// <param name="i">index of the value</param>
        /// <returns>long value</returns>
        public long GetLong(int rowIndex, int columnIndex)
        {
            validRowIndex(rowIndex);
            return Convert.ToInt64(Rows[rowIndex].Values[columnIndex]);
        }

        /// <summary>
        /// Get a double value according to the name of the value
        /// </summary>
        /// <param name="name">name of the value to get</param>
        /// <returns>Double value</returns>
        public double GetDouble(int rowIndex, String name)
        {
            validRowIndex(rowIndex);
            return Convert.ToDouble(Rows[rowIndex].Values[findColumnIndex(name)]);
        }

        /// <summary>
        /// Get a double value according to the index of the value
        /// </summary>
        /// <param name="i">index of the value</param>
        /// <returns>double value</returns>
        public double GetDouble(int rowIndex, int columnIndex)
        {
            validRowIndex(rowIndex);
            return Convert.ToDouble(Rows[rowIndex].Values[columnIndex]);
        }

        /// <summary>
        /// Get a float value according to the name of the value
        /// </summary>
        /// <param name="name">name of the value to get</param>
        /// <returns>float value</returns>
        public float GetFloat(int rowIndex, String name)
        {
            validRowIndex(rowIndex);
            return Convert.ToSingle(Rows[rowIndex].Values[findColumnIndex(name)]);
        }

        /// <summary>
        /// Get a float value according to the index of the value
        /// </summary>
        /// <param name="i">index of the value</param>
        /// <returns>float value</returns>
        public float GetFloat(int rowIndex, int columnIndex)
        {
            validRowIndex(rowIndex);
            return Convert.ToSingle(Rows[rowIndex].Values[columnIndex]);
        }

        /// <summary>
        /// Get a boolean value according to the name of the value
        /// </summary>
        /// <param name="name">name of the value to get</param>
        /// <returns>boolean value</returns>
        public bool GetBoolean(int rowIndex, String name)
        {
            validRowIndex(rowIndex);
            return Convert.ToBoolean(Rows[rowIndex].Values[findColumnIndex(name)]);
        }

        /// <summary>
        /// Get a boolean value according to the index of the value
        /// </summary>
        /// <param name="i">index of the value</param>
        /// <returns>boolean value</returns>
        public bool GetBoolean(int rowIndex, int columnIndex)
        {
            validRowIndex(rowIndex);
            return Convert.ToBoolean(Rows[rowIndex].Values[columnIndex]);
        }

        /// <summary>
        /// Get a Datetime value according to the name of the value
        /// </summary>
        /// <param name="name">name of the value to get</param>
        /// <returns>datetime value</returns>
        public DateTime GetDateTime(int rowIndex, String name)
        {
            validRowIndex(rowIndex);
            return Convert.ToDateTime(Rows[rowIndex].Values[findColumnIndex(name)]);
        }

        /// <summary>
        /// Get a Datetime value according to the index of the value
        /// </summary>
        /// <param name="i">index of the value</param>
        /// <returns>Datetime value</returns>
        public DateTime GetDateTime(int rowIndex, int columnIndex)
        {
            validRowIndex(rowIndex);
            return Convert.ToDateTime(Rows[rowIndex].Values[columnIndex]);
        }

        /// <summary>
        /// Get a GUID value according to the name of the value
        /// </summary>
        /// <param name="name">name of the value to get</param>
        /// <returns>Guid value</returns>
        public Guid GetGuid(int rowIndex, String name)
        {
            validRowIndex(rowIndex);
            return GetGuid(Rows[rowIndex], findColumnIndex(name));
        }

        /// <summary>
        /// Get a Guid value according to the index of the value
        /// </summary>
        /// <param name="i">index of the value</param>
        /// <returns>Guid value</returns>
        public Guid GetGuid(int rowIndex, int columnIndex)
        {
            validRowIndex(rowIndex);
            return GetGuid(Rows[rowIndex], columnIndex);
        }

        private void validRowIndex(int rowIndex)
        {
            if(rowIndex >= Rows.Count)
            {
                throw new ArgumentException("Row index out of range.");
            }
        }

        private int findColumnIndex(string name)
        {
            foreach (ColumnDefinition column in Columns)
            {
                if (column.Label == name)
                {
                    return Columns.IndexOf(column);
                }
            }
            throw new ArgumentException(string.Format("Column name: {0} not found.", name));
        }

        private Guid GetGuid(Row row, int columnIndex)
        {
            Guid result;
            if (Guid.TryParse(Convert.ToString(row.Values[columnIndex]), out result))
            {
                return result;
            }
            throw new InvalidOperationException(
                string.Format("Column name: {0} is not a Guid value.", Columns[columnIndex].Label));
        }
    }
}
