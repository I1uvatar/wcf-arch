using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace AppPlatform.Core.EnterpriseLibrary.Services.PolicyInjection
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PolicyInjectionBehaviorAttribute : Attribute,
      IContractBehavior, IContractBehaviorAttribute
    {
        public PolicyInjectionBehaviorAttribute() { }

        public Type TargetContract
        {
            get
            {
                return null; //return null so we apply to all contracts
            }
        }

        public void AddBindingParameters(ContractDescription description,
          ServiceEndpoint endpoint,
          BindingParameterCollection parameters) { }

        public void ApplyClientBehavior(ContractDescription description,
          ServiceEndpoint endpoint,
          ClientRuntime clientRuntime) { }

        public void ApplyDispatchBehavior(ContractDescription description,
          ServiceEndpoint endpoint,
          DispatchRuntime dispatch)
        {
            Type contractType = description.ContractType;
            dispatch.InstanceProvider = new
              PolicyInjectionInstanceProvider(contractType);
        }

        public void Validate(ContractDescription description,
          ServiceEndpoint endpoint) { }
    }

}
