using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppPlatform.Core.EnterpriseLibrary.Services.Execution
{
    public interface IActionExecutionWrapper
    {
        void Execute(Execute action);
    }
}
