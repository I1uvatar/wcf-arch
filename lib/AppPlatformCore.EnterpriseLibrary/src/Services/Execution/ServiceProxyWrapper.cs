using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using AppPlatform.Core.EnterpriseLibrary.ExceptionHandling;
using AppPlatform.Core.EnterpriseLibrary.Services.Context;

namespace AppPlatform.Core.EnterpriseLibrary.Services.Execution
{
    public class ServiceProxyWrapper<IContract> : IRemoteServiceProxy<IContract>
    {
        private readonly Func<IRemoteServiceProxy<IContract>> instanceCreator;
        private readonly IActionExecutionWrapper executionWrapper;

        private IRemoteServiceProxy<IContract> proxy;

        public ServiceProxyWrapper(Func<IRemoteServiceProxy<IContract>> instanceCreator, IActionExecutionWrapper executionWrapper)
        {
            this.instanceCreator = instanceCreator;
            this.executionWrapper = executionWrapper;

            this.proxy = instanceCreator();
        }

        public IContract Contract
        {
            get { return this.proxy.Contract; }
        }

        public ClientCredentials ClientCredentials
        {
            get { return this.proxy.ClientCredentials; }
        }

        /// <summary>
        /// Gets the state of the communication object.
        /// </summary>
        public CommunicationState CommunicationState
        {
            get { return this.proxy.CommunicationState; }
        }

        public bool ExecuteAndRelease(Action<IContract> contractAction)
        {
            return this.ExecuteAndRelease(contractAction != null ? () => contractAction(this.Contract) : (Execute)null);
        }

        public bool ExecuteAndRelease(Action<IContract> contractAction, string exceptionPolicy)
        {
            return this.ExecuteAndRelease(contractAction != null ? () => contractAction(this.Contract) : (Execute)null, exceptionPolicy);
        }

        public bool ExecuteAndRelease(Execute pendingCommand)
        {
            return this.ExecuteAndRelease(pendingCommand, null);
        }

        public bool ExecuteAndRelease(Execute pendingCommand, string exceptionPolicy)
        {
            var hasCompletedSucessfully = false;

            Execute action = () =>
                                 {
                                     //if (this.proxy == null) //|| this.proxy.CommunicationState != CommunicationState.Opened)
                                     {
                                         this.proxy = this.instanceCreator();
                                     }

                                     hasCompletedSucessfully = this.proxy.ExecuteAndRelease(pendingCommand, exceptionPolicy);
                                 };

            if (this.executionWrapper != null)
            {
                this.executionWrapper.Execute(action);
            }
            else
            {
                action();
            }

            return hasCompletedSucessfully;
        }

        public void SetExceptionHandlingPolicy(Func<Exception, string, bool> policy)
        {
            this.proxy.SetExceptionHandlingPolicy(policy);
        }

        public void SetCustomExceptionProcessors(ExceptionProcessors processors)
        {
            this.proxy.SetCustomExceptionProcessors(processors);
        }

        public void SetOutgoingMessageHeaderBuilders(List<IMessageHeaderBuilder> headerBuilders)
        {
            this.proxy.SetOutgoingMessageHeaderBuilders(headerBuilders);
        }

        //public void SetActionExecutionWrapper(IActionExecutionWrapper actionExecutionWrapper)
        //{
        //    this.proxy.SetActionExecutionWrapper(actionExecutionWrapper);
        //}
    }
}
