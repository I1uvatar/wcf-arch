using System;
using System.Collections;
using System.Collections.Generic;

namespace AppPlatform.Core.EnterpriseLibrary.Comparision
{
    public class ListCompare<T> : IListCompare
    {
        private readonly Func<T, object> elementIdentifier;

        public ListCompare(Func<T, object> elementIdentifier)
        {
            this.elementIdentifier = elementIdentifier;
        }

        #region IListCompare Members

        public bool Compare(ICollection firstCollection, ICollection secondCollection, Dictionary<Type, IListCompare> listComparers, PropertyFilter filter)
        {
            var firstList = new List<T>();
            var secondList = new List<T>();

            if (firstCollection != null)
            {
                foreach (var item in firstCollection)
                {
                    if (item is T)
                    {
                        firstList.Add((T)item);
                    }
                }    
            }

            if (secondCollection != null)
            {
                foreach (var item in secondCollection)
                {
                    if (item is T)
                    {
                        secondList.Add((T)item);
                    }
                }
            }

            return DataComparer.GetListChanges(firstList, secondList, this.elementIdentifier, filter, listComparers, null).HasChanges();
        }

        public bool Compare(ICollection firstCollection, ICollection secondCollection)
        {
            return this.Compare(firstCollection, secondCollection, null, null);
        }

        #endregion
    }
}
