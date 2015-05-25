using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace AppPlatform.Core.EnterpriseLibrary.Serialization
{
    /// <summary> 
    ///  Helper class for binary serialization.
    /// </summary>
    public class BinarySerializer
    {

        /// <summary>
        /// Serializes the specified data.
        /// </summary>
        /// <typeparam name="TInputType">The type of the input data.</typeparam>
        /// <param name="data">Data to serialize.</param>
        /// <returns>Serialized byte array of input data.</returns>
        public static byte[] Serialize<TInputType>(TInputType data)
        {
            var formater = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
            using (var stream = new MemoryStream())
            {
                formater.Serialize(stream, data);
                stream.Position = 0;
                //tmpStream.Seek(0, SeekOrigin.Begin);

                return stream.GetBuffer();
            }
        }

        public static TOutputType Deserialize<TOutputType>(byte[] bytes)
        {
            var formater = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
            using (var stream=new MemoryStream(bytes))
            {
                var obj = formater.Deserialize(stream);
                
                return obj is TOutputType ? (TOutputType) obj : default(TOutputType);
            }
        }

    }
}
