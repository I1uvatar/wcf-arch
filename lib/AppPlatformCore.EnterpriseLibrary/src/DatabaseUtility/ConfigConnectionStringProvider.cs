using System.Configuration;

namespace AppPlatform.Core.EnterpriseLibrary.DatabaseUtility
{
    /// <summary>
    /// Retrieve conection string from configuration file
    /// </summary>
    public class ConfigConnectionStringProvider : IConnectionStringProvider
    {
        private readonly string connectionString;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="connectionStringName">Configuration file connection string name</param>
        public ConfigConnectionStringProvider(string connectionStringName)
        {
            connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        #region IConnectionStringProvider Members

        /// <summary>
        /// Retrieve connection string
        /// </summary>
        /// <returns>connection string</returns>
        public string GetConnectionString()
        {
            return connectionString;
        }

        #endregion
    }
}
