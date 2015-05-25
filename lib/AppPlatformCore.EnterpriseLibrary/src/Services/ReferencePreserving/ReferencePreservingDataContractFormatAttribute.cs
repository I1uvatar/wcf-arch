using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace AppPlatform.Core.EnterpriseLibrary.Services.ReferencePreserving
{
    public class ReferencePreservingDataContractFormatAttribute : Attribute, IOperationBehavior
    {

        #region IOperationBehavior Members

        public void AddBindingParameters(OperationDescription description, BindingParameterCollection parameters)
        {

        }

        public void ApplyClientBehavior(OperationDescription description, System.ServiceModel.Dispatcher.ClientOperation proxy)
        {
            IOperationBehavior innerBehavior = new ReferencePreservingDataContractSerializerOperationBehavior(description);
            innerBehavior.ApplyClientBehavior(description, proxy);
        }

        public void ApplyDispatchBehavior(OperationDescription description, System.ServiceModel.Dispatcher.DispatchOperation dispatch)
        {
            IOperationBehavior innerBehavior = new ReferencePreservingDataContractSerializerOperationBehavior(description);
            innerBehavior.ApplyDispatchBehavior(description, dispatch);
        }
        public void Validate(OperationDescription description)
        {

        }



        #endregion

    }

 

}
