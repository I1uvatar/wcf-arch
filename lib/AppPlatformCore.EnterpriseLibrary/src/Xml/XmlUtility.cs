using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace AppPlatform.Core.EnterpriseLibrary.Xml
{
    /// <summary>
    /// Xml document utilty
    /// </summary>
    public class XmlUtility
    {
        #region Public Methods

        public static List<XmlSchema> GetXsdSchemaList(List<string> xsdList, ValidationEventHandler validationHandler)
        {
            var schemaList = new List<XmlSchema>();
            xsdList.ForEach(xsdSchema =>
            {
                XmlSchema schema = GetXsdSchema(xsdSchema, validationHandler);
                //XmlSchema schema = XmlSchema.Read(new MemoryStream(Encoding.Default.GetBytes(xsdSchema)), validationHandler);
                schemaList.Add(schema);
            });
            return schemaList;
        }

        public static XmlSchema GetXsdSchema(string xsd, ValidationEventHandler validationHandler)
        {
            XmlSchema schema = XmlSchema.Read(new XmlTextReader(new StringReader(xsd)), validationHandler);
            return schema;
        }

        /// <summary>
        /// Validates XML for specified XSD schema.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="xsdSchema">The XSD schema as string</param>
        /// <param name="validationHandler">The validation event handler.</param>
        public static void Validate(string xml, string xsdSchema, ValidationEventHandler validationHandler)
        {
            Validate(xml, new List<string> { xsdSchema }, validationHandler);
        }

        /// <summary>
        /// Validates XML for specified XSD schema.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="xsdSchemaList">The XSD schema list.</param>
        /// <param name="validationHandler">The validation event handler.</param>
        public static void Validate(string xml, List<string> xsdSchemaList, ValidationEventHandler validationHandler)
        {
            XDocument xDocument = XDocument.Load(new XmlTextReader(new StringReader(xml)));
            var schemaList = GetXsdSchemaList(xsdSchemaList, validationHandler);
            Validate(xDocument, schemaList, validationHandler);
        }

        /// <summary>
        /// Validates XML for specified XSD schema.
        /// </summary>
        /// <param name="xsdSchema">The XSD schema.</param>
        /// <param name="xml">The XML.</param>
        /// <param name="validationHandler">The validation handler.</param>
        public static void Validate(XDocument xml, XmlSchema xsdSchema, ValidationEventHandler validationHandler)
        {
            Validate(xml, new List<XmlSchema> { xsdSchema }, validationHandler);
        }

        /// <summary>
        /// Validates XML for specified XSD schema.
        /// </summary>
        /// <param name="xsdSchema">The XSD schema list.</param>
        /// <param name="xml">The XML.</param>
        /// <param name="validationHandler">The validation handler.</param>
        public static void Validate(XDocument xml, List<XmlSchema> xsdSchema, ValidationEventHandler validationHandler)
        {
            XmlSchemaSet schemaSet = CreateXmlSchemaSet(xsdSchema);
            xml.Validate(schemaSet, validationHandler);
        }

        /// <summary>
        /// Return xml as string with declaration.
        /// </summary>
        /// <param name="xDocument">XDocument object.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this XDocument instance.
        /// </returns>
        public static string ToXmlString(XDocument xDocument)
        {
            using (var sw = new StringWriter())
            {
                using (var xtw = new XmlTextWriter(sw))
                {
                    xtw.Formatting = Formatting.Indented;
                    xDocument.WriteTo(xtw);
                    
                    return sw.ToString();
                }
            }
        }

        #endregion

        #region Private Methods

        private static XmlSchemaSet CreateXmlSchemaSet(List<XmlSchema> xsdSchemaList)
        {
            var schemaSet = new XmlSchemaSet();
            xsdSchemaList.ForEach(xsdSchema => schemaSet.Add(xsdSchema));
            return schemaSet;
        }

        #endregion
    }
}
