//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Logging Application Block
//===============================================================================
// Copyright © Microsoft Corporation. All rights reserved.
// Adapted from ACA.NET with permission from Avanade Inc.
// ACA.NET copyright © Avanade Inc. All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Diagnostics;
using System.Messaging;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Threading;
using HermesSoftLab.EnterpriseLibrary.Logging;
using HermesSoftLab.EnterpriseLibrary.Messaging;
using Microsoft.Practices.EnterpriseLibrary.Logging.MsmqDistributor.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Logging.MsmqDistributor.Properties;
using Microsoft.Practices.EnterpriseLibrary.Logging.MsmqDistributor.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.MsmqDistributor
{
	/// <summary>
	/// Receive new log messages from MSMQ and distribute each log entry.
	/// </summary>
	public class MsmqLogDistributor
	{
		private bool isCompleted = true;
		private bool stopReceiving = false;

        private int errorCounter = 0;

		private readonly string msmqPath;
		private readonly DistributorEventLogger eventLogger;

	    private readonly IMessageFormatter queueFormatter = new RawMessageFormatter();
        private readonly DbWriterFactory loggerFactory = new DbWriterFactory();

		/// <summary>
		/// Setup the queue and the formatter of the messages.
		/// </summary>
        public MsmqLogDistributor(DbWriterFactory loggerFactory, string msmqPath, DistributorEventLogger eventLogger)
		{
            this.loggerFactory = loggerFactory;
			this.msmqPath = msmqPath;
			this.eventLogger = eventLogger;
		}

		/// <summary>
		/// Read-only property to check if the synchronous receive is completed.
		/// </summary>
		public virtual bool IsCompleted
		{
			get { return this.isCompleted; }
		}

		/// <summary>
		/// Instructs the listener to stop receiving messages.
		/// </summary>
		public virtual bool StopReceiving
		{
			get { return this.stopReceiving; }
			set { this.stopReceiving = value; }
		}

		/// <summary>
		/// Start receiving the message(s) from the queue.
		/// The messages will be taken from the queue until the queue is empty.
		/// This method is triggered every x seconds. (x is defined in application configuration file)
		/// </summary>
		public virtual void CheckForMessages()
		{            
            IConfigurationSource configurationSource = ConfigurationSourceFactory.Create();
            MsmqDistributorSettings distributorSettings = MsmqDistributorSettings.GetSettings(configurationSource);
            int retryLimit = distributorSettings.Retries;

			try
			{
				ReceiveQueuedMessages();
                errorCounter = 0;
			}
			catch (Exception ex)
			{
                //After x retries, the service will write this error to the event log and try to stop.
                //Prevents filling the event log with the same error.
                if (errorCounter == retryLimit)
                {
                    string errorMsg = string.Format(Resources.Culture, Resources.MsmqReceiveGeneralError, msmqPath);
                    this.eventLogger.LogServiceFailure(
                        errorMsg,
                        ex,
                        TraceEventType.Error);

                    if (ex is LoggingException) {
                        throw;
                    }

                    throw new LoggingException(errorMsg, ex);
                }
                errorCounter++;
			}
			finally
			{
				this.isCompleted = true;
			}
		}

		/// <summary>
		/// This method supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
		/// </summary>
		/// <param name="code">The error code.</param>
		/// <param name="e">The exception, or null.</param>
		/// <returns>The logged message.</returns>
		protected string LogMessageQueueException(MessageQueueErrorCode code, Exception e)
		{
			TraceEventType logType = TraceEventType.Error;
			string errorMsg;

			if (code == MessageQueueErrorCode.TransactionUsage)
			{
				errorMsg = string.Format(Resources.Culture, Resources.MsmqInvalidTransactionUsage, msmqPath);
			}
			else if (code == MessageQueueErrorCode.IOTimeout)
			{
				errorMsg = string.Format(Resources.Culture, Resources.MsmqReceiveTimeout, msmqPath);
				logType = TraceEventType.Warning;
			}
			else if (code == MessageQueueErrorCode.AccessDenied)
			{
				errorMsg = string.Format(Resources.Culture, Resources.MsmqAccessDenied, msmqPath, WindowsIdentity.GetCurrent().Name);
			}
			else
			{
				errorMsg = string.Format(Resources.Culture, Resources.MsmqReceiveError, msmqPath);
			}

			this.eventLogger.LogServiceFailure(
				errorMsg,
				e,
				logType);

			return errorMsg;
		}

		private MessageQueue CreateMessageQueue()
		{
			MessageQueue messageQueue = new MessageQueue(msmqPath, false, true);
			((XmlMessageFormatter)messageQueue.Formatter).TargetTypeNames = new [] { "System.String" };
			return messageQueue;
		}

		private bool IsQueueEmpty()
		{
			bool empty = false;
			try
			{
				using (MessageQueue msmq = CreateMessageQueue())
				{
					msmq.Peek(new TimeSpan(0));
				}
			}
			catch (MessageQueueException e)
			{
				if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
				{
					empty = true;
				}
			}

			return empty;
		}

		private void ReceiveQueuedMessages()
		{
			this.isCompleted = false;
			while (!IsQueueEmpty())
			{
				using (MessageQueue msmq = CreateMessageQueue())
				{
					var message = msmq.Peek();

					object logEntry;
					try
					{
					    logEntry = this.queueFormatter.Read(message);
					}
					catch (FormatException formatException)
					{
						string logMessage = string.Format(
							Resources.Culture,
							Resources.ExceptionCouldNotDeserializeMessageFromQueue,
							message.Id,
							msmq.Path);

						this.eventLogger.LogServiceFailure(
							logMessage,
							formatException,
							TraceEventType.Error);

						throw new LoggingException(logMessage, formatException);
					}
					catch (SerializationException serializationException)
					{
						string logMessage = string.Format(
							Resources.Culture,
							Resources.ExceptionCouldNotDeserializeMessageFromQueue,
							message.Id,
							msmq.Path);

						this.eventLogger.LogServiceFailure(
							logMessage,
							serializationException,
							TraceEventType.Error);

						throw new LoggingException(logMessage, serializationException);
					}

					if (logEntry != null)
					{
					    var writer = this.loggerFactory.GetWriter(logEntry);
                        writer.Write(logEntry);
					}

                    msmq.Receive();

				    if (this.StopReceiving)
				    {
				        this.isCompleted = true;
				        return;
				    }
				}
			}
		}
	}
}