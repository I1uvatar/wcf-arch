using System;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace AppPlatform.Core.EnterpriseLibrary.DatabaseUtility
{
    /// <summary>
    /// Wrapper around MS Data Block Database
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// Creates a DbCommand of type "StoredProcedure"
        /// </summary>
        /// <param name="name">Stored procedure name</param>
        /// <returns></returns>
        DbCommand GetStoredProcCommand(string name);

        /// <summary>
        /// Executes a query and returns a data reader
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <returns></returns>
        IDataReader ExecuteReader(DbCommand command);

        /// <summary>
        /// Executes a query and returns a data reader
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <param name="aLogger"></param>
        /// <returns></returns>
        IDataReader ExecuteReader(DbCommand command, IDatabaseActivityLogger aLogger);

        /// <summary>
        /// Executes a query and returns a single value
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <returns></returns>
        object ExecuteScalar(DbCommand command);

        /// <summary>
        /// Executes a query and returns a single value
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <param name="aLogger"></param>
        /// <returns></returns>
        object ExecuteScalar(DbCommand command, IDatabaseActivityLogger aLogger);

        /// <summary>
        /// Executes a non query command query
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <returns>Number of affected records</returns>
        int ExecuteNonQuery(DbCommand command);

        /// <summary>
        /// Executes a non query command query
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <param name="aLogger"></param>
        /// <returns>Number of affected records</returns>
        int ExecuteNonQuery(DbCommand command, IDatabaseActivityLogger aLogger);

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
        void LoadDataSet(DbCommand command, DataSet dataSet, string tableName);

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
        void LoadDataSet(DbCommand command, DataSet dataSet, string tableName, IDatabaseActivityLogger aLogger);

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
        int UpdateDataSet(DataSet dataSet, string tableName, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand, UpdateBehavior behavior);

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
        int UpdateDataSet(DataSet dataSet, string tableName, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand, UpdateBehavior behavior, IDatabaseActivityLogger aLogger);

        /// <summary>
        /// Gets the string used to open a database.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// <para>
        /// Creates a <see cref="T:System.Data.Common.DbCommand" /> for a SQL query.
        /// </para>
        /// </summary>
        /// <param name="query"><para>The text of the query.</para></param>
        /// <returns>
        /// <para>
        /// The <see cref="T:System.Data.Common.DbCommand" /> for the SQL query.
        /// </para>
        /// </returns>
        DbCommand GetSqlStringCommand(string query);


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
        DbConnection CreateConnection();


        /// <summary>
        /// <para>
        /// Gets DB client type used for connection
        /// </para>
        /// </summary>
        Database.DbClientType DatabaseClientType {get; }
    }
}
