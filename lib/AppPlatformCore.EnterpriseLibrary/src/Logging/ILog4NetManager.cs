using System;

namespace AppPlatform.Core.EnterpriseLibrary.Logging
{
    /// <summary>
    /// Interface for log4net manager
    /// </summary>
    public interface ILog4NetManager
    {
        /// <summary>
        /// Logs a message with the Debug level but does not retrieve the invoking
        /// method. Instead, it assummes that it is the same as from the previous
        /// logging statement.
        /// </summary>
        void FastDebug(string message);

        /// <summary>
        /// Logs a message with the Debug level.
        /// </summary>
        void Debug(string message);

        /// <summary>
        /// Logs a message with the Debug level including the stack trace of the Exception passed as a parameter.
        /// </summary>
        void Debug(string message, Exception exception);

        /// <summary>
        /// Logs a formatted message string with the Debug level.
        /// </summary>
        void DebugFormat(string format, params object[] message);

        /// <summary>
        /// Logs a message with the Info level but does not retrieve the invoking
        /// method. Instead, it assummes that it is the same as from the previous
        /// logging statement.
        /// </summary>
        void FastInfo(string message);

        /// <summary>
        /// Logs a message with the Info level.
        /// </summary>
        void Info(string message);

        /// <summary>
        /// Logs a message with the Info level including the stack trace of the Exception passed as a parameter.
        /// </summary>
        void Info(string message, Exception exception);

        /// <summary>
        /// Logs a formatted message string with the Info level.
        /// </summary>
        void InfoFormat(string format, params object[] message);

        /// <summary>
        /// Logs a message with the Warn level but does not retrieve the invoking
        /// method. Instead, it assummes that it is the same as from the previous
        /// logging statement.
        /// </summary>
        void FastWarn(string message);

        /// <summary>
        /// Logs a message with the Warn level.
        /// </summary>
        void Warn(string message);

        /// <summary>
        /// Logs a message with the Warn level including the stack trace of the Exception passed as a parameter.
        /// </summary>
        void Warn(string message, Exception exception);

        /// <summary>
        /// Logs a formatted message string with the Warn level.
        /// </summary>
        void WarnFormat(string format, params object[] message);

        /// <summary>
        /// Logs a message with the Error level but does not retrieve the invoking
        /// method. Instead, it assummes that it is the same as from the previous
        /// logging statement.
        /// </summary>
        void FastError(string message);

        /// <summary>
        /// Logs a message with the Error level.
        /// </summary>
        void Error(string message);

        /// <summary>
        /// Logs a message with the Error level including the stack trace of the Exception passed as a parameter.
        /// </summary>
        void Error(string message, Exception exception);

        /// <summary>
        /// Logs a formatted message string with the Error level.
        /// </summary>
        void ErrorFormat(string format, params object[] message);

        /// <summary>
        /// Logs a message with the Fatal level but does not retrieve the invoking
        /// method. Instead, it assummes that it is the same as from the previous
        /// logging statement.
        /// </summary>
        void FastFatal(string message);

        /// <summary>
        /// Logs a message with the Fatal level.
        /// </summary>
        void Fatal(string message);

        /// <summary>
        /// Logs a message with the Fatal level including the stack trace of the Exception passed as a parameter.
        /// </summary>
        void Fatal(string message, Exception exception);

        /// <summary>
        /// Logs a formatted message string with the Fatal level.
        /// </summary>
        void FatalFormat(string format, params object[] message);
    }
}