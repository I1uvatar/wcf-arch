using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace AppPlatform.Core.EnterpriseLibrary.Services.PolicyInjection
{
    public class PolicyInjectionBehavior : IEndpointBehavior, IServiceBehavior
    {
        public void Validate(ServiceEndpoint endpoint)
        { }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        { }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            Type contractType = endpoint.Contract.ContractType;
            endpointDispatcher.DispatchRuntime.InstanceProvider = new
              PolicyInjectionInstanceProvider(contractType);
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        { }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        { }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        { }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher channelDispatch in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher endpointDispatch in channelDispatch.Endpoints)
                {
                    var endpointDispatchTmp = endpointDispatch;
                    var relatedEndpoint = serviceDescription.Endpoints.Where(e => e.Contract.Name == endpointDispatchTmp.ContractName).FirstOrDefault();

                    endpointDispatch.DispatchRuntime.InstanceProvider = new PolicyInjectionInstanceProvider(relatedEndpoint != null ? relatedEndpoint.Contract.ContractType : null);
                }
            }
        }
    }
}
