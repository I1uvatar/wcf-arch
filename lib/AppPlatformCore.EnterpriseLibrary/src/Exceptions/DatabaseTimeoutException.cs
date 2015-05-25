using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppPlatform.Core.EnterpriseLibrary.Exceptions
{
    public class DatabaseTimeoutException : ApplicationException
    {
        public string commandText
        {
            get;
            set;
        }

        public override string Message
        {
            get
            {
                return "DatabaseTimeout";
            }
        }
    }
}
