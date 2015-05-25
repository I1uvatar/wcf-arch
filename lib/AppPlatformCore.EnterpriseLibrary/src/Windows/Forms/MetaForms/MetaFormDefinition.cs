using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace AppPlatform.Core.EnterpriseLibrary.Windows.Forms.MetaForms
{
    [XmlRoot("metaFormDefinition")]
    public class MetaFormDefinition
    {
        [XmlElement("titleField")]
        public string TitleField { get; set; }

        [XmlElement("titleImage")]
        public string TitleImage { get; set; }

        [XmlArray("fieldDefinitions")]
        [XmlArrayItem("fieldDefinition")]
        public List<FieldDefinition> FieldDefinitions { get; set; }

        public MetaFormDefinition()
        {
            FieldDefinitions = new List<FieldDefinition>();
        }

        #region Serialization
        private const string xmlNamespace = "http://hsl.si/doctorhouse/metaformdefinition/";
        public static MetaFormDefinition Load(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MetaFormDefinition), xmlNamespace);
            using (StreamReader reader = new StreamReader(path, Encoding.Unicode))
            {
                return (MetaFormDefinition)serializer.Deserialize(reader);
            }
        }

        public void Save(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MetaFormDefinition), xmlNamespace);
            using (StreamWriter writer = new StreamWriter(path, false, Encoding.Unicode))
            {
                serializer.Serialize(writer, this);
            }
        }
        #endregion
    }
}
            