using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace AppPlatform.Core.EnterpriseLibrary.Xml
{
    /// <summary>
    /// Utilities for Xml file manipulation
    /// </summary>
    public static class XmlFileUtility
    {
        /// <summary>
        /// Serialzie using XmlSerializer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="aPath"></param>
        /// <returns></returns>
        public static T ReadFromFileXmlSerializer<T>(string aPath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T obj;
            using (TextReader tr = new StreamReader(aPath))
            {
                obj = (T)serializer.Deserialize(tr);
                tr.Close();
            }
            return obj;
        }

        /// <summary>
        /// Deserialize using XmlSerializer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="aObjectToSave"></param>
        /// <param name="aPath"></param>
        public static void SaveToFileXmlSerializer<T>(T aObjectToSave, string aPath)
        {
            XmlSerializer xml = new XmlSerializer(typeof(T));
            using (StreamWriter streamWriter = new StreamWriter(aPath))
            {
                xml.Serialize(streamWriter, aObjectToSave);
                streamWriter.Close();
            }
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T ReadFromFile<T>(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas()))
                {
                    DataContractSerializer ser = new DataContractSerializer(typeof (T));

                    // Deserialize the data and read it from the instance.
                    var t = (T) ser.ReadObject(reader, true);
                    reader.Close();
                    fs.Close();
                    return t;
                }
            }
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="filename"></param>
        public static void SaveToFile<T>(T source, string filename)
        {
            using (FileStream stream = new FileStream(filename, FileMode.Create))
            {
                var settings = new XmlWriterSettings{Indent = true, IndentChars = "\t", NewLineHandling = NewLineHandling.Replace};
                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    DataContractSerializer ser = new DataContractSerializer(typeof (T));

                    ser.WriteObject(writer, source);
                    writer.Close();
                    stream.Close();
                }
            }
        }
    }
}
