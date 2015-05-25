using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace AppPlatform.Core.EnterpriseLibrary.Serialization
{
    /// <summary>
    /// Helper class for serialization using <see cref="DataContractSerializer"/>
    /// </summary>
    public static class DCSerializer
    {
        /// <summary>
        /// Serializes input data.
        /// </summary>
        /// <typeparam name="InputType">Type of the object to be serialized</typeparam>
        /// <param name="data">Object to be serialized</param>
        /// <returns>Serialized data</returns>
        public static XDocument Serialize<InputType>(InputType data)
        {
            var aSerializer = new DataContractSerializer(typeof(InputType));

            using (var tmpStream = new MemoryStream())
            {
                aSerializer.WriteObject(tmpStream, data);
                tmpStream.Position = 0;
                
                using (var reader = XmlReader.Create(tmpStream))
                {
                    return XDocument.Load(reader);    
                }                
            }
        }

        /// <summary>
        /// Serializes input data.
        /// </summary>
        /// <typeparam name="InputType">Type of the object to be serialized</typeparam>
        /// <param name="data">Object to be serialized</param>
        /// <param name="namespaceList">Namespace specification</param>
        /// <returns>Serialized data</returns>
        public static XDocument Serialize<InputType>(InputType data, XmlSerializerNamespaces namespaceList)
        {
            var aSerializer = new DataContractSerializer(typeof(InputType));

            using (var tmpStream = new MemoryStream())
            {
                aSerializer.WriteObject(tmpStream, data);
                tmpStream.Position = 0;
                
                using (var reader = XmlReader.Create(tmpStream))
                {
                    return XDocument.Load(reader);
                }
            }
        }

        /// <summary>
        /// Serializes input data.
        /// </summary>
        /// <typeparam name="InputType">Type of the object to be serialized</typeparam>
        /// <param name="data">Object to be serialized</param>
        /// <returns>Serialized string data</returns>
        public static string SerializeToString<InputType>(InputType data)
        {
            return Serialize(data).ToString();
        }

        /// <summary>
        /// Serializes input data.
        /// </summary>
        /// <typeparam name="InputType">Type of the object to be serialized</typeparam>
        /// <param name="data">Object to be serialized</param>
        /// <param name="namespaceList">Namespace specification</param>
        /// <returns>Serialized string data</returns>
        public static string SerializeToString<InputType>(InputType data, XmlSerializerNamespaces namespaceList)
        {
            return Serialize(data, namespaceList).ToString();
        }

        /// <summary>
        /// Deserializes input XDocument
        /// </summary>
        /// <typeparam name="OutputType">Type of the object to be deserialized</typeparam>
        /// <param name="inputData">The input data.</param>
        /// <returns>Deserialized object</returns>
        public static OutputType Deserialize<OutputType>(XNode inputData)
        {
            var aSerializer = new DataContractSerializer(typeof(OutputType));
            using (var reader = inputData.CreateReader())
            {
                var anObject = (OutputType)aSerializer.ReadObject(reader);
                return anObject;
            }
        }

        /// <summary>
        /// Deserialize input xml string
        /// </summary>
        /// <typeparam name="OutputType"></typeparam>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static OutputType Deserialize<OutputType>(string inputData)
        {
            var aSerializer = new DataContractSerializer(typeof(OutputType));

            using (var str = new StringReader(inputData))
            {
                using (var reader = XmlReader.Create(str))
                {
                    return (OutputType)aSerializer.ReadObject(reader);
                }
            }
        }
    }
}