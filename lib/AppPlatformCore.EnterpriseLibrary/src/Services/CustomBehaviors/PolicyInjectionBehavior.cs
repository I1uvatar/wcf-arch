using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using AppPlatform.Core.EnterpriseLibrary.Services.PolicyInjection;

namespace AppPlatform.Core.EnterpriseLibrary.Services.CustomBehaviors
{
    public class PolicyInjectionEndpointBehavior : BehaviorExtensionElement,
        IEndpointBehavior
    {

        public override Type BehaviorType
        {
            get { return typeof(PolicyInjectionEndpointBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new PolicyInjectionEndpointBehavior();
        }

        public void Validate(ServiceEndpoint endpoint)
        { }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        { }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint,
            EndpointDispatcher endpointDispatcher)
        {
            Type contractType = endpoint.Contract.ContractType;
            endpointDispatcher.DispatchRuntime.InstanceProvider = new
                PolicyInjectionInstanceProvider(contractType);
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        { }
    }

}
