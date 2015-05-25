namespace AppPlatform.Core.EnterpriseLibrary.Logging
{
    /// <summary>
    /// Log entry processor, writing event info to a DB.
    /// </summary>
    public interface IDbLogWriter
    {
        /// <summary>
        /// Write event data
        /// </summary>
        /// <param name="anEntry"></param>
        void Write(object anEntry);
    }
}