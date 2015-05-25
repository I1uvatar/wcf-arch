using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppPlatform.Core.EnterpriseLibrary.ResourceAccess
{
    public class ListQueryResult<Entity>
    {
        public ListQueryResult(List<Entity> result, long searchLimit)
        {
            Result = result;
            SearchLimit = searchLimit;
        }

        public List<Entity> Result{ get; set;}
        public long SearchLimit{ get; set;}
    }
}
