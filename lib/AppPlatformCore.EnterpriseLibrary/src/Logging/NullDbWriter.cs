namespace AppPlatform.Core.EnterpriseLibrary.Logging
{
    /// <summary>
    /// Do nothing implementation
    /// </summary>
    public class NullDbWriter : IDbLogWriter
    {
        /// <summary>
        /// Write event data
        /// </summary>
        /// <param name="anEntry"></param>
        public void Write(object anEntry)
        {
            var hEntry = anEntry as HslLogEntry;
            if (hEntry == null)
            {
                return;
            }
        }
    }
}