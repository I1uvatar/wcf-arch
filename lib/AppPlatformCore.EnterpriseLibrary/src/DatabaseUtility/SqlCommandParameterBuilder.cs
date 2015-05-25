using System;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.SqlServer.Server;
using System.Data.SqlClient;
using AppPlatform.Core.EnterpriseLibrary.Types;
using AppPlatform.Core.EnterpriseLibrary.Diagnostics;

namespace AppPlatform.Core.EnterpriseLibrary.DatabaseUtility
{
    /// <summary>
    /// Append parameter to the list of command parameters
    /// </summary>
    public class SqlCommandParameterBuilder
    {
        #region Private members

        private readonly IDbCommand command;
        private readonly Database.DbClientType dbClientType;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize object from existinf db command
        /// </summary>
        /// <param name="command">command to build</param>
        public SqlCommandParameterBuilder(IDbCommand command)
        {
            this.command = command;
            this.dbClientType = DbCommandHelper.GetCommandClientType(command);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Add parameter, string type
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>Builder object itself</returns>
        public SqlCommandParameterBuilder Add(string name, string value)
        {
            return this.Add(name, value, DbType.String);
        }

        /// <summary>
        /// Add GUID parameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlCommandParameterBuilder Add(string name, Guid value)
        {
            return this.Add(name, DbType.Guid, delegate(IDbDataParameter p) { p.Value = value; });
        }

        /// <summary>
        /// Add nullable GUID parameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlCommandParameterBuilder Add(string name, Guid? value)
        {
            return this.Add(name, DbType.Guid, delegate(IDbDataParameter p) { p.Value = this.NullableBoxing(value); });
        }

        /// <summary>
        /// Add parameter
        /// </summary>
        public SqlCommandParameterBuilder Add(string name, string value, DbType type)
        {
            return this.Add(name, type, delegate(IDbDataParameter p) { p.Value = (object)value ?? DBNull.Value; });
        }

        public SqlCommandParameterBuilder Add(string name, XElement value)
        {
            return this.Add(name, DbType.Xml, ParameterDirection.Input, delegate(IDbDataParameter p)
            {
                if (value == null)
                {
                    p.Value = DBNull.Value;
                }
                else
                {
                    var xmlReader = XmlReader.Create(new StringReader(value.ToString()));

                    p.Value = new SqlXml(xmlReader);
                }
            });
        }

        /// <summary>
        /// Add sqlxml type parameter to the wrapped command
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>Builder object itself</returns>
        public SqlCommandParameterBuilder Add(string name, XmlElement value)
        {
            return this.Add(name, DbType.Xml, ParameterDirection.Input, delegate(IDbDataParameter p)
            {
                if (value == null)
                {
                    p.Value = DBNull.Value;
                }
                else
                {
                    var xmlReader = XmlReader.Create(new StringReader(value.OuterXml));
                    p.Value = new SqlXml(xmlReader);
                }
            });
        }

        /// <summary>
        /// Add integer value parameter to the wrapped command
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>Builder object itself</returns>
        public SqlCommandParameterBuilder Add(string name, int? value)
        {
            return this.Add(name, DbType.Int32, delegate (IDbDataParameter p) { p.Value = this.NullableBoxing(value); });
        }

        /// <summary>
        /// Add boolean nullable value parameter to the wrapped command
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>Builder object itself</returns>
        public SqlCommandParameterBuilder Add(string name, bool value)
        {
            return this.Add(name, DbType.Boolean, delegate(IDbDataParameter p) { p.Value = value; });
        }

        /// <summary>
        /// Add integer value parameter to the wrapped command
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>Builder object itself</returns>
        public SqlCommandParameterBuilder Add(string name, bool? value)
        {
            return this.Add(name, DbType.Boolean, delegate(IDbDataParameter p) { p.Value = this.NullableBoxing(value); });
        }

        public SqlCommandParameterBuilder Add(string name, byte[] value)
        {
            return this.Add(name, DbType.Binary, delegate(IDbDataParameter p)
                                                     {
                                                         p.Value = this.DBNullBoxing(value);
                                                     });
        }

        /// <summary>
        /// Add nullable long value parameter to the wrapped command
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>Builder object itself</returns>
        public SqlCommandParameterBuilder Add(string name, long? value)
        {
            return this.Add(name, DbType.Int64, delegate(IDbDataParameter p) { p.Value = this.NullableBoxing(value); });
        }

        /// <summary>
        /// Add nullable long value output parameter to the wrapped command
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>Builder object itself</returns>
        public SqlCommandParameterBuilder AddOutput(string name, long? value)
        {
            return this.Add(name, DbType.Int64, ParameterDirection.Output, delegate(IDbDataParameter p) { p.Value = this.NullableBoxing(value); });
        }

        /// <summary>
        /// Adds an output parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="dbType">The parameter type.</param>
        /// <returns></returns>
        public SqlCommandParameterBuilder AddOutput(string name, DbType dbType)
        {
            return this.Add(name, dbType, ParameterDirection.Output, null);
        }

        /// <summary>
        /// Adds an output parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public SqlCommandParameterBuilder AddOutput(string name, DbType dbType, int size)
        {
            return this.Add(name, dbType, ParameterDirection.Output, null, size);
        }

        /// <summary>
        /// Add nullable datetime value output parameter to the wrapped command
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>Builder object itself</returns>
        public SqlCommandParameterBuilder AddOutput(string name, DateTime? value)
        {
            return this.Add(name, DbType.DateTime, ParameterDirection.Output, delegate(IDbDataParameter p) { p.Value = this.NullableBoxing(value); });
        }

        /// <summary>
        /// Add integer value output parameter to the wrapped command
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>Builder object itself</returns>
        public SqlCommandParameterBuilder AddInputOutput(string name, int value)
        {
            return this.Add(name, DbType.Int32, ParameterDirection.InputOutput, delegate(IDbDataParameter p) { p.Value = value; });
        }

        /// <summary>
        /// Add DateTime value output parameter to the wrapped command
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>Builder object itself</returns>
        public SqlCommandParameterBuilder AddInputOutput(string name, DateTime? value)
        {
            return this.Add(name, DbType.DateTime, ParameterDirection.InputOutput, delegate(IDbDataParameter p) { p.Value = this.NullableBoxing(value); });
        }

        /// <summary>
        /// Add long value output parameter to the wrapped command
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>Builder object itself</returns>
        public SqlCommandParameterBuilder AddInputOutput(string name, long value)
        {
            return this.Add(name, DbType.Int64, ParameterDirection.InputOutput, delegate(IDbDataParameter p) { p.Value = value; });
        }

        /// <summary>
        /// Add nullable decimal value parameter to the wrapped command
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>Builder object itself</returns>
        public SqlCommandParameterBuilder Add(string name, decimal? value)
        {
            return this.Add(name, DbType.Decimal, delegate(IDbDataParameter p) { p.Value = this.NullableBoxing(value); });
        }

        /// <summary>
        /// Add nullable datetime value parameter to the wrapped command
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>Builder object itself</returns>
        public SqlCommandParameterBuilder Add(string name, DateTime? value)
        {
            return this.Add(name, DbType.DateTime, delegate(IDbDataParameter p)
            {
                p.Value = this.NullableBoxing(value);
                p.Size = 8;
            });
        }

        /// <summary>
        /// Add nullable datetime value parameter to the wrapped command
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>Builder object itself</returns>
        public SqlCommandParameterBuilder Add(string name, TimeSpan? value)
        {
            ///////NOTE: Enterprise library factory has bug with DbType - Time, this is workaround for SqlParametar
            if (!(this.command is SqlCommand))
            {
                throw new NotImplementedException();
            }

            return this.AddSqlCommandParameter(name,
                                               SqlDbType.Time,
                                               ParameterDirection.Input, p =>
                                                                             {
                                                                                 p.Value = this.NullableBoxing(value);
                                                                             });
            //return this.Add(name, DbType.Time, delegate(IDbDataParameter p)
            //{
            //    p.Value = this.NullableBoxing(value);

            //    if(p is SqlParameter)
            //    {
            //        var x = p as SqlParameter;
            //        x.SqlDbType = SqlDbType.Time;
            //    }
            //});
        }

        /// <summary>
        /// Add nullable long list value to the wrapped command.
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="valueList">Value of the parameter</param>
        /// <returns></returns>
        public SqlCommandParameterBuilder Add(string name, List<long> valueList)
        {
            return this.Add(name, valueList, CustomTypeNames.LongListTblType.Name, CustomTypeNames.LongListTblType.Column);
        }

        /// <summary>
        /// Add nullable long list value to the wrapped command.
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="valueList">Value of the parameter</param>
        /// <param name="typeName"></param>
        /// /// <param name="columnName"></param>
        /// <returns></returns>
        public SqlCommandParameterBuilder Add(string name, List<long> valueList, string typeName, string columnName)
        {
            if (!(this.command is SqlCommand))
            {
                throw new NotImplementedException();
            }

            Assert.ArgumentIsNotNullOrEmpty(name, "[name] must not be NULL");
            Assert.ArgumentIsNotNullOrEmpty(typeName, "[typeName] must not be NULL");
            Assert.ArgumentIsNotNullOrEmpty(columnName, "[columnName] must not be NULL");

            return this.AddSqlCommandParameter(name, SqlDbType.Structured, p =>
            {
                p.Direction = ParameterDirection.Input;
                p.TypeName = typeName;
                if (!Safe.HasItems(valueList))
                {
                    p.Value = null;
                    return;
                }

                var longList = new List<SqlDataRecord>();
                SqlMetaData[] sqlMetaData = { new SqlMetaData(columnName, SqlDbType.BigInt) };

                foreach (var value in valueList)
                {
                    var rec = new SqlDataRecord(sqlMetaData);
                    rec.SetInt64(0, value);
                    longList.Add(rec);
                }

                p.Value = longList;
            });
        }

        /// <summary>
        /// Add nullable long list value to the wrapped command.
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="valueList">Value of the parameter</param>
        /// <param name="typeName"></param>
        /// /// <param name="columnName"></param>
        /// <returns></returns>
        public SqlCommandParameterBuilder Add(string name, List<string> valueList, string typeName, string columnName)
        {
            if (!(this.command is SqlCommand))
            {
                throw new NotImplementedException();
            }

            Assert.ArgumentIsNotNullOrEmpty(name, "[name] must not be NULL");
            Assert.ArgumentIsNotNullOrEmpty(typeName, "[typeName] must not be NULL");
            Assert.ArgumentIsNotNullOrEmpty(columnName, "[columnName] must not be NULL");

            return this.AddSqlCommandParameter(name, SqlDbType.Structured, p =>
            {
                p.Direction = ParameterDirection.Input;
                p.TypeName = typeName;
                if (!Safe.HasItems(valueList))
                {
                    p.Value = null;
                    return;
                }

                var stringList = new List<SqlDataRecord>();
                SqlMetaData[] sqlMetaData = { new SqlMetaData(columnName, SqlDbType.NVarChar, 50) };

                foreach (var value in valueList)
                {
                    var rec = new SqlDataRecord(sqlMetaData);
                    rec.SetString(0, value);
                    stringList.Add(rec);
                }

                p.Value = stringList;
            });
        }

        #endregion

        #region Private methods

        private delegate void Setter(IDbDataParameter aParameter);

        private SqlCommandParameterBuilder Add(string name, DbType type, Setter setValue)
        {
            return this.Add(name, type, ParameterDirection.Input, setValue);
        }

        private SqlCommandParameterBuilder Add(string name, DbType type, ParameterDirection direction, Setter setValue)
        {
            return this.Add(name, type, direction, setValue, null);
        }

        private SqlCommandParameterBuilder Add(string name, DbType type, ParameterDirection direction, Setter setValue, int? size)
        {
            var parameterName = DbCommandHelper.PrepareParamName(name, dbClientType);
            var aParameter =
                this.command.Parameters.Contains(parameterName) ?
                new { P = (IDbDataParameter)this.command.Parameters[parameterName], ToAdd = false } :
                new { P = this.command.CreateParameter(), ToAdd = true } ;
            
            aParameter.P.DbType = type;
            aParameter.P.Direction = direction;
            aParameter.P.ParameterName = parameterName;

            if (setValue != null)
            {
                setValue(aParameter.P);
            }

            if (size.HasValue)
            {
                aParameter.P.Size = size.Value;
            }

            if (aParameter.ToAdd)
            {
                this.command.Parameters.Add(aParameter.P);
            }

            return this;
        }

        #region SqlServer specific implementation

        private delegate void SqlSetter(SqlParameter aParameter);

        private SqlCommandParameterBuilder AddSqlCommandParameter(string name, SqlDbType type, SqlSetter setValue)
        {
            return this.AddSqlCommandParameter(name, type, ParameterDirection.Input, setValue);
        }

        private SqlCommandParameterBuilder AddSqlCommandParameter(string name, SqlDbType type, ParameterDirection direction, SqlSetter setValue)
        {
            return this.AddSqlCommandParameter(name, type, direction, setValue, null);
        }

        private SqlCommandParameterBuilder AddSqlCommandParameter(string name, SqlDbType type, ParameterDirection direction, SqlSetter setValue, int? size)
        {
            SqlParameter aParameter = (SqlParameter)this.command.CreateParameter();

            aParameter.ParameterName = DbCommandHelper.PrepareParamName(name, dbClientType);
            aParameter.SqlDbType = type;
            aParameter.Direction = direction;

            if (setValue != null)
            {
                setValue(aParameter);
            }

            if (size.HasValue)
            {
                aParameter.Size = size.Value;
            }

            this.command.Parameters.Add(aParameter);

            return this;
        }

        #endregion

        private object NullableBoxing<T>(Nullable<T> aValue) where T : struct
        {
            return aValue.HasValue ? (object)aValue.Value : DBNull.Value;
        }

        private object DBNullBoxing<T>(T aValue) where T : class
        {
            return (object)aValue ?? DBNull.Value;
        }

        #endregion
    }
}
