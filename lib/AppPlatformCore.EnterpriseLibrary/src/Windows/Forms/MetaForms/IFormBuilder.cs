using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AppPlatform.Core.EnterpriseLibrary.Windows.Forms.MetaForms;

namespace AppPlatform.Core.EnterpriseLibrary.Windows.Forms.MetaForms
{
    public interface IFormBuilder
    {
        void Create(Control container, MetaFormDefinition concepts, object data);
        Control CreateFromConcept(FieldDefinition concept, object dataObject);
        Control CreateLabel(string labelText);
    }
}
