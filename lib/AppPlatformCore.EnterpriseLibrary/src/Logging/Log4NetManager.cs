using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;

// We want this assembly to have a seperate logging repository to the 
// rest of the application. We will configure this repository seperatly.
[assembly: log4net.Config.Repository("ZIS")]

// Configure logging for this assembly using the 'SimpleModule.dll.log4net' file
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]

namespace AppPlatform.Core.EnterpriseLibrary.Logging
{
    /// <summary>
    /// (Singleton) class used logging
    /// </summary>
    public class Log4NetManager : ILog4NetManager
    {
        private static readonly log4net.ILog eventLog = log4net.LogManager.GetLogger("SystemLog");
        private readonly log4net.ILog traceLog;


        /// <summary>
        /// 
        /// </summary>
        public class Level
        {
            /// <summary>
            /// 
            /// </summary>
            public static readonly log4net.Core.Level Debug = log4net.Core.Level.Debug;
            /// <summary>
            /// 
            /// </summary>
            public static readonly log4net.Core.Level Info = log4net.Core.Level.Info;
            /// <summary>
            /// 
            /// </summary>
            public static readonly log4net.Core.Level Warn = log4net.Core.Level.Warn;
            /// <summary>
            /// 
            /// </summary>
            public static readonly log4net.Core.Level Error = log4net.Core.Level.Error;
            /// <summary>
            /// 
            /// </summary>
            public static readonly log4net.Core.Level Fatal = log4net.Core.Level.Fatal;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Log4NetManager()
        {
            traceLog = log4net.LogManager.GetLogger(GetInvokingClass());
        }

        /// <summary>
        /// Constructor with specified logger
        /// </summary>
        public Log4NetManager(string logger)
        {
            traceLog = log4net.LogManager.GetLogger(logger);
        }

        /// <summary>
        /// Retrieves from the stack the calling method's namespace and class
        /// </summary>
        private static string GetInvokingClass()
        {
            StackTrace sTrace = new StackTrace(true);
            return sTrace.GetFrame(2).GetMethod().ReflectedType.FullName;
        }

        /// <summary>
        /// Retrieves from the stack the calling method's namespace, class and name
        /// </summary>
        private static string GetInvokingMethod()
        {
            StackTrace sTrace = new StackTrace(true);
            return sTrace.GetFrame(2).GetMethod().ReflectedType.FullName + "." + sTrace.GetFrame(2).GetMethod().Name;
        }

        /// <summary>
        /// Retrieves from the stack the calling method's caller namespace, class and name
        /// </summary>
        private static string GetInvokingInvokingMethod()
        {
            StackTrace sTrace = new StackTrace(true);
            return sTrace.GetFrame(3).GetMethod().ReflectedType.FullName + "." + sTrace.GetFrame(3).GetMethod().Name;
        }

        /// <summary>
        /// Retrieves from the stack the calling method's namespace, class and name
        /// </summary>
        private int GetLine()
        {
            StackTrace sTrace = new StackTrace(true);
            return sTrace.GetFrame(2).GetFileLineNumber();
        }

        /// <summary>
        /// Retrieves from the stack the calling method's file name
        /// </summary>
        private string GetFile()
        {
            StackTrace sTrace = new StackTrace(true);
            return sTrace.GetFrame(2).GetFileName();
        }

        private static void ParseZisContext()
        {
            //EdpContext ctx = (EdpContext)CallContext.GetData("EdpContext");
            //if (ctx != null)
            //    log4net.LogicalThreadContext.Properties["sessionID"] = ctx.SessionID;
            //else
            //    log4net.LogicalThreadContext.Properties.Remove("sessionID");
        }

        ///// <summary>
        ///// sets business event information
        ///// </summary>
        //public void SetUserProfileContext(bool addRepresenterData, ZisProfile profile)
        //{
        //    if (profile != null)
        //    {
        //        log4net.LogicalThreadContext.Properties["certificateID"] = profile.CertificateID;
        //        log4net.LogicalThreadContext.Properties["profileType"] = (char)profile.ZisProfileType;

        //        if (addRepresenterData && profile.ZisProfileType != ProfileType.Unknown
        //          && profile is KnownProfile && ((KnownProfile)profile).Representing != null)
        //        {
        //            if (((KnownProfile)profile).Representing.TaxPayerID != -1)
        //            {
        //                log4net.LogicalThreadContext.Properties["representedTaxPayerID"] = ((KnownProfile)profile).Representing.TaxPayerID;
        //                log4net.LogicalThreadContext.Properties["representedTaxPayerType"] = (char)((KnownProfile)profile).Representing.TaxPayerType;
        //            }
        //        }

        //        log4net.LogicalThreadContext.Properties["accessChannel"] = (char)profile.ZisAccessChannel;
        //    }
        //}

        ///// <summary>
        ///// sets business event information
        ///// </summary>
        //private static void SetDocumentDataContext(IBaseDocumentDataLog documentData)
        //{
        //    if (documentData != null)
        //    {
        //        log4net.LogicalThreadContext.Properties["documentID"] = documentData.DocumentID;
        //        log4net.LogicalThreadContext.Properties["typeFormID"] = (int)Enum.Parse(typeof(TypeFormEnum), documentData.FormType.ToString());
        //    }
        //}

        /// <summary>
        /// Clear the local thread storage of all Name/Value pairs
        /// 
        /// 
        /// </summary>
        public void ClearContext()
        {
            log4net.LogicalThreadContext.Properties.Clear();
        }

        /// <summary>
        /// Provides a local thread storage for Name/Value pair.
        /// </summary>
        /// <remarks>
        ///  Name/Value pair is added to every log message that this thread will make from now on.
        ///  Name has to be explicitly added also to layout element in log4net configuration file.
        /// </remarks>
        /// <example>
        /// sample.cs: traceLog.SetContextValue("representedTaxPayerID", "23456543")
        /// log4net.config: ..conversionPattern value="%date %property{log4net:HostName} %property{representedTaxPayerID}
        /// </example>
        /// 
        public void SetContextValue(string name, object value)
        {
            log4net.LogicalThreadContext.Properties[name] = value;
        }

        /// <summary>
        /// Flushes all the buffers of all configured appenders. Can be invoked on demand or hooked on a timer.
        /// </summary>
        public static void Flush()
        {
            foreach (log4net.Appender.IAppender appender in log4net.LogManager.GetRepository().GetAppenders())
            {
                log4net.Appender.BufferingAppenderSkeleton buffer = appender as log4net.Appender.BufferingAppenderSkeleton;
                if (buffer != null)
                    buffer.Flush();
            }
        }

        #region Debug
        /// <summary>
        /// Checks if this class logger is enabled for the Debug level.
        /// </summary>
        public bool IsDebugEnabled
        {
            get { return traceLog.IsDebugEnabled; }
        }

        /// <summary>
        /// Logs a message with the Debug level but does not retrieve the invoking
        /// method. Instead, it assummes that it is the same as from the previous
        /// logging statement.
        /// </summary>
        public void FastDebug(string message)
        {
            ParseZisContext();
            traceLog.Debug(message);
        }

        /// <summary>
        /// Logs a message with the Debug level.
        /// </summary>
        public void Debug(string message)
        {
            ParseZisContext();
            log4net.LogicalThreadContext.Properties["method"] = GetInvokingMethod();
            traceLog.Debug(message);
        }

        /// <summary>
        /// Logs a message with the Debug level including the stack trace of the Exception passed as a parameter.
        /// </summary>
        public void Debug(string message, Exception exception)
        {
            ParseZisContext();
            log4net.LogicalThreadContext.Properties["method"] = GetInvokingMethod();
            traceLog.Debug(message, exception);
        }

        /// <summary>
        /// Logs a formatted message string with the Debug level.
        /// </summary>
        public void DebugFormat(string format, params object[] message)
        {
            ParseZisContext();
            log4net.LogicalThreadContext.Properties["method"] = GetInvokingMethod();
            traceLog.DebugFormat(format, message);
        }

        #endregion

        #region info
        /// <summary>
        /// Checks if this class logger is enabled for the Info level.
        /// </summary>
        public bool IsInfoEnabled
        {
            get { return traceLog.IsInfoEnabled; }
        }

        /// <summary>
        /// Logs a message with the Info level but does not retrieve the invoking
        /// method. Instead, it assummes that it is the same as from the previous
        /// logging statement.
        /// </summary>
        public void FastInfo(string message)
        {
            ParseZisContext();
            traceLog.Info(message);
        }

        /// <summary>
        /// Logs a message with the Info level.
        /// </summary>
        public void Info(string message)
        {
            ParseZisContext();
            log4net.LogicalThreadContext.Properties["method"] = GetInvokingMethod();
            traceLog.Info(message);
        }

        /// <summary>
        /// Logs a message with the Info level including the stack trace of the Exception passed as a parameter.
        /// </summary>
        public void Info(string message, Exception exception)
        {
            ParseZisContext();
            log4net.LogicalThreadContext.Properties["method"] = GetInvokingMethod();
            traceLog.Info(message, exception);
        }

        /// <summary>
        /// Logs a formatted message string with the Info level.
        /// </summary>
        public void InfoFormat(string format, params object[] message)
        {
            ParseZisContext();
            log4net.LogicalThreadContext.Properties["method"] = GetInvokingMethod();
            traceLog.InfoFormat(format, message);
        }
        #endregion

        #region warn
        /// <summary>
        /// Checks if this class logger is enabled for the Warn level.
        /// </summary>
        public bool IsWarnEnabled
        {
            get { return traceLog.IsWarnEnabled; }
        }

        /// <summary>
        /// Logs a message with the Warn level but does not retrieve the invoking
        /// method. Instead, it assummes that it is the same as from the previous
        /// logging statement.
        /// </summary>
        public void FastWarn(string message)
        {
            ParseZisContext();
            traceLog.Warn(message);
        }

        /// <summary>
        /// Logs a message with the Warn level.
        /// </summary>
        public void Warn(string message)
        {
            ParseZisContext();
            log4net.LogicalThreadContext.Properties["method"] = GetInvokingMethod();
            traceLog.Warn(message);
        }

        /// <summary>
        /// Logs a message with the Warn level including the stack trace of the Exception passed as a parameter.
        /// </summary>
        public void Warn(string message, Exception exception)
        {
            ParseZisContext();
            log4net.LogicalThreadContext.Properties["method"] = GetInvokingMethod();
            traceLog.Warn(message, exception);
        }

        /// <summary>
        /// Logs a formatted message string with the Warn level.
        /// </summary>
        public void WarnFormat(string format, params object[] message)
        {
            ParseZisContext();
            log4net.LogicalThreadContext.Properties["method"] = GetInvokingMethod();
            traceLog.WarnFormat(format, message);
        }

        #endregion

        #region error
        /// <summary>
        /// Checks if this class logger is enabled for the Error level.
        /// </summary>
        public bool IsErrorEnabled
        {
            get { return traceLog.IsErrorEnabled; }
        }


        /// <summary>
        /// Logs a message with the Error level but does not retrieve the invoking
        /// method. Instead, it assummes that it is the same as from the previous
        /// logging statement.
        /// </summary>
        public void FastError(string message)
        {
            ParseZisContext();
            traceLog.Error(message);
        }

        /// <summary>
        /// Logs a message with the Error level.
        /// </summary>
        public void Error(string message)
        {
            ParseZisContext();
            log4net.LogicalThreadContext.Properties["method"] = GetInvokingMethod();
            traceLog.Error(message);
        }

        /// <summary>
        /// Logs a message with the Error level including the stack trace of the Exception passed as a parameter.
        /// </summary>
        public void Error(string message, Exception exception)
        {
            ParseZisContext();
            log4net.LogicalThreadContext.Properties["method"] = GetInvokingMethod();
            traceLog.Error(message, exception);
        }

        /// <summary>
        /// Logs a formatted message string with the Error level.
        /// </summary>
        public void ErrorFormat(string format, params object[] message)
        {
            ParseZisContext();
            log4net.LogicalThreadContext.Properties["method"] = GetInvokingMethod();
            traceLog.ErrorFormat(format, message);
        }

        #endregion

        #region fatal
        /// <summary>
        /// Checks if this class logger is enabled for the Fatal level.
        /// </summary>
        public bool IsFatalEnabled
        {
            get { return traceLog.IsFatalEnabled; }
        }

        /// <summary>
        /// Logs a message with the Fatal level but does not retrieve the invoking
        /// method. Instead, it assummes that it is the same as from the previous
        /// logging statement.
        /// </summary>
        public void FastFatal(string message)
        {
            ParseZisContext();
            traceLog.Fatal(message);
        }

        /// <summary>
        /// Logs a message with the Fatal level.
        /// </summary>
        public void Fatal(string message)
        {
            ParseZisContext();
            log4net.LogicalThreadContext.Properties["method"] = GetInvokingMethod();
            traceLog.Fatal(message);
        }

        /// <summary>
        /// Logs a message with the Fatal level including the stack trace of the Exception passed as a parameter.
        /// </summary>
        public void Fatal(string message, Exception exception)
        {
            ParseZisContext();
            log4net.LogicalThreadContext.Properties["method"] = GetInvokingMethod();
            traceLog.Fatal(message, exception);
        }

        /// <summary>
        /// Logs a formatted message string with the Fatal level.
        /// </summary>
        public void FatalFormat(string format, params object[] message)
        {
            ParseZisContext();
            log4net.LogicalThreadContext.Properties["method"] = GetInvokingMethod();
            traceLog.FatalFormat(format, message);
        }

        #endregion
    }
}