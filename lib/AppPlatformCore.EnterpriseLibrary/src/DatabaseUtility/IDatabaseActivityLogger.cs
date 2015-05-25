using System;
using System.Data.Common;

namespace AppPlatform.Core.EnterpriseLibrary.DatabaseUtility
{
    /// <summary>
    /// Logs system database events
    /// </summary>
    public interface IDatabaseActivityLogger
    {
        /// <summary>
        /// Before the command is executed
        /// </summary>
        /// <param name="aCommand"></param>
        void LogPreExecute(DbCommand aCommand);

        /// <summary>
        /// In case of exception
        /// </summary>
        /// <param name="aCommand"></param>
        /// <param name="ex"></param>
        void LogException(DbCommand aCommand, Exception ex);

        /// <summary>
        /// After the command is executed
        /// </summary>
        /// <param name="aCommand">A command.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="elapsedMilliseconds">The elapsed milliseconds.</param>
        /// <param name="recordsAffected">The records affected.</param>
        void LogPostExecute(DbCommand aCommand, DateTime startTime, long elapsedMilliseconds, long? recordsAffected);

        /// <summary>
        /// Logs the post execute.
        /// </summary>
        /// <param name="aCommand">A command.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="elapsedMilliseconds">The elapsed milliseconds.</param>
        /// <param name="recordsAffected">The records affected.</param>
        /// <param name="ex">The ex.</param>
        void LogPostExecute(DbCommand aCommand, DateTime startTime, long elapsedMilliseconds, long? recordsAffected, Exception ex);

        /// <summary>
        /// ConfigureCommandFormat
        /// </summary>
        /// <param name="aFormatter"></param>
        void ConfigureCommandFormat(Func<DbCommand, string> aFormatter);

        /// <summary>
        /// LogParameterValues 
        /// </summary>
        bool LogParameterValues { get; set; }

        /// <summary>
        /// LogSecurityEvent 
        /// </summary>
        bool LogSecurityEvent { get; set; }
    }

    /// <summary>
    /// Default logger
    /// </summary>
    internal class NullLogger : IDatabaseActivityLogger
    {
        /// <summary>
        /// LogPreExecute
        /// </summary>
        /// <param name="aCommand"></param>
        public void LogPreExecute(DbCommand aCommand) { }
        
        /// <summary>
        /// Log exception
        /// </summary>
        /// <param name="command">Db command</param>
        /// <param name="ex">Exception to log</param>
        public void LogException(DbCommand command, Exception ex) { }

        /// <summary>
        /// After the command is executed
        /// </summary>
        public void LogPostExecute(DbCommand command, TimeSpan duration, long? recordsAffected) { }

        /// <summary>
        /// After the command is executed
        /// </summary>
        public void LogPostExecute(DbCommand aCommand, DateTime startTime, long elapsedMilliseconds, long? recordsAffected) { }

        /// <summary>
        /// Logs the post execute.
        /// </summary>
        public void LogPostExecute(DbCommand aCommand, DateTime startTime, long elapsedMilliseconds, long? recordsAffected, Exception ex) { }

        /// <summary>
        /// ConfigureCommandFormat
        /// </summary>
        public void ConfigureCommandFormat(Func<DbCommand, string> aFormatter) { }

        /// <summary>
        /// LogParameterValues 
        /// </summary>
        public bool LogParameterValues { get; set; }

        /// <summary>
        /// LogSecurityEvent 
        /// </summary>
        public bool LogSecurityEvent { get; set; }
    }
}
