using System;
using System.IO;
using System.Messaging;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using AppPlatform.Core.EnterpriseLibrary.Logging;

namespace AppPlatform.Core.EnterpriseLibrary.Messaging
{
    public class RawMessageFormatter : IMessageFormatter
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            return new RawMessageFormatter();
        }

        /// <summary>
        /// When implemented in a class, determines whether the formatter can deserialize the contents of the message.
        /// </summary>
        /// <returns>
        /// true if the formatter can deserialize the message; otherwise, false.
        /// </returns>
        /// <param name="message">The <see cref="T:System.Messaging.Message" /> to inspect. </param>
        public bool CanRead(Message message)
        {
            return true;
        }

        /// <summary>
        /// When implemented in a class, reads the contents from the given message and creates an object that contains data from the message.
        /// </summary>
        /// <returns>
        /// The deserialized message.
        /// </returns>
        /// <param name="message">The <see cref="T:System.Messaging.Message" /> to deserialize. </param>
        public object Read(Message message)
        {
            using (var reader = XmlReader.Create(message.BodyStream))
            {
                var xDoc = XDocument.Load(reader);
                reader.Close();

                return HslLogEntry.FromXmlDocument(xDoc);
            }
        }

        /// <summary>
        /// When implemented in a class, serializes an object into the body of the message.
        /// </summary>
        /// <param name="message">The <see cref="T:System.Messaging.Message" /> that will contain the serialized object. </param>
        /// <param name="obj">The object to be serialized into the message. </param>
        public void Write(Message message, object obj)
        {
            var data = obj as HslLogEntry;
            if (data == null)
            {
                var formatter = new XmlMessageFormatter();
                formatter.Write(message, obj);
                return;
            }

            var xDoc = data.ToXmlDocument();
            if (xDoc == null)
            {
                return;
            }

            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                Indent = true,
                IndentChars = "",
                NewLineChars = Environment.NewLine
            };

            message.BodyStream = new MemoryStream(2048);
            using (var writer = XmlWriter.Create(message.BodyStream, settings))
            {
                xDoc.WriteTo(writer);
                writer.Flush();
                writer.Close();
            }
        }
    }
}
