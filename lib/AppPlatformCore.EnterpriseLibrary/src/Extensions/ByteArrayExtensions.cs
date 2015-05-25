using System.IO;
using System.Text;

namespace AppPlatform.Core.EnterpriseLibrary.Extensions
{
    public static class ByteArrayExtensions
    {
        public static string ReadString(this byte[] source)
        {
            if (source == null)
            {
                return null;
            }
            string rtf;
            var newMemoryStream = new MemoryStream(source);
            using (var reader = TextReader.Synchronized(new StreamReader(newMemoryStream)))
            {
                rtf = reader.ReadToEnd();
            }
            //Encoding enc = Encoding.Default;
            //rtf = enc.GetString(memoryStream.GetBuffer());
            return rtf;
        }

        public static string ReadString(this byte[] source, Encoding encoding)
        {
            if (source == null)
            {
                return null;
            }
            return encoding.GetString(source);

            //string rtf;
            //var newMemoryStream = new MemoryStream(source);
            //using (var reader = TextReader.Synchronized(new StreamReader(newMemoryStream, encoding)))
            //{
            //    rtf = reader.ReadToEnd();
            //}
            //return rtf;
            
        }
    }
}
