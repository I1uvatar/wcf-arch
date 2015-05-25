using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace AppPlatform.Core.EnterpriseLibrary.Object
{
    /// <summary>
    /// Helper class for cloning objects. 
    /// </summary>
    public static class ObjectCloner
    {
        /// <summary>
        /// Make a clone of the object. Classes to be cloned must be serializable!
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="origObj">The object to be cloned.</param>
        /// <returns>The cloned object.</returns>
        public static T BinaryClone<T>(T origObj) where T: class
        {
            using(var stream = new MemoryStream())
            {
                var formater = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));

                formater.Serialize(stream, origObj);
                stream.Seek(0, SeekOrigin.Begin);
                object clonedObj = formater.Deserialize(stream);
                return clonedObj as T;
            }
        }

        /// <summary>
        /// Make a clone of the object using DataContractSerializer.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="origObj">The object to be cloned.</param>
        /// <returns>The cloned object.</returns>
        public static T Clone<T>(T origObj) where T : class
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(stream, origObj);
                stream.Seek(0, SeekOrigin.Begin);
                var clonedObj = (T)serializer.ReadObject(stream);
                return clonedObj;
            }
        }
    }
}
