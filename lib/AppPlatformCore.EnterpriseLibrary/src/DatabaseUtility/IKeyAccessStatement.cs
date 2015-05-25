namespace AppPlatform.Core.EnterpriseLibrary.DatabaseUtility
{
    /// <summary>
    /// Declares methods for accessing cryptography key
    /// </summary>
    public interface IKeyAccessStatement
    {
        /// <summary>
        /// Return sql statement for epening cryptography key
        /// </summary>
        /// <returns></returns>
        string GetAccessStatement();

        /// <summary>
        /// GUID of the key used for encryption
        /// </summary>
        string KeyGUID{ get; }
    }
}
