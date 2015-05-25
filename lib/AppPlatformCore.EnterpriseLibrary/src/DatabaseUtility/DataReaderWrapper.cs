using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Xml.Linq;


namespace AppPlatform.Core.EnterpriseLibrary.DatabaseUtility
{
    /// <summary>
    /// Wrapper for IDataReader
    /// </summary>
    public class DataReaderWrapper : IDataRead
    {
        /// <summary>
        /// Missing DataReader columns handler
        /// </summary>
        public interface IHandleMissingColumn
        {
            /// <summary>
            /// Return a missing value or throw exception.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="columnName">Missing column name</param>
            /// <returns></returns>
            T  HandleMissingObjectValue<T>(string columnName) where T : class;

            /// <summary>
            /// Return a missing value or throw exception.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="columnName">Missing column name</param>
            /// <returns></returns>
            T? HandleMissingNullableValue<T>(string columnName) where T : struct;

            /// <summary>
            /// Return a missing value or throw exception.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="columnName">Missing column name</param>
            /// <returns></returns>
            T  HandleMissingValue<T>(string columnName) where T : struct;
        }

        #region Private members

        private readonly IDataReader aReader;
        private readonly Dictionary<string, int> columnMap;
        private readonly IHandleMissingColumn missingHandler;

        #endregion

        private class DefaultMissingColumnHandler : IHandleMissingColumn
        {
            /// <summary>
            /// Return a missing value or throw exception.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="columnName">Missing column name</param>
            /// <returns></returns>
            public T HandleMissingObjectValue<T>(string columnName) where T : class
            {
                return null;
            }

            /// <summary>
            /// Return a missing value or throw exception.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="columnName">Missing column name</param>
            /// <returns></returns>
            public T? HandleMissingNullableValue<T>(string columnName) where T : struct
            {
                return null;
            }

            /// <summary>
            /// Return a missing value or throw exception.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="columnName">Missing column name</param>
            /// <returns></returns>
            public T HandleMissingValue<T>(string columnName) where T : struct
            {
                return default(T);
            }
        }

        #region Public methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reader">Underlaying datareader to wrap</param>
        public DataReaderWrapper(IDataReader reader) : this (reader, new DefaultMissingColumnHandler())
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="aReader">Underlaying datareader to wrap</param>
        /// <param name="missingHandler">Strategy for handling missing columns</param>
        public DataReaderWrapper(IDataReader aReader, IHandleMissingColumn missingHandler)
        {
            this.aReader = aReader;
            this.missingHandler = missingHandler;
            this.columnMap = new Dictionary<string, int>();

            for (var i = 0; i < aReader.FieldCount; i++)
            {
                this.columnMap[aReader.GetName(i)] = i;
            }
        }

        /// <summary>
        /// Retrieve object value from datareader in specified column
        /// </summary>
        /// <typeparam name="T">Type of the object to retrieve</typeparam>
        /// <param name="columnName">Name of the column containing data</param>
        /// <returns>Value from the column</returns>
        public T GetObjectValue<T>(string columnName) where T : class
        {
            int columnIndex = this.GetOrdinalIndex(this.aReader, columnName);

            if (columnIndex < 0)
            {
                return this.missingHandler.HandleMissingObjectValue<T>(columnName);
            }

            object data = this.aReader[columnIndex];
            if (this.IsNull(data))
            {
                return null;
            }

            return  data is T ? (T)data : (T)Convert.ChangeType(data, typeof (T));
        }

        /// <summary>
        /// Retrieve object nullable value from datareader in specified column
        /// </summary>
        /// <typeparam name="T">Type of the object to retrieve</typeparam>
        /// <param name="columnName">Name of the column containing data</param>
        /// <returns>Value from the column</returns>
        public T? GetNullableValue<T>(string columnName) where T : struct
        {
            int columnIndex = this.GetOrdinalIndex(this.aReader, columnName);

            if (columnIndex < 0)
            {
                return this.missingHandler.HandleMissingNullableValue<T>(columnName);
            }

            object data = this.aReader[columnIndex];

            if (this.IsNull(data))
            {
                return null;
            }

            return data is T ? (T)data : (T)Convert.ChangeType(data, typeof(T));
        }

        /// <summary>
        /// Retrieve value from datareader in specified column
        /// </summary>
        /// <typeparam name="T">Type of the object to retrieve</typeparam>
        /// <param name="columnName">Name of the column containing data</param>
        /// <returns>Value from the column</returns>
        public T GetValue<T>(string columnName) where T : struct
        {
            int columnIndex = this.GetOrdinalIndex(this.aReader, columnName);

            if (columnIndex < 0)
            {
                return this.missingHandler.HandleMissingValue<T>(columnName);
            }

            object data = this.aReader[columnIndex];

            if (this.IsNull(data))
            {
                throw new NullReferenceException(String.Format("Column [{0}] contains NULL", columnName));
            }

            return data is T ? (T)data : (T)Convert.ChangeType(data, typeof(T));
        }

        [Obsolete("Use GetXValue()")]
        public XmlElement GetXmlValue(string columnName)
        {
            int columnIndex = this.GetOrdinalIndex(this.aReader, columnName);

            if (columnIndex < 0)
            {
                return this.missingHandler.HandleMissingObjectValue<XmlElement>(columnName);
            }

            object data = this.aReader[columnIndex];
            if (this.IsNull(data))
            {
                return null;
            }

            var element = new XmlDocument();

            element.LoadXml(Convert.ToString(data));

            return element.DocumentElement;
        }


        public XElement GetXValue(string columnName)
        {
            int columnIndex = this.GetOrdinalIndex(this.aReader, columnName);

            if (columnIndex < 0)
            {
                return this.missingHandler.HandleMissingObjectValue<XElement>(columnName);
            }

            if(this.aReader is SqlDataReader)
            {
                var xmlData = ((SqlDataReader) this.aReader).GetSqlXml(columnIndex);
                if (xmlData.IsNull)
                    return null;

                using (var reader = xmlData.CreateReader())
                {
                    return !xmlData.IsNull ? XElement.Load(reader) : null;
                }
            }

            var data = this.aReader[columnIndex];
            if (this.IsNull(data))
            {
                return null;
            }

            var element = new XmlDocument();
            element.LoadXml(Convert.ToString(data));

            return XElement.Load(new XmlNodeReader(element.DocumentElement));
        }

        public bool HasColumn(string columnName)
        {
            return (this.GetOrdinalIndex(this.aReader, columnName) >= 0);
        }

        #endregion

        #region Private members

        private int GetOrdinalIndex(IDataReader pendingReader, string columnName)
        {
            if (columnName == null)
            {
                throw new NullReferenceException("Parameter columnName is NULL");
            }
            if (this.columnMap.ContainsKey(columnName))
            {
                return this.columnMap[columnName];
            }
            else if (this.columnMap.ContainsKey(columnName.ToUpper()))
            {
                return this.columnMap[columnName.ToUpper()];
            }
            return -1;
        }

        private bool IsNull(object data)
        {
            if (data == null || data == DBNull.Value)
            {
                return true;
            }

            return false;
        }

        
        #endregion

    }
}