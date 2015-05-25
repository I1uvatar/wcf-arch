using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using AppPlatform.Core.EnterpriseLibrary.ExceptionHandling;
using AppPlatform.Core.EnterpriseLibrary.Services.Context;
using AppPlatform.Core.EnterpriseLibrary.Services.Execution;

namespace AppPlatform.Core.EnterpriseLibrary.Services
{
    /// <summary>
    /// Defines methods for retrieving service proxy
    /// </summary>
    public class ServiceProvider : IRemoteServiceProvider
    {
        protected ExceptionProcessors postHandlers;
        protected List<IMessageHeaderBuilder> messageHeaderBuilders;
        private IActionExecutionWrapper actionExecutionWrapper;

        #region IRemoteServiceProvider Members

        /// <summary>
        /// Get service proxy by endpoint name
        /// </summary>
        /// <typeparam name="TService">Type of service</typeparam>
        /// <param name="endpointConfigurationName">Endpoint configuration name</param>
        /// <param name="processors">The exception processors</param>
        /// <param name="headerBuilders">The header builders.</param>
        /// <param name="executionWrapper">The execution wrapper.</param>
        /// <returns></returns>
        public IRemoteServiceProxy<TService> GetServiceByEndPointName<TService>(
            string endpointConfigurationName, 
            ExceptionProcessors processors, 
            List<IMessageHeaderBuilder> headerBuilders,
            IActionExecutionWrapper executionWrapper)
            where TService : class
        {
            var serviceProxy = new ServiceProxy<TService>(endpointConfigurationName);
            serviceProxy.SetCustomExceptionProcessors(processors ?? this.postHandlers);
            serviceProxy.SetOutgoingMessageHeaderBuilders(headerBuilders ?? this.messageHeaderBuilders);
            //serviceProxy.SetActionExecutionWrapper(executionWrapper);

            return serviceProxy;
        }

        public IRemoteServiceProxy<IContract> GetService<IContract>()
            where IContract : class
        {
            var endpoints = this.GetEndpointConfiguration();
            var candidates = endpoints.Where(e => e.Contract.Equals(typeof(IContract).FullName));

            if (candidates.Count() > 1)
            {
                throw new ApplicationException("Can't obtain endpoint for service since there are multiple endpoints for the same contract type");
            }

            var endpointName = candidates.First().Name;

            var serviceProxy = new ServiceProxy<IContract>(endpointName);
            serviceProxy.SetCustomExceptionProcessors(this.postHandlers);
            serviceProxy.SetOutgoingMessageHeaderBuilders(this.messageHeaderBuilders);
            //serviceProxy.SetActionExecutionWrapper(this.actionExecutionWrapper);

            return serviceProxy;
        }

        public IRemoteServiceProxy<TService> GetServiceByEndPointName<TService>(string endpointConfigurationName)
            where TService : class
        {
            return this.GetServiceByEndPointName<TService>(endpointConfigurationName, null, null, null);
        }

        public IRemoteServiceProxy<TService> GetService<TService>(string metadataExchangeEndpointAddress)
            where TService : class
        {
            return this.GetService<TService>(metadataExchangeEndpointAddress, null, null, null, null);
        }

        public IRemoteServiceProxy<TService> GetService<TService>(string metadataExchangeEndpointAddress, Binding expectedBinding) where TService : class
        {
            return this.GetService<TService>(metadataExchangeEndpointAddress, null, null, null, expectedBinding);
        }

        /// <summary>
        /// Get service proxy at defined mex address
        /// </summary>
        /// <typeparam name="TService">Type of service</typeparam>
        /// <param name="metadataExchangeEndpointAddress">EnAddress of mex endpoint</param>
        /// <param name="processors">The processors.</param>
        /// <param name="headerBuilders">The message header builders.</param>
        /// <param name="executionWrapper">The execution wrapper.</param>
        /// <param name="expectedBinding">The expected binding.</param>
        /// <returns></returns>
        public IRemoteServiceProxy<TService> GetService<TService>(
            string metadataExchangeEndpointAddress, 
            ExceptionProcessors processors, 
            List<IMessageHeaderBuilder> headerBuilders,
            IActionExecutionWrapper executionWrapper,
            Binding expectedBinding
            )
            where TService : class
        {
            var endpointAddress = new EndpointAddress(metadataExchangeEndpointAddress);

            //synchronizationEndpoints[0].Binding.SendTimeout = System.TimeSpan.Parse("00:10:00");

            var binding = new WSHttpBinding(SecurityMode.None);
            binding.MaxReceivedMessageSize = 50000000;
            binding.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;
            var mexClient = new MetadataExchangeClient(binding);

            var description = ContractDescription.GetContract(typeof(TService));

            var synchronizationEndpoints = MetadataResolver.Resolve(new List<ContractDescription> { description }, endpointAddress, mexClient);

            if (synchronizationEndpoints.Count == 0)
            {
                throw new ApplicationException(
                    string.Format(
                        "No endpoints for contract {0} at address {1} found.",
                        typeof(TService),
                        metadataExchangeEndpointAddress
                        )
                    );
            }

            var endpoint = synchronizationEndpoints.Where(ep => (expectedBinding != null && ep.Binding.GetType() == expectedBinding.GetType()) || expectedBinding == null).First();

            if (expectedBinding != null)
            {
                endpoint.Binding = expectedBinding;
            }

            var serviceProxy = new ServiceProxy<TService>(endpoint.Binding, endpoint.Address);
            serviceProxy.SetCustomExceptionProcessors(processors ?? this.postHandlers);
            serviceProxy.SetOutgoingMessageHeaderBuilders(headerBuilders ?? this.messageHeaderBuilders);
            //serviceProxy.SetActionExecutionWrapper(executionWrapper ?? this.actionExecutionWrapper);

            return serviceProxy;
        }

        public void SetCustomExceptionProcessors(ExceptionProcessors processors)
        {
            this.postHandlers = processors;
        }

        public void SetOutgoingMessageHeaderBuilders(List<IMessageHeaderBuilder> headerBuilders)
        {
            this.messageHeaderBuilders = headerBuilders;
        }

        public void SetActionExecutionWrapper(IActionExecutionWrapper executionWrapper)
        {
            this.actionExecutionWrapper = executionWrapper;
        }

        #endregion

        private IEnumerable<ChannelEndpointElement> GetEndpointConfiguration()
        {
            var endpoints = ((ClientSection)ConfigurationManager.GetSection("system.serviceModel/client")).Endpoints;
            return endpoints.Cast<ChannelEndpointElement>();
        }
    }
}