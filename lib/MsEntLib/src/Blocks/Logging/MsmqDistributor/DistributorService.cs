//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Logging Application Block
//===============================================================================
// Copyright � Microsoft Corporation. All rights reserved.
// Adapted from ACA.NET with permission from Avanade Inc.
// ACA.NET copyright � Avanade Inc. All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using HermesSoftLab.eHealthPlatform.Common.DatabaseUtility;
using HermesSoftLab.eHealthPlatform.Common.Logging.Enums;
using HermesSoftLab.eHealthPlatform.Common.Logging.Resource_Access;
using HermesSoftLab.EnterpriseLibrary.DatabaseUtility;
using HermesSoftLab.EnterpriseLibrary.Patterns.Gof;
using HermesSoftLab.EnterpriseLibrary.PolicyInjectionExtension;
using HermesSoftLab.EnterpriseLibrary.Unity;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.MsmqDistributor.Properties;
using Microsoft.Practices.EnterpriseLibrary.Logging.MsmqDistributor.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.MsmqDistributor.Instrumentation;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.MsmqDistributor
{
	/// <summary>
	/// <para>This type supports the Data Access Instrumentation infrastructure and is not intended to be used directly from your code.</para>
	/// </summary>    
	public class DistributorService : ServiceBase
	{
		private bool initializeComponentsCalled = false;
		internal const string DefaultApplicationName = "Enterprise Library Logging Distributor Service";
		private static string NameTag = Properties.Resources.DistributorServiceNameTag;

		private DistributorEventLogger eventLogger;
		private string applicationName;
		private ServiceStatus status;

		private MsmqListener queueListener;
		/// <summary/>
		/// <exclude/>
		public DistributorService()
		{
			base.CanStop = true;
			base.CanPauseAndContinue = true;
			base.AutoLog = false;
		}

		/// <summary/>
		/// <exclude/>
		private static void Main()
		{
			ServiceBase[] servicesToRun = new ServiceBase[] { new DistributorService() };

            ServiceBase.Run(servicesToRun);
		}

		/// <summary/>
		/// <exclude/>
		/// <devdoc>
		/// Gets or sets the current status of the service.  Values are defined in <see cref="ServiceStatus"/> enumeration.
		/// </devdoc>
		public virtual ServiceStatus Status
		{
			get { return this.status; }
			set { this.status = value; }
		}

		/// <summary/>
		/// <exclude/>
		/// <devdoc>
		/// Gets or sets the name of the windows service.
		/// </devdoc>
		public string ApplicationName
		{
			get { return this.applicationName; }
			set { this.applicationName = value; }
		}

		/// <summary/>
		/// <exclude/>
		/// <devdoc>
		/// Gets the logger used to log events for this service.
		/// </devdoc>
		public DistributorEventLogger EventLogger
		{
			get { return this.eventLogger; }
		}

		/// <summary/>
		/// <exclude/>
		/// <devdoc>
		/// Gets or sets the <see cref="MsmqListener"/> for the service.
		/// </devdoc>
		public MsmqListener QueueListener
		{
			get { return this.queueListener; }
			set { this.queueListener = value; }
		}

		/// <summary/>
		/// <exclude/>
		/// <devdoc>
		/// Initialization of the service.  Start the queue listener and write status to event log.
		/// </devdoc>
		public void InitializeComponent()
		{
            try
			{
				// Use the default settings for log name and application name.
				// This is done to ensure the windows service starts correctly.
				this.ApplicationName = DefaultApplicationName;

				this.eventLogger = new DistributorEventLogger();
				this.eventLogger.AddMessage(Resources.InitializeComponentStartedMessage, Resources.InitializeComponentStarted);
				this.status = ServiceStatus.OK;

				IConfigurationSource configurationSource = GetConfigurationSource();

				MsmqDistributorSettings distributorSettings = MsmqDistributorSettings.GetSettings(configurationSource);
				if (distributorSettings == null)
				{
					throw new ConfigurationErrorsException(string.Format(
							Resources.Culture,
							Resources.ExceptionCouldNotFindConfigurationSection,
							MsmqDistributorSettings.SectionName));
				}

                //IOC initialization
                IDecorator policyInjectionWrapper = new PolicyWrapDecorator();
                IExtendedUnityContainer ioc = new ExtendedUnityContainer(policyInjectionWrapper);
                IocProvider.Initialize(ioc);

                var dbConnectionStringKey = "ZIS_AUTH";
			    var logDbConnectionKey = "ZIS_LOG";
                Database.ConfigureSchema(dbConnectionStringKey, "zis_authentication");
                Database.ConfigureSqlExceptionProcessor(SQLExceptionProcessor.Instance);

                ioc.RegisterTypeAsSingleton<IDbSessionContext, Database.NullContext>();
                ioc.RegisterTypeAsSingleton<IHslLogManager, HslLogManager>()
                    .ConfigureConstructor<HslLogManager>(new SecurityLogMapper(), dbConnectionStringKey);
                ioc.RegisterTypeAsSingleton<IHslLogManager, PerformanceLogManager>(LogCategory.Database)
                    .ConfigureConstructorForRegistration<PerformanceLogManager>(LogCategory.Database, new DatabaseLogMapper(), logDbConnectionKey);
                ioc.RegisterTypeAsSingleton<IHslLogManager, PerformanceLogManager>(LogCategory.ServiceCall)
                    .ConfigureConstructorForRegistration<PerformanceLogManager>(LogCategory.ServiceCall, new ServiceCallLogMapper(), logDbConnectionKey);

                this.queueListener = CreateListener(this, distributorSettings.QueueTimerInterval, distributorSettings.MsmqPath);

				this.ApplicationName = this.ServiceName;
				this.ApplicationName = distributorSettings.ServiceName;
				this.eventLogger.AddMessage(NameTag, this.ApplicationName);

				this.eventLogger.ApplicationName = this.ApplicationName;
				this.eventLogger.AddMessage(Resources.InitializeComponentCompletedMessage, Resources.InitializeComponentCompleted);
			}
			catch (LoggingException loggingException)
			{
				this.eventLogger.LogServiceFailure(
					string.Format(Resources.Culture, Resources.ServiceStartError, this.ApplicationName),
					loggingException,
					TraceEventType.Error);

				throw;
			}
			catch (Exception ex)
			{
				this.eventLogger.LogServiceFailure(
					string.Format(Resources.Culture, Resources.ServiceStartError, this.ApplicationName),
					ex,
					TraceEventType.Error);

				throw new LoggingException(Resources.ErrorInitializingService, ex);
			}
		}
		/// <summary>
		/// Makes sure that target message queue exists and returns a new <see cref="MsmqListener"/>.
		/// </summary>
		/// <param name="distributorService">The distributor service for the listener.</param>
		/// <param name="timerInterval">Interval to check for new messages.</param>
		/// <param name="msmqPath">The name of the queue to get messages from.</param>
		/// <returns>A new msmq listener.</returns>
		protected virtual MsmqListener CreateListener(DistributorService distributorService, int timerInterval, string msmqPath)
		{
            if (!System.Messaging.MessageQueue.Exists(msmqPath))
            {
                throw new LoggingException(string.Format("Message queue {0} does not exist.", msmqPath));
            }

			return new MsmqListener(distributorService, timerInterval, msmqPath);
		}

		/// <summary/>
		/// <exclude/>
		/// <devdoc>
		/// The windows service start event.
		/// </devdoc>
		protected override void OnStart(string[] args)
		{
			if (!initializeComponentsCalled)
			{
				InitializeComponent();
			}

			try
			{
				SanityCheck sanityCheck = new SanityCheck(this);
				sanityCheck.StartCheckTimer();

				if (this.Status == ServiceStatus.OK)
				{
					StartMsmqListener();

					this.eventLogger.LogServiceStarted();
				}
			}
			catch (Exception e)
			{
				this.eventLogger.LogServiceFailure(
					string.Format(Resources.Culture, Resources.ServiceStartError, this.ApplicationName),
					e,
					TraceEventType.Error);

				this.Status = ServiceStatus.Shutdown;
			}
		}

		/// <summary/>
		/// <exclude/>
		/// <devdoc>
		/// The windows service stop event.
		/// </devdoc>
		protected override void OnStop()
		{
			try
			{
				StopMsmqListener();
			}
			catch (Exception e)
			{
				this.eventLogger.LogServiceFailure(
					string.Format(Resources.Culture, Resources.ServiceStopError, this.ApplicationName),
					e,
					TraceEventType.Error);

				this.Status = ServiceStatus.Shutdown;
			}

			GC.Collect();

		}

		/// <summary/>
		/// <exclude/>
		/// <devdoc>
		/// The windows service pause event.
		/// </devdoc>
		protected override void OnPause()
		{
			try
			{
				if (this.queueListener.StopListener())
				{
					this.eventLogger.LogServicePaused();
				}
				else
				{
					this.eventLogger.LogServiceFailure(
						string.Format(Resources.Culture, Resources.ServicePauseWarning, this.ApplicationName),
						null,
						TraceEventType.Warning);
				}
			}
			catch (Exception e)
			{
				this.eventLogger.LogServiceFailure(
					string.Format(Resources.Culture, Resources.ServicePauseError, this.ApplicationName),
					e,
					TraceEventType.Error);

				this.Status = ServiceStatus.Shutdown;
			}
		}

		/// <summary/>
		/// <exclude/>
		/// <devdoc>
		/// The windows service resume event.
		/// </devdoc>
		protected override void OnContinue()
		{
			try
			{
				this.queueListener.StartListener();
				this.eventLogger.LogServiceResumed();
			}
			catch (Exception e)
			{
				this.eventLogger.LogServiceFailure(
					string.Format(Resources.Culture, Resources.ServiceResumeError, this.ApplicationName),
					e,
					TraceEventType.Error);

				this.Status = ServiceStatus.Shutdown;
			}
		}

		private void StartMsmqListener()
		{
			try
			{
				this.eventLogger.AddMessage(Resources.InitializeStartupSequenceStartedMessage, Resources.ValidationStarted);

				this.queueListener.StartListener();

				this.eventLogger.AddMessage(Resources.InitializeStartupSequenceFinishedMessage, Resources.ValidationComplete);
			}
			catch
			{
				this.eventLogger.AddMessage(Resources.InitializeStartupSequenceErrorMessage, Resources.ValidationError);

				this.Status = ServiceStatus.Shutdown;
				throw;
			}
		}

		private void StopMsmqListener()
		{
			if (this.queueListener.StopListener())
			{
				this.eventLogger.LogServiceStopped();
			}
			else
			{
				this.eventLogger.LogServiceFailure(
					string.Format(Resources.Culture, Resources.ServiceStopWarning, this.ApplicationName),
					null,
					TraceEventType.Warning);
			}
		}

		private static IConfigurationSource GetConfigurationSource()
		{
			return ConfigurationSourceFactory.Create();
		}
	}
}