using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AppPlatform.Core.EnterpriseLibrary.Diagnostics;

namespace AppPlatform.Core.EnterpriseLibrary.Windows.Forms.Controls
{
   public class ControlStateModeWalker
    {
       /// <summary>
       /// Finds all elements by name, and then apply state to them
       /// </summary>
       /// <param name="view"></param>
       /// <param name="controlStateModes"></param>
       public static void ApplyControlStateMode(Control view, List<ControlStateMode> controlStateModes)
       {
           if(Safe.HasItems(controlStateModes))
           {
               foreach (var controlStateMode in controlStateModes)
               {
                   Control control = FindControlByName(view, controlStateMode.Name);
                   if(control != null )
                   {
                       if(controlStateMode.Enabled.HasValue)
                       {
                           control.Enabled = controlStateMode.Enabled.Value;
                       }
                       if (controlStateMode.Visible.HasValue)
                       {
                           control.Visible = controlStateMode.Visible.Value;
                       }
                       if (controlStateMode.Height.HasValue)
                       {
                           control.Height = controlStateMode.Height.Value;
                       }
                       if (!string.IsNullOrEmpty(controlStateMode.Text))
                       {
                           control.Text = controlStateMode.Text;
                       }
                   }
               }
           }
       }

       private static Control FindControlByName(Control view, string name)
       {
           if (view == null)
               return null;
           if (view.Name == name)
               return view;

           if (Safe.HasItems(view.Controls))
           {
               foreach (var component in view.Controls.OfType<Control>())
               {
                   var control = FindControlByName(component, name);
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
