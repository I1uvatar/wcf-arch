using System;
using System.Collections.Generic;
using AppPlatform.Core.EnterpriseLibrary.Comparision;
using AppPlatform.Core.EnterpriseLibrary.Diagnostics;
using AppPlatform.Core.EnterpriseLibrary.Types;

namespace AppPlatform.Core.EnterpriseLibrary.ResourceAccess
{
    public class ListChange
    {
        public static ListChanges<T> FindChanges<Container, T>(Container oldData, Container newData, Func<T, object> identifier,
            Func<Container, List<T>> getListToCompare, PropertyFilter filter)
        {
            var oldList = getListToCompare(oldData);
            var newList = getListToCompare(newData);

            var changes = DataComparer.GetListChanges(oldList, newList, identifier, filter);

            return changes;
        }

        public static void CommitListChanges<T>(ListChanges<T> listChanges, IEntityManager<T, long> manager)
        {
            CommitListChanges(listChanges, manager, false);
        }

        public static void CommitListChanges<T>(ListChanges<T> listChanges, IEntityManager<T, long> manager, bool appendOnly)
        {
            listChanges.AddedElements.ForEach(el => manager.Insert(el));
            if (!appendOnly)
            {
                listChanges.RemovedElements.ForEach(el => manager.Delete(el));
            }
            listChanges.ModifiedElements.ForEach(el => manager.Update(el));
        }

        public static void FindAndCommitChanges<Container, ListType>(
            Container oldData,
            Container newData,
            Func<ListType, object> identifier,
            Func<Container, List<ListType>> getListToCompare,
            IEntityManager<ListType, long> manager,
            PropertyFilter filter
            )
        {
            FindAndCommitChanges(oldData, newData, identifier, getListToCompare, null, null, null, manager, filter);
        }

        public static bool FindAndCommitChanges<Container, ListType>(
            Container oldData,
            Container newData,
            Func<ListType, object> identifier,
            Func<Container, List<ListType>> getListToCompare,
            Action<ListType> preInsertNewEntity,
            Action<ListType> preDeleteEntity,
            Action<ListType, ListType> preUpdateEntity,
            IEntityManager<ListType, long> manager,
            PropertyFilter filter
            )
        {
            var changes = FindChanges(oldData, newData, identifier, getListToCompare, filter);

            if (preInsertNewEntity != null && Safe.HasItems(changes.AddedElements))
            {
                changes.AddedElements.ForEach(preInsertNewEntity);
            }

            if (preUpdateEntity != null && Safe.HasItems(changes.ModifiedElements))
            {
                changes.ModifiedElements.ForEach(
                    modifiedItem => preUpdateEntity(
                                        modifiedItem,
                                        getListToCompare(newData).Find(
                                            item => identifier(item) != null
                                                    && identifier(item).Equals(identifier(modifiedItem)))
                                        )
                    );
            }

            if (preDeleteEntity != null && Safe.HasItems(changes.RemovedElements))
            {
                changes.RemovedElements.ForEach(preDeleteEntity);
            }

            CommitListChanges(changes, manager);

            return changes.HasChanges();
        }

        public static void FindAndCommitChanges<ListType>(
            List<ListType> oldList,
            List<ListType> newList,
            Func<ListType, object> identifier,
            IEntityManager<ListType, long> manager,
            PropertyFilter filter
        )
        {
            FindAndCommitChanges
                (oldList,
                 newList,
                 identifier,
                 manager,
                 filter,
                 null);
        }

        /// <summary>
        /// Finds the and commit changes.
        /// </summary>
        /// <typeparam name="ListType">The type of the ist type.</typeparam>
        /// <param name="oldList">The old list.</param>
        /// <param name="newList">The new list.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="useIComparableComparision">The use I comparable comparision.</param>
        public static void FindAndCommitChanges<ListType>(
            List<ListType> oldList,
            List<ListType> newList,
            Func<ListType, object> identifier,
            IEntityManager<ListType, long> manager,
            PropertyFilter filter,
            bool? useIComparableComparision
            )
        {
            FindAndCommitChanges(oldList, newList, identifier, manager, filter, useIComparableComparision, false);
        }

        /// <summary>
        /// Finds the and commit changes.
        /// </summary>
        /// <typeparam name="ListType">The type of the ist type.</typeparam>
        /// <param name="oldList">The old list.</param>
        /// <param name="newList">The new list.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="useIComparableComparision">The use I comparable comparision.</param>
        /// <param name="append">if true delete will not occure</param>
        public static void FindAndCommitChanges<ListType>(
            List<ListType> oldList,
            List<ListType> newList,
            Func<ListType, object> identifier,
            IEntityManager<ListType, long> manager,
            PropertyFilter filter,
            bool? useIComparableComparision,
            bool append
            )
        {
            FindAndCommitChanges(oldList, newList, identifier, manager, filter, useIComparableComparision, false, null);
        }

        /// <summary>
        /// Finds the and commit changes.
        /// </summary>
        /// <typeparam name="ListType">The type of the ist type.</typeparam>
        /// <param name="oldList">The old list.</param>
        /// <param name="newList">The new list.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="useIComparableComparision">The use I comparable comparision.</param>
        /// <param name="append">If true delete will not occure.</param>
        /// <param name="preUpdateEntity">Action which should be executed before update.</param>
        public static void FindAndCommitChanges<ListType>(
            List<ListType> oldList,
            List<ListType> newList,
            Func<ListType, object> identifier,
            IEntityManager<ListType, long> manager,
            PropertyFilter filter,
            bool? useIComparableComparision,
            bool append,
            Action<ListType, ListType> preUpdateEntity
            )
        {
            var changes = DataComparer.GetListChanges(oldList, newList, identifier, filter, null, useIComparableComparision);

            if (preUpdateEntity != null && Safe.HasItems(changes.ModifiedElements))
            {
                changes.ModifiedElements.ForEach(
                    modifiedItem => preUpdateEntity(
                        oldList.Find(
                            item => identifier(item) != null && identifier(item).Equals(identifier(modifiedItem))),
                        modifiedItem)
                    );
            }

            CommitListChanges(changes, manager, append);
        }

        public static void FindAndCommitChanges<ListType>(
            List<ListType> oldList,
            List<ListType> newList,
            Func<ListType, object> identifier,
            Action<ListType> insertNewEntity,
            Action<ListType> deleteEntity,
            Action<ListType> updateEntity,
            PropertyFilter filter
            )
        {
            FindAndCommitChanges(
                oldList,
                newList,
                identifier,
                insertNewEntity,
                deleteEntity,
                updateEntity,
                filter,
                null);
        }

        public static ListChanges<ListType> FindAndCommitChanges<ListType>(
            List<ListType> oldList,
            List<ListType> newList,
            Func<ListType, object> identifier,
            Action<ListType> insertNewEntity,
            Action<ListType> deleteEntity,
            Action<ListType> updateEntity,
            PropertyFilter filter,
            bool? useIComparableComparision)
        {
            var changes = DataComparer.GetListChanges(oldList, newList, identifier, filter, null, useIComparableComparision);

            if (insertNewEntity != null)
            {
                changes.AddedElements.ForEach(insertNewEntity);
            }

            if (deleteEntity != null)
            {
                changes.RemovedElements.ForEach(deleteEntity);
            }

            if (updateEntity != null)
            {
                changes.ModifiedElements.ForEach(updateEntity);
            }

            return changes;
        }
    }
}
