using System;
using System.Collections.Generic;
using AppPlatform.Core.EnterpriseLibrary.Diagnostics;

namespace AppPlatform.Core.EnterpriseLibrary.Comparision
{
    /// <summary>
    /// Data class which contains list change
    /// </summary>
    /// <typeparam name="TMember"></typeparam>
    [Serializable]
    public class ListChanges<TMember>
    {
        /// <summary>
        /// Removed elements
        /// </summary>
        public List<TMember> RemovedElements { get; set; }
        /// <summary>
        /// Added elements
        /// </summary>
        public List<TMember> AddedElements { get; set; }
        /// <summary>
        /// Modified elements
        /// </summary>
        public List<TMember> ModifiedElements { get; set; }

        /// <summary>
        /// Idicates if there are changes in the object
        /// </summary>
        /// <returns></returns>
        public bool HasChanges()
        {
            return
                Safe.HasItems(this.RemovedElements) || Safe.HasItems(this.AddedElements) ||
                Safe.HasItems(this.ModifiedElements);
        }
    }
}
