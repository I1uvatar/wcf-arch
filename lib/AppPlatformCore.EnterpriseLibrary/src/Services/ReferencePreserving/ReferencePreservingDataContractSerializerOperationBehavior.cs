using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel.Description;
using System.Xml;

namespace AppPlatform.Core.EnterpriseLibrary.Services.ReferencePreserving
{
    public class ReferencePreservingDataContractSerializerOperationBehavior
        : DataContractSerializerOperationBehavior
    {
        public ReferencePreservingDataContractSerializerOperationBehavior(
            OperationDescription operationDescription)
            : base(operationDescription) { }

        public override XmlObjectSerializer CreateSerializer(
            Type type, string name, string ns, IList<Type> knownTypes)
        {
            return CreateDataContractSerializer(type, name, ns, knownTypes);
        }

        private static XmlObjectSerializer CreateDataContractSerializer(
            Type type, string name, string ns, IList<Type> knownTypes)
        {
            return CreateDataContractSerializer(type, name, ns, knownTypes);
        }

        public static void SetDataContractSerializerBehavior(ContractDescription contractDescription)
        {
            foreach (OperationDescription operation in contractDescription.Operations)
            {
                DataContractSerializerOperationBehavior dcsob = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (dcsob != null)
                {
                    operation.Behaviors.Remove(dcsob);
                }
                operation.Behaviors.Add(new ReferencePreservingDataContractSerializerOperationBehavior(operation));
            }
        }

        public override XmlObjectSerializer CreateSerializer(
            Type type, XmlDictionaryString name, XmlDictionaryString ns,
            IList<Type> knownTypes)
        {
            return new DataContractSerializer(type, name, ns, knownTypes,
                                              0x7FFF /*maxItemsInObjectGraph*/,
                                              false/*ignoreExtensionDataObject*/,
                                              true/*preserveObjectReferences*/,
                                              null/*dataContractSurrogate*/);
        }
    }
}