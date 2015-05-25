using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace AppPlatform.Core.EnterpriseLibrary.Serialization
{
    /// <summary>
    /// Helper class for XML serialization
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Serializes input data.
        /// </summary>
        /// <typeparam name="InputType">Type of the object to be serialized</typeparam>
        /// <param name="data">Object to be serialized</param>
        /// <returns>Serialized data</returns>
        public static XDocument Serialize<InputType>(InputType data)
        {
            return Serialize(data, new XmlReaderSettings());
        }

        public static XDocument Serialize<InputType>(InputType data, XmlReaderSettings xmlReaderSettings)
        {
            var aSerializer = new XmlSerializer(typeof(InputType));

            using (var tmpStream = new MemoryStream())
            {
                aSerializer.Serialize(tmpStream, data);
                
                tmpStream.Position = 0;
                using (var reader = XmlReader.Create(tmpStream, xmlReaderSettings))
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
            var aSerializer = new XmlSerializer(typeof(InputType));

            using (var tmpStream = new MemoryStream())
            {
                aSerializer.Serialize(tmpStream, data, namespaceList);
                
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
        /// <param name="inputData">XmlElement to be serialized</param>
        /// <returns>Deserialized object</returns>
        public static OutputType Deserialize<OutputType>(XDocument inputData)
        {
            var aSerializer = new XmlSerializer(typeof(OutputType));

            using (var tmpStream = new MemoryStream())
            {
                using (var awriter = XmlWriter.Create(tmpStream))
                {
                    inputData.WriteTo(awriter);
                    awriter.Flush();
                }

                tmpStream.Position = 0;
                var anObject = (OutputType) aSerializer.Deserialize(tmpStream);

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
            var aSerializer = new XmlSerializer(typeof(OutputType));

            using (var reader = new StringReader(inputData))
            {
                return (OutputType)aSerializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Serialize given object into XmlElement.
        /// </summary>
        /// <param name="transformObject">Input object for serialization.</param>
        /// <returns>Returns serialized XmlElement.</returns>
        public static XmlElement SerializeToXmlElement(object transformObject)
        {
            XmlElement serializedElement = null;
            try
            {                
                var serializer = new XmlSerializer(transformObject.GetType());

                var xmlDoc = new XmlDocument();
                using (var memStream = new MemoryStream())
                {
                    serializer.Serialize(memStream, transformObject);
                    memStream.Position = 0;                    
                    xmlDoc.Load(memStream);                    
                }

                serializedElement = xmlDoc.DocumentElement;                
            }
            catch (Exception)
            {
                return null;
            }
            return serializedElement;
        }

        /// <summary>
        /// Deserialize given XmlElement into object.
        /// </summary>
        /// <param name="xmlElement">xmlElement to deserialize.</param>
        /// <param name="type">Type of resultant deserialized object.</param>
        /// <returns>Returns deserialized object.</returns>
        public static object DeserializeFromXmlElement(XmlElement xmlElement, Type type)
        {
            object transformedObject;
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(xmlElement.OuterXml);
                using (var memStream = new MemoryStream(buffer))
                {
                    var serializer = new XmlSerializer(type);
                    
                    memStream.Position = 0;
                    transformedObject = serializer.Deserialize(memStream);
                }
            }
            catch (Exception)
            {
                return null;
            }
            return transformedObject;
        }
    }
}