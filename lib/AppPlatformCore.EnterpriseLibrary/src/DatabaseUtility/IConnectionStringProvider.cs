namespace AppPlatform.Core.EnterpriseLibrary.DatabaseUtility
{
    /// <summary>
    /// Interface for providing connection string
    /// </summary>
    public interface IConnectionStringProvider
    {
        /// <summary>
        /// Retrieve connection string
        /// </summary>
        /// <returns>connection string</returns>
        string GetConnectionString();
    }
}
