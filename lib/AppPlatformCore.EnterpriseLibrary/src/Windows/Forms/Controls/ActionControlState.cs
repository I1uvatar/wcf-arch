using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppPlatform.Core.EnterpriseLibrary.Windows.Forms.Controls
{
    public enum AllActionControlsStates
    {
        AllActive = 1,
        AllDisabled = 2
    }
    
    public enum ActionControlStates
    {
        Active = 1,
        Disabled = 2,
        NotVisible = 3
    }

    public class ActionControlState
    {
        public ActionControlState(string name)
            : this(name, ActionControlStates.Active)
        { }

        public ActionControlState(string name, ActionControlStates state)
        {
            this.Name = name;
            this.State = state;
        }

        public string Name { get; set; }
        public ActionControlStates State { get; set; }
    }
}
