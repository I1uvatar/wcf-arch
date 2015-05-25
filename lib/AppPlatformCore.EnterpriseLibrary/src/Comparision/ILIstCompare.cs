using System;
using System.Collections;
using System.Collections.Generic;

namespace AppPlatform.Core.EnterpriseLibrary.Comparision
{
    public interface IListCompare
    {
        /// <summary>
        /// Check if data in two list are the same
        /// </summary>
        /// <param name="firstCollection"></param>
        /// <param name="secondCollection"></param>
        /// <returns>True if data are the same, otherwise false</returns>
        bool Compare(ICollection firstCollection, ICollection secondCollection);

        /// <summary>
        /// Check if data in two list are the same
        /// </summary>
        /// <param name="firstCollection">The first collection.</param>
        /// <param name="secondCollection">The second collection.</param>
        /// <param name="listComparers">The list comparers.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>
        /// True if data are the same, otherwise false
        /// </returns>
        bool Compare(ICollection firstCollection, ICollection secondCollection, Dictionary<Type, IListCompare> listComparers, PropertyFilter filter);
    }
}
