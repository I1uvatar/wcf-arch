using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace AppPlatform.Core.EnterpriseLibrary.DatabaseUtility
{
    /// <summary>
    /// Defines methods for listening changes on database
    /// </summary>
    public interface IDatabaseListener : IDisposable
    {
        /// <summary>
        /// Start database listening
        /// </summary>
        void Start();
        /// <summary>
        /// Notifies about database changes
        /// </summary>
        event OnChangeEventHandler OnChange;
    }

    /// <summary>
    /// Implements methods for listening for database changes
    /// </summary>
    public class DatabaseListener : IDatabaseListener
    {
        #region Private fields

        private readonly string listeningProcedure;
        private readonly IDatabase database;
        private SqlDependency dependency;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of DatabaseListener class for database specified by conn string name, and a listening procedure
        /// </summary>
        /// <param name="connectionStringName">Connection string configuraion name</param>
        /// <param name="listeningProcedure">Listening procedure name</param>
        public DatabaseListener(string connectionStringName, string listeningProcedure)
        {
            this.database = Database.Create(connectionStringName);
            this.listeningProcedure = listeningProcedure;
        }

        #endregion

        #region Private methods

        private void PrepareListening()
        {
            DbCommand command = database.GetStoredProcCommand(listeningProcedure);

            if (!(command is SqlCommand))
            {
                return;
            }

           // SqlCommand listeningCommand = command as SqlCommand;

            SqlCommand listeningCommand = new SqlCommand("SELECT [RegisterChangeID] FROM [zis_register].[T_RegisterChange]") {Notification = null};
            
            //listeningCommand.CommandType = CommandType.StoredProcedure;

            //SqlConnection connection = (SqlConnection)this.database.CreateConnection();

            //listeningCommand.Connection = connection;

            this.dependency = new SqlDependency(listeningCommand);

            this.dependency.OnChange += new OnChangeEventHandler(OnDbChange);

            //if (connection.State != ConnectionState.Open)
            //    connection.Open();

            //listeningCommand.ExecuteReader();

            this.database.ExecuteReader(listeningCommand);

            return;
        }

        private void OnDbChange(object sender, SqlNotificationEventArgs e)
        {
            try
            {
                if (this.OnChange != null)
                {
                    this.OnChange(sender, e);
                }
            }
            finally
            {
                //DbCommand command = database.GetStoredProcCommand(listeningProcedure);

                //if ((command is SqlCommand))
                //{
                //    SqlCommand listeningCommand = command as SqlCommand;
                //    listeningCommand.Connection = (SqlConnection)this.database.CreateConnection();
                //    this.dependency.AddCommandDependency(listeningCommand);
                //    //this.database.ExecuteNonQuery(listeningCommand);
                //    //this.dependency.OnChange += new OnChangeEventHandler(OnDbChange);
                //}
                this.PrepareListening();
            }
        }

        #endregion
        
        #region IDatabaseListener methods

        /// <summary>
        /// Notifies about database changes
        /// </summary>
        public event OnChangeEventHandler OnChange;

        /// <summary>
        /// Start database listening
        /// </summary>
        public void Start()
        {
            SqlDependency.Start(this.database.ConnectionString);
            this.PrepareListening();
        }

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            SqlDependency.Stop(this.database.ConnectionString);
        }

        #endregion
    }
}