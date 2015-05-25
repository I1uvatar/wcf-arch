using System;
using System.Diagnostics;
using AppPlatform.Core.EnterpriseLibrary.Messaging;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using System.Messaging;
using System.Collections.Generic;
using System.Configuration;

namespace AppPlatform.Core.EnterpriseLibrary.Logging
{
    /// <summary>
    /// MsqmTraceListener, specialised for handling HslLogEntry log records.
    /// It is using RawMessageFormatter to format MSMQ messages.
    /// </summary>
    [ConfigurationElementType(typeof(CustomTraceListenerData))]
    public class MsmqRawTraceListener : CustomTraceListener
    {
        readonly IMsmqSendInterfaceFactory msmqInterfaceFactory;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MsmqRawTraceListener()
        {
            this.msmqInterfaceFactory = new MsmqSendInterfaceFactory();
        }

        /// <summary>
        /// The path to the queue to deliver to.
        /// </summary>
        public string QueuePath
        {
            get { return this.Attributes["queuePath"]; }
            set { this.Attributes["queuePath"] = value; }
        }

        /// <summary>
        /// The priority for the messages to send.
        /// </summary>
        public MessagePriority MessagePriority
        {
            get { return (MessagePriority)Enum.Parse(typeof(MessagePriority), this.Attributes["messagePriority"]); }
            set { this.Attributes["messagePriority"] = value.ToString(); }
        }

        /// <summary>
        /// The recoverable flag for the messages to send.
        /// </summary>
        public bool Recoverable
        {
            get { return bool.Parse(this.Attributes["recoverable"]); }
            set { this.Attributes["recoverable"] = value.ToString(); }
        }

        /// <summary>
        /// The timeToBeReceived for the messages to send.
        /// </summary>
        public TimeSpan TimeToBeReceived
        {
            get { return TimeSpan.Parse(this.Attributes["timeToBeReceived"]); }
            set { this.Attributes["timeToBeReceived"] = value.ToString(); }
        }

        /// <summary>
        /// The timeToReachQueue for the messages to send.
        /// </summary>
        public TimeSpan TimeToReachQueue
        {
            get { return TimeSpan.Parse(this.Attributes["timeToReachQueue"]); }
            set { this.Attributes["timeToReachQueue"] = value.ToString(); }
        }

        /// <summary>
        /// The <see cref="MessageQueueTransactionType"/> for the message to send.
        /// </summary>
        public MessageQueueTransactionType TransactionType
        {
            get { return (MessageQueueTransactionType)Enum.Parse(typeof(MessageQueueTransactionType), this.Attributes["transactionType"]); }
            set { this.Attributes["transactionType"] = value.ToString(); }
        }

        /// <summary>
        /// The useAuthentication flag for the messages to send.
        /// </summary>
        public bool UseAuthentication
        {
            get { return bool.Parse(this.Attributes["useAuthentication"]); }
            set { this.Attributes["useAuthentication"] = value.ToString(); }
        }

        /// <summary>
        /// The useDeadLetterQueue flag for the messages to send.
        /// </summary>
        public bool UseDeadLetterQueue
        {
            get { return bool.Parse(this.Attributes["useDeadLetterQueue"]); }
            set { this.Attributes["useDeadLetterQueue"] = value.ToString(); }
        }

        /// <summary>
        /// The useEncryption flag for the messages to send.
        /// </summary>
        public bool UseEncryption
        {
            get { return bool.Parse(this.Attributes["useEncryption"]); }
            set { this.Attributes["useEncryption"] = value.ToString(); }
        }


        /// <summary>
        /// Checks configuration and enviromental settings
        /// - Throws an error if target MSMQ queue cannot be accessed
        /// </summary>
        public void ConfigurationCheck()
        {
            if (MessageQueue.Exists(this.QueuePath))
            {
                MessageQueue messageQueue = new MessageQueue(this.QueuePath);
                if (!messageQueue.CanWrite)
                {
                    throw new ConfigurationErrorsException(
                        string.Format(
                            "Write access to message queue '{0}' was denied.",
                            this.QueuePath)
                        );
                }
            }
            else
            {
                try
                {
                    // try to create the message queue
                    System.Messaging.MessageQueue.Create(this.QueuePath);
                    MessageQueue messageQueue = new MessageQueue(this.QueuePath, false, true);
                    messageQueue.SetPermissions("NT AUTHORITY\\NetworkService", MessageQueueAccessRights.ReceiveMessage);
                    messageQueue.SetPermissions("Administrators", MessageQueueAccessRights.FullControl);
                }
                catch (Exception ex)
                {
                    throw new ConfigurationErrorsException(
                        string.Format(
                            "Message queue '{0}' does not exist and could not be created.",
                            this.QueuePath),
                        ex);
                }
            };

        }

        /// <summary>
        /// Create a message from a <see cref="LogEntry"/>.
        /// </summary>
        /// <param name="logEntry">The <see cref="LogEntry"/></param>
        /// <returns>A <see cref="Message"/> object.</returns>
        public Message CreateMessage(LogEntry logEntry)
        {
            string formattedLogEntry = FormatEntry(logEntry);
            return this.CreateMessage(formattedLogEntry, logEntry.Title);
        }

        private Message CreateMessage(HslLogEntry logEntry)
        {
            var msg = this.CreateMessage(FormatEntry(logEntry), logEntry.Title);
            msg.Body = logEntry;

            return msg;
        }

        Message CreateMessage(string messageBody, string messageLabel)
        {
            Message queueMessage = new Message
            {
                Body = messageBody,
                Label = messageLabel,
                Priority = this.MessagePriority,
                TimeToBeReceived = this.TimeToBeReceived,
                TimeToReachQueue = this.TimeToReachQueue,
                Recoverable = this.Recoverable,
                UseAuthentication = this.UseAuthentication,
                UseDeadLetterQueue = this.UseDeadLetterQueue,
                UseEncryption = this.UseEncryption,
                Formatter = new RawMessageFormatter()
            };

            return queueMessage;
        }

        string FormatEntry(LogEntry entry)
        {
            string formattedMessage = Formatter.Format(entry);
            return formattedMessage;
        }

        void SendMessageToQueue(string message)
        {
            using (IMsmqSendInterface messageQueueInterface = msmqInterfaceFactory.CreateMsmqInterface(this.QueuePath))
            {
                using (Message queueMessage = CreateMessage(message, string.Empty))
                {
                    messageQueueInterface.Send(queueMessage, this.TransactionType);
                    messageQueueInterface.Close();
                }
            }
        }

        void SendMessageToQueue(LogEntry logEntry)
        {
            using (IMsmqSendInterface messageQueueInterface = msmqInterfaceFactory.CreateMsmqInterface(this.QueuePath))
            {
                using (Message queueMessage = CreateMessage(logEntry))
                {
                    messageQueueInterface.Send(queueMessage, this.TransactionType);
                    messageQueueInterface.Close();
                }
            }
        }

        void SendMessageToQueue(HslLogEntry logEntry)
        {
            using (IMsmqSendInterface messageQueueInterface = msmqInterfaceFactory.CreateMsmqInterface(this.QueuePath))
            {
                using (Message queueMessage = CreateMessage(logEntry))
                {
                    messageQueueInterface.Send(queueMessage, this.TransactionType);
                    messageQueueInterface.Close();
                }
            }
        }

        /// <summary>
        /// Sends the traced object to its final destination through a <see cref="MessageQueue"/>.
        /// </summary>
        /// <param name="eventCache">The context information provided by <see cref="System.Diagnostics"/>.</param>
        /// <param name="source">The name of the trace source that delivered the trace data.</param>
        /// <param name="eventType">The type of event.</param>
        /// <param name="id">The id of the event.</param>
        /// <param name="data">The data to trace.</param>
        public override void TraceData(TraceEventCache eventCache,
                                       string source,
                                       TraceEventType eventType,
                                       int id,
                                       object data)
        {
            if ((Filter == null) || Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null))
            {
                if (data is HslLogEntry)
                {
                    SendMessageToQueue(data as HslLogEntry);
                }
                else if (data is LogEntry)
                {
                    this.SendMessageToQueue(data as LogEntry);
                }
                else if (data is string)
                {
                    this.Write(data as string);
                }
                else
                {
                    base.TraceData(eventCache, source, eventType, id, data);
                }
            }
        }

        /// <summary>
        /// Writes the specified message to the message queue.
        /// </summary>
        /// <param name="message">Message to be written.</param>
        public override void Write(string message)
        {
            this.SendMessageToQueue(message);
        }

        /// <summary>
        /// Writes the specified message to the message queue.
        /// </summary>
        /// <param name="message"></param>
        public override void WriteLine(string message)
        {
            this.Write(message);
        }
    }
}
