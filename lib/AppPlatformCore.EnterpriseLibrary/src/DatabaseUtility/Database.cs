using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using AppPlatform.Core.EnterpriseLibrary.Collections;
using AppPlatform.Core.EnterpriseLibrary.Unity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.SqlClient;
using Microsoft.Practices.Unity;

namespace AppPlatform.Core.EnterpriseLibrary.DatabaseUtility
{
    /// <summary>
    /// Wrapper around Microsoft.Practices.EnterpriseLibrary.Data.Database
    /// </summary>
    [Serializable]
    public class Database : IDatabase
    {
        public class NullContext : IDbSessionContext
        {
            public void SetupParameters(DbCommand aCommand, IList<string> aParameterList) { /* Do nothing */ return; }
        }

        // Define database client type
        public enum DbClientType {SqlServer, OracleServer, Auto};

        private readonly Microsoft.Practices.EnterpriseLibrary.Data.Database aDatabase;
        private readonly string defaultSchema;
        private readonly DbClientType dbClientType;
        private readonly IDbSessionContext dbContextProvider;
        private static readonly Dictionary<string, string> defaultSchemaList = new Dictionary<string, string>();
        private static readonly ThreadSafeDictionary<string, IList<string>> parameterListCache = new ThreadSafeDictionary<string, IList<string>>();
        private static ISQLExceptionProcessor sqlExceptionProcessor;

        /// <summary>
        /// Constructor
        /// Use for Test Only !!!
        /// Use factory method instead !!!
        /// </summary>
        /// <param name="aDatabase"></param>
        /// <param name="aSchemaName"></param>
        /// <param name="dbClientType"></param>
        /// <param name="aDbContextProvider"></param>
        public Database(Microsoft.Practices.EnterpriseLibrary.Data.Database aDatabase, string aSchemaName, DbClientType dbClientType, IDbSessionContext aDbContextProvider)
        {
            this.aDatabase = aDatabase;
            this.defaultSchema = aSchemaName;
            this.dbClientType = dbClientType;
            this.dbContextProvider = aDbContextProvider ?? new NullContext();
        }

        /// <summary>
        /// Factory Method
        /// </summary>
        /// <param name="connectionStringKey"></param>
        /// <returns></returns>
        public static Database Create(string connectionStringKey)
        {
            var ctx = IocProvider.Container.Resolve<IDbSessionContext>();
            Microsoft.Practices.EnterpriseLibrary.Data.Database injectedDatabase = null;
            try
            {
                injectedDatabase = IocProvider.Container.Resolve<Microsoft.Practices.EnterpriseLibrary.Data.Database>();
            }
            catch (ResolutionFailedException) { }
            return Database.Create(injectedDatabase, DbClientType.Auto, connectionStringKey, ctx);
        }


        /// <summary>
        /// Factory Method
        /// </summary>
        /// <param name="aDatabase"></param>
        /// <param name="connectionStringKey"></param>
        /// <param name="dbClientType"></param>
        /// <param name="aDbContextProvider"></param>
        /// <returns></returns>
        public static Database Create(
            Microsoft.Practices.EnterpriseLibrary.Data.Database aDatabase, DbClientType dbClientType,
            string connectionStringKey, IDbSessionContext aDbContextProvider
            )
        {
            aDatabase = aDatabase ?? DatabaseFactory.CreateDatabase(connectionStringKey);
            string schemaName;
            defaultSchemaList.TryGetValue(connectionStringKey, out schemaName);

            switch (dbClientType)
            {
                case DbClientType.Auto:
                    if (aDatabase is Microsoft.Practices.EnterpriseLibrary.Data.Sql.SqlDatabase)
                    {
                        return new Database(aDatabase, schemaName, DbClientType.SqlServer, aDbContextProvider);
                    }
                    return new Database(aDatabase, schemaName, DbClientType.OracleServer, aDbContextProvider);

                default:
                    return new Database(aDatabase, schemaName, dbClientType, aDbContextProvider);
            }
        }


        /// <summary>
        /// Declares a default schema name for a given DB connection
        /// </summary>
        /// <param name="connectionStringKey"></param>
        /// <param name="defaultPrefix"></param>
        public static void ConfigureSchema(string connectionStringKey, string defaultPrefix)
        {
            defaultSchemaList[connectionStringKey] = defaultPrefix;
        }

        /// <summary>
        ///Creates DB to get client type 
        /// </summary>
        /// <param name="connectionStringKey"></param>
        /// <returns></returns>
        public static DbClientType GetClientTypeFromConnectionString(string connectionStringKey)
        {
            var db = DatabaseFactory.CreateDatabase(connectionStringKey);
            if (db is Microsoft.Practices.EnterpriseLibrary.Data.Oracle.OracleDatabase)
            {
                return DbClientType.OracleServer;
            }
            else
            {
                return DbClientType.SqlServer;
            }
        }


        public static void ConfigureSqlExceptionProcessor(ISQLExceptionProcessor sqlExcProcessor)
        {
            sqlExceptionProcessor = sqlExcProcessor;
        }

        private delegate long? Execute();

        private void DoExecuteAndLog(Execute doExecute, DbCommand aCommand, IDatabaseActivityLogger aLogger)
        {
            var sw = new Stopwatch();
            var startTime = DateTime.UtcNow;
            Exception exception = null;

            long? recordsAffected = null;
            try
            {
                if (aLogger != null)
                {
                    aLogger.LogPreExecute(aCommand);
                }
                sw.Start();
                startTime = DateTime.UtcNow;
                recordsAffected = doExecute();
            }
            catch (Exception ex)
            {
                //if (aLogger != null)
                //{
                //    aLogger.LogException(aCommand, ex);
                //}
                if (sqlExceptionProcessor != null)
                {
                    if(ex is SqlException)
                    {
                        sqlExceptionProcessor.Process((SqlException)ex, aCommand);
                    }
                }

                exception = ex;
                throw;
            }
            finally
            {
                sw.Stop();
                if (aLogger != null)
                {
                    aLogger.LogPostExecute(aCommand, startTime, sw.ElapsedMilliseconds, recordsAffected, exception);
                }
            }
        }

        /// <summary>
        /// <para>
        /// Executes the <paramref name="command" /> and returns the number of rows affected.
        /// </para>
        /// </summary>
        /// <param name="command"><para>The command that contains the query to execute.</para></param>
        /// <seealso cref="M:System.Data.IDbCommand.ExecuteScalar" />
        public virtual int ExecuteNonQuery(DbCommand command)
        {
            return this.ExecuteNonQuery(command, new NullLogger());
        }

        /// <summary>
        /// Executes a non query command query
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <param name="aLogger"></param>
        /// <returns>Number of affected records</returns>
        public int ExecuteNonQuery(DbCommand command, IDatabaseActivityLogger aLogger)
        {
            int res = 0;

            this.DoExecuteAndLog(() => { res = this.aDatabase.ExecuteNonQuery(command); return res; }, command, aLogger);

            return res;
        }

        /// <summary>
        /// <para>
        /// Executes the <paramref name="command" /> and returns an <see cref="T:System.Data.IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.
        /// </para>
        /// </summary>
        /// <param name="command"><para>The command that contains the query to execute.</para></param>
        /// <returns>
        /// <para>
        /// An <see cref="T:System.Data.IDataReader" /> object.
        /// </para>
        /// </returns>
        public virtual IDataReader ExecuteReader(DbCommand command)
        {
            return this.ExecuteReader(command, new NullLogger());
        }

        /// <summary>
        /// Executes a query and returns a data reader
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <param name="aLogger"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(DbCommand command, IDatabaseActivityLogger aLogger)
        {
            IDataReader res = null;

            this.DoExecuteAndLog(() => { res = this.aDatabase.ExecuteReader(command); return null; }, command, aLogger);

            return res;
        }

        /// <summary>
        /// <para>
        /// Executes the <paramref name="command" /> and adds a new <see cref="T:System.Data.DataTable"></see> to the existing <see cref="T:System.Data.DataSet"></see>.
        /// </para>
        /// </summary>
        /// <param name="command"><para>The <see cref="T:System.Data.Common.DbCommand" /> to execute.</para></param>
        /// <param name="dataSet"><para>The <see cref="T:System.Data.DataSet" /> to load.</para></param>
        /// <param name="tableName"><para>The name for the new <see cref="T:System.Data.DataTable" /> to add to the <see cref="T:System.Data.DataSet" />.</para></param>
        /// <exception cref="T:System.ArgumentNullException">Any input parameter was <see langword="null" /> (<b>Nothing</b> in Visual Basic)</exception>
        /// <exception cref="T:System.ArgumentException">tableName was an empty string</exception>
        public virtual void LoadDataSet(DbCommand command, DataSet dataSet, string tableName)
        {
            this.LoadDataSet(command, dataSet, tableName, new NullLogger());
        }

        /// <summary>
        /// <para>
        /// Executes the <paramref name="command" /> and adds a new <see cref="T:System.Data.DataTable"></see> to the existing <see cref="T:System.Data.DataSet"></see>.
        /// </para>
        /// </summary>
        /// <param name="command"><para>The <see cref="T:System.Data.Common.DbCommand" /> to execute.</para></param>
        /// <param name="dataSet"><para>The <see cref="T:System.Data.DataSet" /> to load.</para></param>
        /// <param name="tableName"><para>The name for the new <see cref="T:System.Data.DataTable" /> to add to the <see cref="T:System.Data.DataSet" />.</para></param>
        /// <param name="aLogger"></param>
        /// <exception cref="T:System.ArgumentNullException">Any input parameter was <see langword="null" /> (<b>Nothing</b> in Visual Basic)</exception>
        /// <exception cref="T:System.ArgumentException">tableName was an empty string</exception>
        public void LoadDataSet(DbCommand command, DataSet dataSet, string tableName, IDatabaseActivityLogger aLogger)
        {
            this.DoExecuteAndLog(() => { this.aDatabase.LoadDataSet(command, dataSet, tableName); return null; }, command, aLogger);
        }

        /// <summary>
        /// <para>
        /// Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="T:System.Data.DataSet" /> within a transaction.
        /// </para>
        /// </summary>
        /// <param name="dataSet"><para>The <see cref="T:System.Data.DataSet" /> used to update the data source.</para></param>
        /// <param name="tableName"><para>The name of the source table to use for table mapping.</para></param>
        /// <param name="insertCommand"><para>The <see cref="T:System.Data.Common.DbCommand" /> executed when <see cref="T:System.Data.DataRowState" /> is <seealso cref="F:System.Data.DataRowState.Added" />.</para></param>
        /// <param name="updateCommand"><para>The <see cref="T:System.Data.Common.DbCommand" /> executed when <see cref="T:System.Data.DataRowState" /> is <seealso cref="F:System.Data.DataRowState.Modified" />.</para></param>
        /// <param name="deleteCommand"><para>The <see cref="T:System.Data.Common.DbCommand" /> executed when <see cref="T:System.Data.DataRowState" /> is <seealso cref="F:System.Data.DataRowState.Deleted" />.</para></param>        
        /// <param name="behavior"></param>
        /// <returns>
        /// Number of records affected.
        /// </returns>
        public int UpdateDataSet(DataSet dataSet, string tableName, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand, UpdateBehavior behavior)
        {
            return this.UpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand, behavior, new NullLogger());
        }

        /// <summary>
        /// <para>
        /// Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="T:System.Data.DataSet" /> within a transaction.
        /// </para>
        /// </summary>
        /// <param name="dataSet"><para>The <see cref="T:System.Data.DataSet" /> used to update the data source.</para></param>
        /// <param name="tableName"><para>The name of the source table to use for table mapping.</para></param>
        /// <param name="insertCommand"><para>The <see cref="T:System.Data.Common.DbCommand" /> executed when <see cref="T:System.Data.DataRowState" /> is <seealso cref="F:System.Data.DataRowState.Added" />.</para></param>
        /// <param name="updateCommand"><para>The <see cref="T:System.Data.Common.DbCommand" /> executed when <see cref="T:System.Data.DataRowState" /> is <seealso cref="F:System.Data.DataRowState.Modified" />.</para></param>
        /// <param name="deleteCommand"><para>The <see cref="T:System.Data.Common.DbCommand" /> executed when <see cref="T:System.Data.DataRowState" /> is <seealso cref="F:System.Data.DataRowState.Deleted" />.</para></param>        
        /// <param name="behavior"></param>
        /// <param name="aLogger"></param>
        /// <returns>
        /// Number of records affected.
        /// </returns>
        public int UpdateDataSet(DataSet dataSet, string tableName, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand, UpdateBehavior behavior, IDatabaseActivityLogger aLogger)
        {
            var records = 0;
            this.DoExecuteAndLog(() => records = this.aDatabase.UpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand, behavior), insertCommand, aLogger);
            return records;
        }

        /// <summary>
        /// <para>
        /// Executes the <paramref name="command" /> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.
        /// </para>
        /// </summary>
        /// <param name="command"><para>The command that contains the query to execute.</para></param>
        /// <returns>
        /// <para>
        /// The first column of the first row in the result set.
        /// </para>
        /// </returns>
        /// <seealso cref="M:System.Data.IDbCommand.ExecuteScalar" />
        public virtual object ExecuteScalar(DbCommand command)
        {
            return this.ExecuteScalar(command, new NullLogger());
        }

        /// <summary>
        /// Executes a query and returns a single value
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <param name="aLogger"></param>
        /// <returns></returns>
        public object ExecuteScalar(DbCommand command, IDatabaseActivityLogger aLogger)
        {
            object res = null;

            this.DoExecuteAndLog(() => { res = this.aDatabase.ExecuteScalar(command); return null; }, command, aLogger);

            return res;
        }

        /// <summary>
        /// <para> Creates a <see cref="T:System.Data.Common.DbCommand" /> for a stored procedure.</para>
        /// </summary>
        /// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
        /// <returns>
        /// <para> The <see cref="T:System.Data.Common.DbCommand" /> for the stored procedure. </para>
        /// </returns>
        public virtual DbCommand GetStoredProcCommand(string storedProcedureName)
        {
            if (!String.IsNullOrEmpty(this.defaultSchema) && !storedProcedureName.StartsWith(this.defaultSchema))
            {
                storedProcedureName = String.Concat(this.defaultSchema, ".", storedProcedureName);
            }

            var command = this.aDatabase.GetStoredProcCommand(storedProcedureName);
            var parameterList = this.GetProcedureParameters(storedProcedureName);
            this.dbContextProvider.SetupParameters(command, parameterList);
            return command;
        }

        private IList<string> GetProcedureParameters(string procedureName)
        {
            if (parameterListCache.ContainsKey(procedureName))
            {
                return parameterListCache[procedureName];
            }

            var parameterListQuery = @"
SELECT PARAMETER_NAME
FROM
    information_schema.PARAMETERS
WHERE 
    SPECIFIC_SCHEMA = @SchemaName
    AND SPECIFIC_NAME = @ProcedureName
";

            var parametersQuery = this.aDatabase.GetSqlStringCommand(parameterListQuery);
            var build = new SqlCommandParameterBuilder(parametersQuery);
            var name = procedureName.Split('.');
            build.Add("SchemaName", name[0]);
            build.Add("ProcedureName", name[1]);

            var parameters = new List<string>();
            using (var read = this.aDatabase.ExecuteReader(parametersQuery))
            {
                while (read.Read())
                {
                    parameters.Add(read.GetString(0));
                }
            }

            parameterListCache.MergeSafe(procedureName, parameters);

            return parameters;
        }

        /// <summary>
        /// <para> Creates a <see cref="T:System.Data.Common.DbCommand" /> for a SQL query. </para>
        /// </summary>
        /// <param name="query"><para>The text of the query.</para></param>
        /// <returns>
        /// <para> The <see cref="T:System.Data.Common.DbCommand" /> for the SQL query. </para>
        /// </returns>
        public DbCommand GetSqlStringCommand(string query)
        {
            var command = this.aDatabase.GetSqlStringCommand(query);
            // For now, the text SQL commands are not supported.
            // this.dbContextProvider.SetupParameters(command, null);
            return command;
        }

        /// <summary>
        /// <para>
        /// Gets the string used to open a database.
        /// </para>
        /// </summary>
        /// <value>
        /// <para>
        /// The string used to open a database.
        /// </para>
        /// </value>
        /// <seealso cref="P:System.Data.Common.DbConnection.ConnectionString" />
        public virtual string ConnectionString
        {
            get { return this.aDatabase.ConnectionString; }
        }

        /// <summary>
        /// <para>
        /// Gets database client type
        /// </para>
        /// </summary>
        /// <seealso cref="P:System.Data.Common.DbConnection.ConnectionString" />
        public virtual Database.DbClientType DatabaseClientType
        {
            get { return this.dbClientType;  }
        }

        /// <summary>
        /// <para>
        /// Creates a connection for this database.
        /// </para>
        /// </summary>
        /// <returns>
        /// <para>
        /// The <see cref="T:System.Data.Common.DbConnection" /> for this database.
        /// </para>
        /// </returns>
        /// <seealso cref="T:System.Data.Common.DbConnection" />
        public virtual DbConnection CreateConnection()
        {
            return this.aDatabase.CreateConnection();
        }
    }
}
