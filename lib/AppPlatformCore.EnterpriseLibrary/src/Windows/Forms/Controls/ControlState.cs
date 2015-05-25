using System.Linq;
using System.Windows.Forms;
using AppPlatform.Core.EnterpriseLibrary.Diagnostics;

namespace AppPlatform.Core.EnterpriseLibrary.Windows.Forms.Controls
{
    public class ControlState
    {
        public string ControlName { get; set; }

        public bool Visible { get; set; }

        public static Control FindChildControlByName(Control parrentControl, string controlName)
        {
            if (parrentControl == null)
                return null;

            if (parrentControl.Name == controlName)
                return parrentControl;

            if (Safe.HasItems(parrentControl.Controls))
            {
                foreach (var component in parrentControl.Controls.OfType<Control>())
                {
                    var control = FindChildControlByName(component, controlName);
                    if(control != null)
                    {
                        return control;
                    }
                }
            }

            return null;
        }
    }
}
