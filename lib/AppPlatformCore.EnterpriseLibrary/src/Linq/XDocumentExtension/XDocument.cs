using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace AppPlatform.Core.EnterpriseLibrary.Linq.XDocumentExtension
{
    public static class XDocumentExtension
    {
        public static XmlElement ToXmlElement(this XDocument instance)
        {
            if (instance == null)
            {
                return null;
            }

            var newDoc = new XmlDocument();
            newDoc.LoadXml(instance.ToString());

            return newDoc.DocumentElement; 
        }

        public static XmlDocument ToXmlDocument(this XElement instance)
        {
            if (instance == null)
            {
                return null;
            }

            var sb = new StringBuilder();
            var xws = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true };

            using (var xw = XmlWriter.Create(sb, xws))
            {
                instance.WriteTo(xw);
            }

            var doc = new XmlDocument();
            doc.LoadXml(sb.ToString());

            return doc;
        }

        public static XmlElement ToXmlElement(this XElement instance)
        {
            if (instance == null)
            {
                return null;
            }

            var doc = instance.ToXmlDocument();

            return doc.DocumentElement;
        }

        public static XElement ToXElement(this XmlElement element)
        {
            if (element == null)
            {
                return null;
            }

            return XDocument.Parse(element.OuterXml).Root;
        }

        public static XDocument ToXDocument(this XmlElement element)
        {
            if (element == null)
            {
                return null;
            }

            return XDocument.Parse(element.OuterXml);
        }

        //public static XDocument LoadFromXmlElement(XmlElement element)
        //{
        //    if (element == null)
        //    {
        //        return null;
        //    }

        //    return XDocument.Parse(element.OuterXml);
        //}
    }
}