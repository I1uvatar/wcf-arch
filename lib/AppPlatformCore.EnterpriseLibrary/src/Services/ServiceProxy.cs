using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using AppPlatform.Core.EnterpriseLibrary.ExceptionHandling;
using AppPlatform.Core.EnterpriseLibrary.Services.Context;
using AppPlatform.Core.EnterpriseLibrary.Services.Execution;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace AppPlatform.Core.EnterpriseLibrary.Services
{
    public class ServiceProxy<IServiceContract> : ClientBase<IServiceContract>, IRemoteServiceProxy<IServiceContract>
        where IServiceContract : class
    {
        private const string DefaultPolicy = "Default Policy";

        protected Func<Exception, string, bool> exceptionHandler = (ex, exceptionPolicy) => ExceptionPolicy.HandleException(ex, exceptionPolicy);

        protected ExceptionProcessors postHandlers;
        protected List<IMessageHeaderBuilder> messageHeaderBuilders;

        #region IRemoteServiceProxy<IServiceContract> Members

        #region .ctor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ServiceProxy() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        public ServiceProxy(string endpointConfigurationName) : base(endpointConfigurationName) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="address"></param>
        /// <param name="binding"></param>
        public ServiceProxy(Binding binding, EndpointAddress address) : base(binding, address) { }

        #endregion

        #region IRemoteServiceProxy

        /// <summary>
        /// Get the remote service contract implementation.
        /// </summary>
        public IServiceContract Contract
        {
            get { return this.Channel; }
        }

        /// <summary>
        /// Gets the state of the communication object.
        /// </summary>
        public CommunicationState CommunicationState
        {
            get { return this.State; }
        }

        /// <summary>
        /// "Safe" execute of the remote call sequence. Default exception policy ("Default Policy" configured 
        /// in configiration file) is used.
        /// It mimics the "using" pattern.
        /// Because it is advised not to use the using pattern we have our own implementation.
        /// See http://msdn2.microsoft.com/en-us/library/aa355056.aspx
        /// </summary>
        /// <param name="pendingCommand">The command to execute.</param>
        /// <returns>True if command is successfully executed, false otherwise.</returns>
        public bool ExecuteAndRelease(Execute pendingCommand)
        {
            return ExecuteAndRelease(pendingCommand, DefaultPolicy);
        }

        /// <summary>
        /// "Safe" execute of the remote call sequence. 
        /// Default exception policy ("Default Policy" configured in configiration file) is used.
        /// It mimics the "using" pattern.
        /// Because it is advised not to use the using pattern we have our own implementation.
        /// See http://msdn2.microsoft.com/en-us/library/aa355056.aspx
        /// </summary>
        /// <param name="contractAction">The contract action.</param>
        /// <returns> True if command is successfully executed, false otherwise. </returns>
        public bool ExecuteAndRelease(Action<IServiceContract> contractAction)
        {
            return this.ExecuteAndRelease(contractAction != null ? () => contractAction(this.Contract) : (Execute)null);
        }

        /// <summary>
        /// "Safe" execute of the remote call sequence using the specified exception policy.
        /// It mimics the "using" pattern.
        /// Because it is advised not to use the using pattern we have our own implementation.
        /// See http://msdn2.microsoft.com/en-us/library/aa355056.aspx
        /// </summary>
        /// <param name="contractAction">The contract action.</param>
        /// <param name="exceptionPolicy">The exception policy.</param>
        /// <returns>True if command is successfully executed, false otherwise.</returns>
        public bool ExecuteAndRelease(Action<IServiceContract> contractAction, string exceptionPolicy)
        {
            return this.ExecuteAndRelease(contractAction != null ? () => contractAction(this.Contract) : (Execute)null, exceptionPolicy);
        }

        /// <summary>
        /// "Safe" execute of the remote call sequence using the specified exception policy.
        /// It mimics the "using" pattern.
        /// Because it is advised not to use the using pattern we have our own implementation.
        /// See http://msdn2.microsoft.com/en-us/library/aa355056.aspx
        /// </summary>
        /// <param name="pendingCommand">The command to execute.</param>
        /// <param name="exceptionPolicy">The exception policy.</param>
        /// <returns>True if command is successfully executed, false otherwise.</returns>
        public bool ExecuteAndRelease(Execute pendingCommand, string exceptionPolicy)
        {
            if (string.IsNullOrEmpty(exceptionPolicy))
            {
                exceptionPolicy = DefaultPolicy;
            }

            var hasCommandCompletedSuccessfully = false;
            try
            {
                if (this.messageHeaderBuilders != null && this.messageHeaderBuilders.Count != 0)
                {
                    this.AppendHeadersAndExecuteCommand(pendingCommand, ref hasCommandCompletedSuccessfully);
                }
                else
                {
                    pendingCommand();

                    hasCommandCompletedSuccessfully = true;
                    this.Close();
                }
            }
            catch (FaultException ex)
            {
                if (this.State == CommunicationState.Opened)
                {
                    this.Close();
                }
                else
                {
                    this.Abort();
                }

                if (this.HandleException(ex, exceptionPolicy)) throw;
            }
            catch (CommunicationException ex)
            {
                this.Abort();
                if (!hasCommandCompletedSuccessfully && this.HandleException(ex, exceptionPolicy)) throw;
            }
            catch (TimeoutException ex)
            {
                this.Abort();
                if (!hasCommandCompletedSuccessfully && this.HandleException(ex, exceptionPolicy)) throw;
            }
            catch (Exception ex)
            {
                this.Abort();
                if (this.HandleException(ex, exceptionPolicy)) throw;
            }

            return hasCommandCompletedSuccessfully;
        }

        private void AppendHeadersAndExecuteCommand(Execute pendingCommand, ref bool hasCommandCompletedSuccessfully)
        {
            var headersToAppend = new List<MessageHeader>();

            foreach (var builder in messageHeaderBuilders)
            {
                var header = builder.GetHeaderToAppend();

                if (header != null)
                {
                    headersToAppend.Add(header);
                }
            }

            using (var scope = new OperationContextScope(this.InnerChannel))
            {
                foreach (var header in headersToAppend)
                {
                    OperationContext.Current.OutgoingMessageHeaders.Add(header);
                }

                pendingCommand();

                hasCommandCompletedSuccessfully = true;
                this.Close();
            }
        }

        /// <summary>
        /// Sets a custom exception handling policy. If not set, EnterpriseLibrary ExceptionHandling block is used. 
        /// If <para>policy</para> returns true, the exception should be rethrown
        /// </summary>
        /// <param name="policy"></param>
        public void SetExceptionHandlingPolicy(Func<Exception, string, bool> policy)
        {
            this.exceptionHandler = policy;
        }

        public void SetCustomExceptionProcessors(ExceptionProcessors processors)
        {
            this.postHandlers = processors;
        }

        public void SetOutgoingMessageHeaderBuilders(List<IMessageHeaderBuilder> headerBuilders)
        {
            this.messageHeaderBuilders = headerBuilders;
        }

        //public void SetActionExecutionWrapper(IActionExecutionWrapper executionWrapper)
        //{
        //    this.actionExecutionWrapper = executionWrapper;
        //}

        public

        #endregion

        virtual bool HandleException(Exception ex, string exceptionPolicy)
        {
            if (this.exceptionHandler(ex, exceptionPolicy))
            {
                if (this.postHandlers != null)
                {
                    this.postHandlers.ProcessException(ex, true);
                }

                return true;
            }
            return false;
        }

        #endregion
    }
}