using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using AppPlatform.Core.EnterpriseLibrary.Xml;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace AppPlatform.Core.EnterpriseLibrary.Logging
{
    /// <summary>
    /// Base class for event logging
    /// </summary>
    public class HslLogEntry : LogEntry
    {
        private const string CustomDataKey = "CustomData";
        private const string CustomDataXmlKey = "CustomDataXml";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="category"></param>
        /// <param name="priority"></param>
        /// <param name="eventId"></param>
        /// <param name="severity"></param>
        /// <param name="title"></param>
        /// <param name="customData"></param>
        public HslLogEntry(object message, string category, int priority, int eventId, TraceEventType severity, string title, object customData)
            : base(message, category, priority, eventId, severity, title, new Dictionary<string, object>())
        {
            if (customData != null)
            {
                this.ExtendedProperties[CustomDataKey] = customData;
                this.ExtendedProperties[CustomDataXmlKey] = this.ToXElement(customData);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="category"></param>
        /// <param name="priority"></param>
        /// <param name="eventId"></param>
        /// <param name="severity"></param>
        /// <param name="title"></param>
        /// <param name="customData"></param>
        public HslLogEntry(object message, string category, int priority, int eventId, TraceEventType severity, string title, XElement customData)
            : base(message, category, priority, eventId, severity, title, new Dictionary<string, object>())
        {
            if (customData != null)
            {
                this.ExtendedProperties[CustomDataXmlKey] = customData;
            }
        }

        /// <summary>
        /// User id
        /// </summary>
        /// <remarks>None</remarks>
        public string UserId
        {
            get
            {
                return this.GetExtendedPropertySafe("UserId") as string;
            }
            set
            {
                this.ExtendedProperties["UserId"] = value;
            }
        }

        /// <summary>
        /// XML reperesentation of additional custom data
        /// </summary>
        public XElement CustomDataXml
        {
            get { return (XElement)this.GetExtendedPropertySafe(CustomDataXmlKey); }
        }

        protected object GetExtendedPropertySafe(string key)
        {
            if(this.ExtendedProperties.ContainsKey(key))
            {
                return this.ExtendedProperties[key];
            }

            return null;
        }

        /// <summary>
        /// Retrieves a custom object, based on the custom XML data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CustomData<T>() where T : class, new()
        {
            try
            {
                T custom;
                if (this.ExtendedProperties.ContainsKey(CustomDataKey))
                {
                    custom = (T)this.ExtendedProperties[CustomDataKey];
                }
                else
                {
                    custom = this.FromXml<T>(this.CustomDataXml.Elements().First());
                    this.ExtendedProperties[CustomDataKey] = custom;
                }

                return custom;
            }
            catch (InvalidCastException e)
            {
                throw new InvalidOperationException(String.Format("Cast to {0} failed.", typeof(T).Name), e);
            }
        }

        public void SetCustomData(object data)
        {
            this.ExtendedProperties[CustomDataKey] = data;
        }

        /// <summary>
        /// Returns an XML DOM representation of the instance.
        /// </summary>
        /// <returns></returns>
        public XDocument ToXmlDocument()
        {
            var basicData = new XElement(
                "HslLogEntry",
                new XElement("Title", this.Title),
                new XElement("Category", this.Categories),
                new XElement("Severity", this.Severity),
                new XElement("Priority", this.Priority),
                new XElement("Message", this.Message),
                new XElement("MachineName", this.MachineName),
                new XElement("TimeStamp", this.TimeStamp),
                new XElement("UserId", this.UserId),
                new XElement("EventId", this.EventId),
                new XElement("AppDomainName", this.AppDomainName),
                new XElement("ProcessId", this.ProcessId),
                new XElement("ProcessName", this.ProcessName),
                new XElement("ManagedThreadName", this.ManagedThreadName),
                new XElement("Win32ThreadId", this.Win32ThreadId),
                new XElement("ActivityId", this.ActivityId),
                new XElement("RelatedActivityId", this.RelatedActivityId)
                );

            var xmlDocument = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                basicData
            );

            var customData = this.GetCustomData();
            if (customData != null)
            {
                basicData.Add(new XElement("CustomData", customData));
            }

            return xmlDocument;
        }

        /// <summary>
        /// Builds a domain object from a XML DOM representation.
        /// </summary>
        /// <param name="xDoc"></param>
        /// <returns></returns>
        public static HslLogEntry FromXmlDocument(XDocument xDoc)
        {
            if (xDoc == null)
            {
                return null;
            }

            var basicData = xDoc.Element("HslLogEntry");
            if (basicData == null)
            {
                return null;
            }

            var data = new XElementValueExtractor(basicData);
            string title = data.GetString("Title");
            string message = data.GetString("Message");
            string categories = data.GetString("Category");
            TraceEventType severity = data.GetEnum<TraceEventType>("Severity");
            int priority = data.GetInt("Priority");
            int eventId = data.GetInt("EventId");
            var customData = xDoc.Element("HslLogEntry").Element("CustomData");

            var anEntry = new HslLogEntry(message, categories, priority, eventId, severity, title, customData)
            {
                MachineName = data.GetString("MachineName"),
                TimeStamp = data.GetDateTime("TimeStamp"),
                AppDomainName = data.GetString("AppDomainName"),
                ProcessId = data.GetString("ProcessId"),
                UserId = data.GetString("UserId"),
                ProcessName = data.GetString("ProcessName"),
                ManagedThreadName = data.GetString("ManagedThreadName"),
                Win32ThreadId = data.GetString("Win32ThreadId"),
                ActivityId = data.GetGuid("ActivityId"),
                RelatedActivityId = data.GetGuid("RelatedActivityId")
            };

            return anEntry;
        }

        protected virtual XElement GetCustomData()
        {
            return this.CustomDataXml;
        }

        protected XElement ToXElement(object data)
        {
            // Add this namespace with empty prefix to skip adding default namespaces to the serialized XML
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "http://www.w3.org/2001/XMLSchema");

            using (var stream = new MemoryStream())
            {
                var serializer = new XmlSerializer(data.GetType());
                serializer.Serialize(stream, data, namespaces);

                stream.Position = 0;
                using (var reader = XmlReader.Create(stream))
                {
                    XElement customXml = null;
                   //TODO ovo treba promeniti, stavljeno zbog problema sa serualize string-a koji sadrzi RTF
                    try
                    {
                        customXml = XElement.Load(reader);
                    }
                    catch
                    {
                        
                    }
                   
                    return customXml;
                }
            }
        }

        protected T FromXml<T>(XElement data) where T : class, new()
        {
            if (data == null)
            {
                return null;
            }

            using (var stream = new MemoryStream())
            {
                var settings = new XmlWriterSettings { Encoding = new UnicodeEncoding(), OmitXmlDeclaration = false };
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    data.WriteTo(writer);
                    writer.Flush();
                }

                stream.Position = 0;
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stream);
            }
        }
    }
}
