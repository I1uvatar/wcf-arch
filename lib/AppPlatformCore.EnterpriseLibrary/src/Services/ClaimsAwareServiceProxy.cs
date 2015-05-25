using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using AppPlatform.Core.EnterpriseLibrary.ExceptionHandling;
using AppPlatform.Core.EnterpriseLibrary.Services.Context;
using AppPlatform.Core.EnterpriseLibrary.Services.IdentityModel;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace AppPlatform.Core.EnterpriseLibrary.Services
{
    public class ClaimsAwareServiceProxy
    {
        protected static ITokenProvider tokenProvider;
        protected static ITokenCustomRequestProvider customRequestProvider;

        public static void Initialize(ITokenProvider provider, ITokenCustomRequestProvider customProvider)
        {
            tokenProvider = provider;
            customRequestProvider = customProvider;
        }
    }

    public class ClaimsAwareServiceProxy<IServiceContract> : ClaimsAwareServiceProxy, IRemoteServiceProxy<IServiceContract>
        where IServiceContract : class
    {
        private const string DefaultPolicy = "Default Policy";

        protected Func<Exception, string, bool> exceptionHandler = (ex, exceptionPolicy) => ExceptionPolicy.HandleException(ex, exceptionPolicy);

        protected ExceptionProcessors postHandlers;
        protected List<IMessageHeaderBuilder> messageHeaderBuilders;

        private readonly ChannelFactory<IServiceContract> channelFactory;
        private IServiceContract contract;

        #region .ctor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ClaimsAwareServiceProxy() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        public ClaimsAwareServiceProxy(string endpointConfigurationName)
        {
            this.channelFactory = new ChannelFactory<IServiceContract>(endpointConfigurationName);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="address"></param>
        /// <param name="binding"></param>
        public ClaimsAwareServiceProxy(Binding binding, EndpointAddress address)
        {
            this.channelFactory = new ChannelFactory<IServiceContract>(binding, address);
        }

        #endregion

        #region IRemoteServiceProxy<IServiceContract> Members

        public ClientCredentials ClientCredentials
        {
            get { return this.channelFactory.Credentials; }
        }

        /// <summary>
        /// Gets the state of the communication object.
        /// </summary>
        public CommunicationState CommunicationState
        {
            get
            {
                var commObject = this.GetUnderlyingCommObject();

                if (commObject == null)
                {
                    throw new InvalidOperationException("Communication object is null");
                }

                return commObject.State;
            }
        }

        public IServiceContract Contract
        {
            get
            {
                if (this.contract == null)
                {
                    if (tokenProvider != null)
                    {
                        var token = tokenProvider.GetToken(this.channelFactory.Endpoint, customRequestProvider);

                        //token obtained, create channel with explicit token
                        if (token != null)
                        {
                            FederatedClientCredentials.ConfigureChannelFactory(this.channelFactory);

                            this.contract = this.channelFactory.CreateChannelWithIssuedToken(token);
                        }
                        else
                        {
                            this.contract = this.channelFactory.CreateChannel();
                            CustomRequestConfigHelper.ConfigureFederationBinding(this.channelFactory.Endpoint.Binding, customRequestProvider);
                        }
                    }
                    else
                    {
                        this.contract = this.channelFactory.CreateChannel();
                        CustomRequestConfigHelper.ConfigureFederationBinding(this.channelFactory.Endpoint.Binding, customRequestProvider);
                    }
                }

                return this.contract;
            }
        }

        public bool ExecuteAndRelease(Action<IServiceContract> contractAction)
        {
            return this.ExecuteAndRelease(contractAction != null ? () => contractAction(this.Contract) : (Execute) null);
        }

        public bool ExecuteAndRelease(Action<IServiceContract> contractAction, string exceptionPolicy)
        {
            return this.ExecuteAndRelease(contractAction != null ? () => contractAction(this.Contract) : (Execute) null, exceptionPolicy);
        }

        public bool ExecuteAndRelease(Execute pendingCommand)
        {
            return this.ExecuteAndRelease(pendingCommand, DefaultPolicy);
        }

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
                var commObject = this.GetUnderlyingCommObject();

                if (commObject != null && commObject.State == CommunicationState.Opened)
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

        #endregion

        private ICommunicationObject GetUnderlyingCommObject()
        {
            return this.contract as ICommunicationObject;
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

            using (var scope = new OperationContextScope(this.Contract as IContextChannel))
            {
                foreach (var header in headersToAppend)
                {
                    OperationContext.Current.OutgoingMessageHeaders.Add(header);
                }

                pendingCommand();

                hasCommandCompletedSuccessfully = true;

                this.GetUnderlyingCommObject().Close();
            }
        }

        private void Close()
        {
            var communicationObject = this.GetUnderlyingCommObject();

            if (communicationObject == null)
            {
                return;
            }

            if (communicationObject.State == CommunicationState.Opened)
            {
                communicationObject.Close();
            }
        }

        private void Abort()
        {
            var communicationObject = this.GetUnderlyingCommObject();

            if (communicationObject == null)
            {
                return;
            }

            communicationObject.Abort();
        }

        public virtual bool HandleException(Exception ex, string exceptionPolicy)
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

    }
}
