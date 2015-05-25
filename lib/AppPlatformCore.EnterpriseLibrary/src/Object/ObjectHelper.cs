using System;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Linq;
using AppPlatform.Core.EnterpriseLibrary.Diagnostics;

namespace AppPlatform.Core.EnterpriseLibrary.Object
{
    /// <summary>
    /// Object helper
    /// </summary>
    public class ObjectHelper
    {
        private const string TranIDCol = "TranID";
        private const string TranIDDbCol = "jnTranID";

        /// <summary>
        /// Creates data object from datarow, uses property names for identifying row collumns
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T CreateFromDataRow<T>(DataRow row)
        {
            if (row == null)
            {
                return default(T);
            }

            Type dataType = typeof(T);
            var data = (T)System.Activator.CreateInstance(dataType);

            foreach (var property in dataType.GetProperties())
            {
                if (property.Name == TranIDCol && row[TranIDDbCol] != DBNull.Value)
                {
                    property.SetValue(data, row["jnTranID"], null);
                    continue;
                }

                if (row[property.Name] == DBNull.Value && !IsNullableType(property.PropertyType) && property.PropertyType != typeof(string))
                {
                    throw new ApplicationException(string.Format("Property {0} can't contains null value", property.Name));
                }

                property.SetValue(data, row[property.Name] != DBNull.Value ? row[property.Name] : null, null);
            }

            return data;
        }

        public static bool IsSomeKindOfXml(Type type)
        {
            return (type == typeof(XmlDocument)
                    || type == typeof(XDocument)
                    || type == typeof(XmlElement)
                    || type == typeof(XElement));
        }

        private static bool IsNullableType(Type theType)
        {
            return (theType.IsGenericType && theType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
        }

        public static string GetXmlString(object xmlData)
        {
            if (xmlData == null)
            {
                return null;
            }

            var type = xmlData.GetType();

            if (!IsSomeKindOfXml(type))
            {
                return null;
            }

            if (type == typeof(XmlDocument) || type == typeof(XmlElement))
            {
                return (xmlData as XmlNode).ToString();
            }

            if (type == typeof(XDocument) || type == typeof(XElement))
            {
                return (xmlData as XNode).ToString();
            }

            return null;
        }

        public static MemoryStream ConvertToStream(object data)
        {
            if (Safe.IsNull(data))
            {
                return null;
            }

            var stream = new MemoryStream();
            var formater = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));

            formater.Serialize(stream, data);
            stream.Position = 0;

            return stream;
        }

        public static T ConvertFromStream<T>(MemoryStream stream)
            where T : class
        {
            var formater = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));

            stream.Position = 0;
            var data = formater.Deserialize(stream);

            if (data is T)
            {
                return data as T;
            }

            throw new InvalidDataException(String.Format("Stream doesn't contans data of type {0}", typeof (T).FullName));
        }
    }
}
