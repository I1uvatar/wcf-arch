using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Common;

namespace AppPlatform.Core.EnterpriseLibrary.DatabaseUtility
{
    public interface ISQLExceptionProcessor
    {
        void Process(SqlException ex,DbCommand aCommand);
    }
}
