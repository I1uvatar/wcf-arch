namespace AppPlatform.Core.EnterpriseLibrary.DatabaseUtility
{
    public interface IDataRead
    {
        /// <summary>
        /// Retrieve object value from datareader in specified column
        /// </summary>
        /// <typeparam name="T">Type of the object to retrieve</typeparam>
        /// <param name="columnName">Name of the column containing data</param>
        /// <returns>Value from the column</returns>
        T GetObjectValue<T>(string columnName) where T : class;

        /// <summary>
        /// Retrieve object nullable value from datareader in specified column
        /// </summary>
        /// <typeparam name="T">Type of the object to retrieve</typeparam>
        /// <param name="columnName">Name of the column containing data</param>
        /// <returns>Value from the column</returns>
        T? GetNullableValue<T>(string columnName) where T : struct;

        /// <summary>
        /// Retrieve value from datareader in specified column
        /// </summary>
        /// <typeparam name="T">Type of the object to retrieve</typeparam>
        /// <param name="columnName">Name of the column containing data</param>
        /// <returns>Value from the column</returns>
        T GetValue<T>(string columnName) where T : struct;

        /// <summary>
        /// Checks for existance of specified column in the reader
        /// </summary>
        /// <param name="columnName">Name of the column containing data</param>
        /// <returns>true if reader has a column with specified name</returns>
        bool HasColumn(string columnName);
    }
}
