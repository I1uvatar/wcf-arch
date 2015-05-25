using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using log4net.Appender;

namespace AppPlatform.Core.EnterpriseLibrary.Logging
{
    class AdonetIntervalAppender : AdoNetAppender
    {
        struct StoredProcedureCallInfo
        {
            public log4net.Core.LoggingEvent LoggingEvent;
            public long ClientCallID;
            public string StoredProcedureName;
            public DateTime ExecutionStartTime;
            public long ExecutionDuration;
            public string ExceptionMessage;
            public string ExceptionStackTrace;
            public string InputParameters;
            public long? RecordsAffected;
        }

        private int m_LogInterval;
        private Queue<StoredProcedureCallInfo> queue;
        BackgroundWorker backgroundWorker;
        private DateTime lastLoggingEventsFlushDate;

        public AdonetIntervalAppender()
        {
            //Setting the lastLoggingEventsFlushDate to the current time.
            lastLoggingEventsFlushDate = DateTime.Now;
            queue = new Queue<StoredProcedureCallInfo>();
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
        }

        ///<summary>
        /// Override the Append method of the base class.
        /// Every time Logger log the LoggingEvent, Append method is called.
        /// If current time minus last logging flush out time is greater than LogInterval
        /// then flus out the LoggingEvents from the buffer else append the LoggingEvent into the Buffer.
        ///</summary>
        ///<param name=”loggingEvent”></param>
        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            try
            {
                var newQueueItem = new StoredProcedureCallInfo
                                    {
                                        LoggingEvent = loggingEvent,
                                        ClientCallID = (long) log4net.LogicalThreadContext.Properties["ClientCallID"],
                                        ExceptionMessage = (string) log4net.LogicalThreadContext.Properties["ExceptionMessage"],
                                        ExceptionStackTrace = (string) log4net.LogicalThreadContext.Properties["ExceptionStackTrace"],
                                        ExecutionDuration = (long) log4net.LogicalThreadContext.Properties["ExecutionDuration"],
                                        ExecutionStartTime = (DateTime) log4net.LogicalThreadContext.Properties["ExecutionStartTime"],
                                        InputParameters = (string) log4net.LogicalThreadContext.Properties["InputParameters"],
                                        RecordsAffected = (long?) log4net.LogicalThreadContext.Properties["RecordsAffected"],
                                        StoredProcedureName = (string)log4net.LogicalThreadContext.Properties["StoredProcedureName"]
                                    };
                queue.Enqueue(newQueueItem);
            }
            catch (Exception)
            {
                queue.Clear();
            }

            // Check if current time difference with last flus out time is greater
            if (DateTime.Now.Subtract(lastLoggingEventsFlushDate).TotalMilliseconds > LogInterval)
            {
                // Set the lastLoggingEventsFlushDate to current time.
                lastLoggingEventsFlushDate = DateTime.Now;

                //Flush out the logging events from the buffer.
                if (!backgroundWorker.IsBusy)
                    backgroundWorker.RunWorkerAsync();
            }
        }

        ///<summary>
        /// dequeue logging events from queue and invode base.Flush()
        ///</summary>
        ///<param name=”sender”></param>
        ///<param name=”e”></param>
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            do
            {
                var queueItem = queue.Dequeue();
                if (queueItem.LoggingEvent != null)
                {
                    log4net.LogicalThreadContext.Properties["ClientCallID"] = queueItem.ClientCallID;
                    log4net.LogicalThreadContext.Properties["StoredProcedureName"] = queueItem.StoredProcedureName;
                    log4net.LogicalThreadContext.Properties["ExecutionStartTime"] = queueItem.ExecutionStartTime;
                    log4net.LogicalThreadContext.Properties["ExecutionDuration"] = queueItem.ExecutionDuration;
                    log4net.LogicalThreadContext.Properties["ExceptionMessage"] = queueItem.ExceptionMessage;
                    log4net.LogicalThreadContext.Properties["ExceptionStackTrace"] = queueItem.ExceptionStackTrace;
                    log4net.LogicalThreadContext.Properties["InputParameters"] = queueItem.InputParameters;
                    log4net.LogicalThreadContext.Properties["RecordsAffected"] = queueItem.RecordsAffected;
                    base.Append(queueItem.LoggingEvent);
                }

                // process the customers request
            } while (queue.Count != 0);

            base.Flush();
        }

        ///<summary>
        /// This is the interval length in miliseconds which indicates that after
        /// LogInterval (miliseconds) LogginEvents will be flushed out from the buffer.
        ///</summary>
        public int LogInterval
        {
            get
            {
                return m_LogInterval;
            }
            set { m_LogInterval = value; }
        }

    }
}
