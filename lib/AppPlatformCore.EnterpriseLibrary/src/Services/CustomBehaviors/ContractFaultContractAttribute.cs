using System;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace AppPlatform.Core.EnterpriseLibrary.Services.CustomBehaviors
{
    public class ContractFaultContractAttribute : Attribute, IContractBehavior
    {
        private readonly Type[] faultTypes;

        public ContractFaultContractAttribute(params Type[] faultTypes)
        {
            this.faultTypes = faultTypes;
        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        { }

        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
        { }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        { }

        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            foreach (var operation in contractDescription.Operations)
            {
                foreach (var type in faultTypes)
                {
                    var typeTemp = type;
                    if (operation.Faults.Count(fault => fault.DetailType == typeTemp) == 0)
                    {
                        var decs = new FaultDescription(string.Concat("urn:", endpoint.Contract.Name, operation.Name))
                        {
                            DetailType = typeTemp,
                            Name = string.Concat(typeTemp.Name, "Fault"),
                            Namespace = endpoint.Contract.Namespace
                        };

                        operation.Faults.Add(decs);
                    }
                }
            }
        }
    }
}
