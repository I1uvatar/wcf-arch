using System.Xml.Serialization;

namespace AppPlatform.Core.EnterpriseLibrary.Windows.Forms.MetaForms.Concept
{
    public class Concept
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("type")]
        public ConceptType ConceptType { get; set; }

        [XmlArray("constraints")]
        [XmlArrayItem("constraint")]
        public string[] Constraints { get; set; }

        public Concept()
        {
        }

        public Concept(string name, ConceptType type, string description, string[] constraints)
        {
            Name = name;
            ConceptType = type;
            Description = description;
            Constraints = constraints;
        }
    }
}