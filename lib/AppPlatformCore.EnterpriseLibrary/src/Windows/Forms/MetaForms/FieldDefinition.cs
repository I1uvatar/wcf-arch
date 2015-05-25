using System.Xml.Serialization;
using AppPlatform.Core.EnterpriseLibrary.Windows.Forms.MetaForms.Concept;

namespace AppPlatform.Core.EnterpriseLibrary.Windows.Forms.MetaForms
{
    public class FieldDefinition
    {
        public FieldDefinition()
        {
        }

        public FieldDefinition(string name, ConceptType type, string labelText, string[] constraints)
        {
            Concept = new Concept.Concept(name, type, labelText, constraints);
        }

        [XmlElement("concept")]
        public Concept.Concept Concept { get; set; }

        [XmlElement("inputControlLength")]
        public int InputControlLength { get; set; }

        [XmlElement("groupName")]
        public string GroupName { get; set; }

        [XmlElement("column")]
        public int Column { get; set; }

        [XmlElement("columnSpan")]
        public int columnSpan { get; set; }

        [XmlElement("toolTip")]
        public string ToolTip { get; set; }
    }
}
