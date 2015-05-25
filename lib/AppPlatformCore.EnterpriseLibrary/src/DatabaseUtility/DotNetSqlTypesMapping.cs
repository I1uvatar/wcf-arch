using System;
using System.Collections.Generic;
using System.Data;

namespace AppPlatform.Core.EnterpriseLibrary.DatabaseUtility
{
    /// <summary>
    /// Class containing .Net type -> DbType mapping
    /// </summary>
    public class DotNetSqlTypesMapping
    {
        #region Private variables

        static Dictionary<Type, DbType> mappingDictionary;

        #endregion

        #region Constructor

        static DotNetSqlTypesMapping()
        {
            FillMapping();
        }

        #endregion


        #region methods

        /// <summary>
        /// Retrive DbType for specified .Net type
        /// </summary>
        /// <param name="netDataType">.Net data type</param>
        /// <returns>correspondent DbType</returns>
        public static DbType GetDbType(Type netDataType)
        {
            DbType dbType;
            mappingDictionary.TryGetValue(netDataType, out dbType);
            return dbType;
        }

        #endregion


        #region Private methods

        private static void FillMapping()
        {
            mappingDictionary = new Dictionary<Type, DbType>();
            mappingDictionary.Add(typeof(string), DbType.String);
            mappingDictionary.Add(typeof(byte[]), DbType.Binary);
            mappingDictionary.Add(typeof(bool), DbType.Boolean);
            mappingDictionary.Add(typeof(bool?), DbType.Boolean);
            mappingDictionary.Add(typeof(byte?), DbType.Byte);
            mappingDictionary.Add(typeof(byte), DbType.Byte);
            mappingDictionary.Add(typeof(decimal?), DbType.Decimal);
            mappingDictionary.Add(typeof(decimal), DbType.Decimal);
            mappingDictionary.Add(typeof(DateTime?), DbType.DateTime);
            mappingDictionary.Add(typeof(DateTime), DbType.DateTime);
            mappingDictionary.Add(typeof(double?), DbType.Double);
            mappingDictionary.Add(typeof(double), DbType.Double);
            mappingDictionary.Add(typeof(Guid), DbType.Guid);
            mappingDictionary.Add(typeof(short?), DbType.Int16);
            mappingDictionary.Add(typeof(short), DbType.Int16);
            mappingDictionary.Add(typeof(int), DbType.Int32);
            mappingDictionary.Add(typeof(int?), DbType.Int32);
            mappingDictionary.Add(typeof(long), DbType.Int64);
            mappingDictionary.Add(typeof(long?), DbType.Int64);
            mappingDictionary.Add(typeof(object), DbType.Object);
            mappingDictionary.Add(typeof(sbyte?), DbType.SByte);
            mappingDictionary.Add(typeof(sbyte), DbType.SByte);
            mappingDictionary.Add(typeof(float?), DbType.Single);
            mappingDictionary.Add(typeof(float), DbType.Single);
            mappingDictionary.Add(typeof(TimeSpan?), DbType.Time);
            mappingDictionary.Add(typeof(TimeSpan), DbType.Time);
            mappingDictionary.Add(typeof(ushort?), DbType.UInt16);
            mappingDictionary.Add(typeof(ushort), DbType.UInt16);
            mappingDictionary.Add(typeof(uint), DbType.UInt32);
            mappingDictionary.Add(typeof(uint?), DbType.UInt32);
            mappingDictionary.Add(typeof(ulong), DbType.UInt64);
        }

        #endregion

    }
}