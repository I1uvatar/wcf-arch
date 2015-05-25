using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace AppPlatform.Core.EnterpriseLibrary.Services.CustomBehaviors
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceFaultContractAttribute : Attribute, IServiceBehavior
    {
        private readonly Type[] faultTypes;

        public ServiceFaultContractAttribute(params Type[] faultTypes)
        {
            this.faultTypes = faultTypes;
        }

        public ServiceFaultContractAttribute(Type faultType)
        {
            this.faultTypes = new[] { faultType };
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            //var type = Type.GetType("System.ServiceModel.Description.TypeLoader, System.ServiceModel, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

            //var constructor = type.GetConstructor(new Type[0]);
            //var typeLoader = constructor.Invoke(null);

            //var method = type.GetMethod("CreateFaultDescription", BindingFlags.Instance | BindingFlags.NonPublic);

            //var xmlNameType = Type.GetType("System.ServiceModel.Description.XmlName, System.ServiceModel, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

            //var xmlNameConstructor = xmlNameType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(string) }, null);

            foreach (var endpoint in serviceDescription.Endpoints)
            {
                foreach (var operation in endpoint.Contract.Operations)
                {
                    //if (operation.Faults.Count(fault => fault.DetailType == this.faultType) != 0)
                    //{
                    //    operation.Faults.Clear();
                    //}

                    //var faultDescription = (FaultDescription)method.Invoke(
                    //        typeLoader,
                    //        new[]
                    //            {
                    //                new FaultContractAttribute(this.faultType), 
                    //                new XmlQualifiedName(operation.DeclaringContract.Name, operation.DeclaringContract.Namespace), 
                    //                operation.DeclaringContract.Namespace, 
                    //                xmlNameConstructor.Invoke(new[] {operation.Name})
                    //            });

                    //    operation.Faults.Add(faultDescription);

                    foreach (var faultType in faultTypes)
                    {
                        var faultTypeTemp = faultType;

                        if (operation.Faults.Count(fault => fault.DetailType == faultTypeTemp) == 0)
                        {
                            var decs = new FaultDescription(string.Concat("urn:", endpoint.Contract.Name, operation.Name))
                                           {
                                               DetailType = faultTypeTemp,
                                               Name = string.Concat(faultTypeTemp.Name, "Fault"),
                                               Namespace = endpoint.Contract.Namespace
                                           };

                            operation.Faults.Add(decs);
                        }
                    }
                }
            }
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        { }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        { }

        private FaultDescription MakeFault(Type detailType, OperationDescription operation)
        {
            string action = detailType.Name;
            DescriptionAttribute description = (DescriptionAttribute)Attribute.GetCustomAttribute(detailType, typeof(DescriptionAttribute));
            if (description != null)
                action = description.Description;

            action = "aaaa";

            FaultDescription fd = new FaultDescription(action);
            fd.DetailType = detailType;
            fd.Name = "ServiceFaultFault";
            return fd;
        }
    }
}
