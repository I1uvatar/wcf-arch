using System;
using System.Collections.Generic;
using System.Data.Common;

namespace AppPlatform.Core.EnterpriseLibrary.DatabaseUtility
{
    /// <summary>
    /// Provides context information for DB queries
    /// </summary>
    public interface IDbSessionContext
    {
        /// <summary>
        /// Set up default context parameters of a DB operation.
        /// </summary>
        /// <param name="aCommand">Command, pending for execution.</param>
        /// <param name="aParameterList">List of command's parameters.</param>
        void SetupParameters(DbCommand aCommand, IList<string> aParameterList);
    }
}
