using System.Linq;
using System.Windows.Forms;
using AppPlatform.Core.EnterpriseLibrary.Diagnostics;

namespace AppPlatform.Core.EnterpriseLibrary.Windows.Forms.Controls
{
    public class ControlSetter
    {
        /// <summary>
        /// Sets view state
        /// </summary>
        /// <param name="view"></param>
        /// <param name="clearView">If true clears data from view</param>
        /// <param name="setEnabled">If true view is enabled, otherwise set disabled</param>
        /// <param name="setReadOnly">If true all controls on view are sets to be ReadOnly</param>
        public static void SetView(Control view, bool clearView, bool setEnabled, bool setReadOnly) 
        {
            if (view == null)
                return;

            if (Safe.HasItems(view.Controls)
                && !(view is IInputControl || view is IActionControl))
            {
                foreach (var component in view.Controls.OfType<Control>())
                {
                    SetView(component, clearView, setEnabled, setReadOnly);
                }
            }
            else if (view is IInputControl)
            {
                if (clearView)
                {
                    ((IInputControl)view).Clear();
                }
                ((IInputControl)view).SetEnabled(setEnabled);
                ((IInputControl)view).SetReadOnly(setReadOnly);

            }
            else if (view is IActionControl)
            {
                ((IActionControl)view).SetEnabled(setEnabled);
                ((IActionControl)view).SetReadOnly(setReadOnly);
            }
        }

        public static void ClearView(Control view, bool clearView)
        {
            if (view == null)
                return;

            if (Safe.HasItems(view.Controls)
                && !(view is IInputControl || view is IActionControl))
            {
                foreach (var component in view.Controls.OfType<Control>())
                {
                    ClearView(component, clearView);
                }
            }
            else if (view is IInputControl)
            {
                if (clearView)
                {
                    ((IInputControl) view).Clear();
                }
            }
        }

    }
}
