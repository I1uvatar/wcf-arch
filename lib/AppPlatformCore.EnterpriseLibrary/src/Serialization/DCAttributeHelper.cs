using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace AppPlatform.Core.EnterpriseLibrary.Serialization
{
    /// <summary>
    /// <see cref="DataContractAttribute"/> helper
    /// </summary>
    public class DCAttributeHelper
    {
        /// <summary>
        /// Gets the data contract attribute full name.
        /// </summary>
        /// <param name="contract">The contract.</param>
        /// <returns></returns>
        public static string GetDataContractAttributelName(Type contract)
        {
            return GetDataContractAttributelName(contract, true);
        }

        public static string GetDataContractAttributelName(Type contract, bool assureIsDataContract)
        {
            var dcAttributes = contract.GetCustomAttributes(typeof(DataContractAttribute), false);

            var dcAttribute = dcAttributes.FirstOrDefault(attr => attr is DataContractAttribute) as DataContractAttribute;

            if (dcAttribute == null)
            {
                if (assureIsDataContract)
                {
                    throw new InvalidOperationException(string.Format("Type {0} is not marked with DataContract attribute", contract.FullName));
                }
                return null;

            }

            return dcAttribute.Name;
        }

        /// <summary>
        /// Gets the data contract attributel.
        /// </summary>
        /// <param name="contract">The contract.</param>
        /// <returns></returns>
        public static DataContractAttribute GetDataContractAttribute(Type contract)
        {
            var dcAttributes = contract.GetCustomAttributes(typeof(DataContractAttribute), false);

            return dcAttributes.FirstOrDefault(attr => attr is DataContractAttribute) as DataContractAttribute;
        }

        /// <summary>
        /// Gets the data contract attributel.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public static DataMemberAttribute GetDatMemberAttribute(PropertyInfo property)
        {
            var dcAttributes = property.GetCustomAttributes(typeof(DataMemberAttribute), false);

            return dcAttributes.FirstOrDefault(attr => attr is DataMemberAttribute) as DataMemberAttribute;
        }
    }
}
